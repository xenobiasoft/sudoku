using DepenMock.XUnit;
using Sudoku.Domain.Events;
using Sudoku.Domain.ValueObjects;
using XenobiaSoft.Sudoku.Infrastructure.EventHandling;

namespace UnitTests.Infrastructure.EventHandling;

public class MoveMadeEventHandlerTests : BaseTestByAbstraction<MoveMadeEventHandler, IDomainEventHandler<MoveMadeEvent>>
{
    [Fact]
    public async Task HandleAsync_WithValidEvent_LogsDebugInformation()
    {
        // Arrange
        var gameId = GameId.New();
        var row = 0;
        var column = 1;
        var value = 5;
        var statistics = GameStatistics.Create();
        var domainEvent = new MoveMadeEvent(gameId, row, column, value, statistics);

        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);

        // Assert
        Logger.DebugLogs().ContainsMessage("Move made");
    }

    [Fact]
    public async Task HandleAsync_WithValidEvent_CompletesSuccessfully()
    {
        // Arrange
        var gameId = GameId.New();
        var row = 5;
        var column = 3;
        var value = 9;
        var statistics = GameStatistics.Create();
        var domainEvent = new MoveMadeEvent(gameId, row, column, value, statistics);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.HandleAsync(domainEvent);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task HandleAsync_WithNullValue_LogsCorrectly()
    {
        // Arrange
        var gameId = GameId.New();
        var row = 2;
        var column = 4;
        int? value = null;
        var statistics = GameStatistics.Create();
        var domainEvent = new MoveMadeEvent(gameId, row, column, value, statistics);

        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);

        // Assert
        Logger.DebugLogs().ContainsMessage("Move made");
    }

    [Fact]
    public async Task HandleAsync_WithDifferentCoordinates_LogsCorrectCoordinates()
    {
        // Arrange
        var gameId = GameId.New();
        var row = 8;
        var column = 7;
        var value = 1;
        var statistics = GameStatistics.Create();
        var domainEvent = new MoveMadeEvent(gameId, row, column, value, statistics);

        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);

        // Assert
        Logger.DebugLogs().ContainsMessage("Row=8");
        Logger.DebugLogs().ContainsMessage("Column=7");
    }
}