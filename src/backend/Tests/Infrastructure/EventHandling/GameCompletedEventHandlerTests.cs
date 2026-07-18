using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Models;
using Sudoku.Domain.Events;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.EventHandling;

namespace UnitTests.Infrastructure.EventHandling;

[LogOutput(LogOutputTiming.Always)]
public class GameCompletedEventHandlerTests : MoqBaseTestByAbstraction<GameCompletedEventHandler, IDomainEventHandler<GameCompletedEvent>>
{
    private readonly Mock<IGameCompletionRepository> _mockCompletionRepository;

    public GameCompletedEventHandlerTests()
    {
        _mockCompletionRepository = Container.ResolveMock<IGameCompletionRepository>().AsMoq();
    }

    [Fact]
    public async Task HandleAsync_WithValidEvent_UpsertsCompletionRecordFromTheEvent()
    {
        // Arrange
        var domainEvent = CreateEvent(GameDifficulty.Hard, TimeSpan.FromMinutes(12));
        var expected = new GameCompletion(
            domainEvent.GameId.ToString(),
            domainEvent.ProfileId.ToString(),
            "Hard",
            TimeSpan.FromMinutes(12),
            domainEvent.CompletedAt);
        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);

        // Assert
        _mockCompletionRepository.VerifyUpserted(expected, Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithSameEventTwice_UpsertsTheSameGameIdBothTimes()
    {
        // Arrange — the upsert is keyed by gameId, so a duplicate event cannot double-count.
        var domainEvent = CreateEvent(GameDifficulty.Easy, TimeSpan.FromMinutes(3));
        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);
        await sut.HandleAsync(domainEvent);

        // Assert
        _mockCompletionRepository.VerifyUpsertedGame(domainEvent.GameId.ToString(), () => Times.Exactly(2));
    }

    [Fact]
    public async Task HandleAsync_WithValidEvent_LogsInformation()
    {
        // Arrange
        var domainEvent = CreateEvent(GameDifficulty.Medium, TimeSpan.FromMinutes(7));
        var sut = ResolveSut();

        // Act
        await sut.HandleAsync(domainEvent);

        // Assert
        Logger.InformationLogs().ContainsMessage($"Recorded completion for game {domainEvent.GameId}");
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrows_PropagatesException()
    {
        // Arrange — the caller (CosmosDbGameRepository.SaveAsync) contains the fault; the
        // handler itself must not silently swallow a failed write.
        var domainEvent = CreateEvent(GameDifficulty.Expert, TimeSpan.FromMinutes(30));
        _mockCompletionRepository.SetupUpsertThrows(new InvalidOperationException("Cosmos is down"));
        var sut = ResolveSut();

        // Act
        var handleAsync = async () => await sut.HandleAsync(domainEvent);

        // Assert
        await handleAsync.Should().ThrowAsync<InvalidOperationException>();
    }

    private static GameCompletedEvent CreateEvent(GameDifficulty difficulty, TimeSpan playDuration)
    {
        var statistics = GameStatistics.Create();
        statistics.UpdatePlayDuration(playDuration);

        return new GameCompletedEvent(
            GameId.New(),
            ProfileId.New(),
            difficulty,
            statistics,
            DateTime.UtcNow,
            BoardSize.Nine);
    }
}
