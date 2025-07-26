using DepenMock.XUnit;
using Sudoku.Domain.Events;
using Sudoku.Domain.ValueObjects;
using XenobiaSoft.Sudoku.Infrastructure.EventHandling;

namespace UnitTests.Infrastructure.EventHandling;

public class GameCompletedEventHandlerTests : BaseTestByAbstraction<GameCompletedEventHandler, IDomainEventHandler<GameCompletedEvent>>
{
    [Fact]
    public async Task HandleAsync_WithValidEvent_LogsInformation()
    {
        // Arrange
        var gameId = GameId.New();
        var statistics = GameStatistics.Create();
        var domainEvent = new GameCompletedEvent(gameId, statistics);

        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);

        // Assert
        Logger.InformationLogs().ContainsMessage("Game completed");
    }

    [Fact]
    public async Task HandleAsync_WithValidEvent_CompletesSuccessfully()
    {
        // Arrange
        var gameId = GameId.New();
        var statistics = GameStatistics.Create();
        var domainEvent = new GameCompletedEvent(gameId, statistics);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.HandleAsync(domainEvent);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task HandleAsync_WithValidEvent_LogsCorrectGameId()
    {
        // Arrange
        var gameId = GameId.New();
        var statistics = GameStatistics.Create();
        var domainEvent = new GameCompletedEvent(gameId, statistics);

        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);

        // Assert
        Logger.InformationLogs().ContainsMessage(gameId.Value.ToString());
    }

    [Fact]
    public async Task HandleAsync_WithDifferentGameId_LogsCorrectGameId()
    {
        // Arrange
        var gameId = GameId.New();
        var statistics = GameStatistics.Create();
        var domainEvent = new GameCompletedEvent(gameId, statistics);

        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);

        // Assert
        Logger.InformationLogs().ContainsMessage(gameId.Value.ToString());
    }
}