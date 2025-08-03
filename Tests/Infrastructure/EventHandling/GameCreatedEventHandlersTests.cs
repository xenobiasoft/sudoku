using DepenMock.XUnit;
using Sudoku.Domain.Events;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.EventHandling;

namespace UnitTests.Infrastructure.EventHandling;

public class GameCreatedEventHandlerTests : BaseTestByAbstraction<GameCreatedEventHandler, IDomainEventHandler<GameCreatedEvent>>
{
    [Fact]
    public async Task HandleAsync_WithValidEvent_LogsInformation()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var domainEvent = new GameCreatedEvent(gameId, playerAlias, difficulty);

        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);

        // Assert
        Logger.InformationLogs().ContainsMessage("Game Created");
    }

    [Fact]
    public async Task HandleAsync_WithValidEvent_CompletesSuccessfully()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Hard;
        var domainEvent = new GameCreatedEvent(gameId, playerAlias, difficulty);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.HandleAsync(domainEvent);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task HandleAsync_WithDifferentDifficulties_LogsCorrectDifficulty()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Expert;
        var domainEvent = new GameCreatedEvent(gameId, playerAlias, difficulty);

        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);

        // Assert
        Logger.InformationLogs().ContainsMessage("Expert");
    }

    [Fact]
    public async Task HandleAsync_WithDifferentPlayerAlias_LogsCorrectPlayerAlias()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("DifferentPlayer");
        var difficulty = GameDifficulty.Easy;
        var domainEvent = new GameCreatedEvent(gameId, playerAlias, difficulty);

        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);

        // Assert
        Logger.InformationLogs().ContainsMessage("DifferentPlayer");
    }
}