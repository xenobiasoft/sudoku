using DepenMock.XUnit;
using Sudoku.Domain.ValueObjects;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.HttpClients;
using Sudoku.Web.Server.Services.V2;
using UnitTests.Helpers.Factories;

namespace UnitTests.Web.Services.V2;

public class GameStatisticsManagerTests : BaseTestByAbstraction<GameManager, IGameStatisticsManager>
{
    private readonly Mock<IGameApiClient> _mockGameApiClient;
    private readonly Mock<IGameTimer> _mockGameTimer;
    private readonly GameModel _testGame;

    private readonly string _testAlias;
    private readonly string _testGameId;

    public GameStatisticsManagerTests()
    {
        _mockGameApiClient = Container.ResolveMock<IGameApiClient>();
        _mockGameTimer = Container.ResolveMock<IGameTimer>();

        _testAlias = Container.Create<string>();
        _testGameId = Container.Create<string>();
        _testGame = GameModelFactory
            .Build()
            .WithStatus(GameStatus.NotStarted)
            .WithPlayerAlias(_testAlias)
            .WithId(_testGameId)
            .Create();

        _mockGameApiClient
            .Setup(x => x.GetGameAsync(_testAlias, _testGameId))
            .ReturnsAsync(ApiResult<GameModel>.Success(_testGame));
    }

    [Fact]
    public void CurrentStatistics_WhenGameIsLoaded_ReturnsGameStatistics()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        var result = sut.CurrentStatistics;

        // Assert
        result.Should().BeSameAs(_testGame.Statistics);
    }

    [Fact]
    public async Task EndSession_ResetsTimer()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.EndSession();

        // Assert
        _mockGameTimer.Verify(x => x.Reset(), Times.Once);
    }

    [Fact]
    public async Task EndSession_WhenGameIsSolved_SetsGameStatusToCompleted()
    {
        // Arrange
        var game = GameModelFactory.GetSolvedPuzzle();
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        await sut.EndSession();

        // Assert
        sut.Game.Status.Should().Be(GameStatus.Completed);
    }

    [Fact]
    public async Task EndSession_WhenGameCompleted_SavesGameStatus()
    {
        // Arrange
        var game = GameModelFactory.GetSolvedPuzzle();
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        await sut.EndSession();

        // Assert
        _mockGameApiClient.VerifySavesGameStatus(game.PlayerAlias, game.Id, GameStatus.Completed, Times.Once);
    }

    [Fact]
    public async Task EndSession_WhenGameNotComplete_SetsGameStatusToAbandoned()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.EndSession();

        // Assert
        sut.Game.Status.Should().Be(GameStatus.Abandoned);
    }

    [Fact]
    public async Task EndSession_WhenGameNotComplete_SavesGameStatus()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.EndSession();

        // Assert
        _mockGameApiClient.VerifySavesGameStatus(_testAlias, _testGameId, GameStatus.Abandoned, Times.Once);
    }

    [Fact]
    public async Task EndSession_UnsubscribesFromTimerTick()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.EndSession();

        // Assert
        _mockGameTimer.VerifyRemove(x => x.OnTick -= It.IsAny<EventHandler<TimeSpan>>(), Times.Once);
    }

    [Fact]
    public async Task OnTimerTick_UpdatesPlayDuration()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        await sut.StartNewSession();
        var expectedElapsedTime = TimeSpan.FromMinutes(5);

        // Act
        _mockGameTimer.Raise(x => x.OnTick += null, sut, expectedElapsedTime);

        // Assert
        _testGame.Statistics.PlayDuration.Should().Be(expectedElapsedTime);
    }

    [Fact]
    public async Task OnTimerTick_WithLargeElapsedTime_UpdatesPlayDuration()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        await sut.StartNewSession();
        var expectedElapsedTime = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(30));

        // Act
        _mockGameTimer.Raise(x => x.OnTick += null, sut, expectedElapsedTime);

        // Assert
        _testGame.Statistics.PlayDuration.Should().Be(expectedElapsedTime);
    }

    [Fact]
    public async Task OnTimerTick_WithZeroElapsedTime_UpdatesPlayDurationToZero()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        await sut.StartNewSession();
        _testGame.Statistics.SetPlayDuration(TimeSpan.FromMinutes(5)); // Set initial duration
        var expectedElapsedTime = TimeSpan.Zero;

        // Act
        _mockGameTimer.Raise(x => x.OnTick += null, sut, expectedElapsedTime);

        // Assert
        _testGame.Statistics.PlayDuration.Should().Be(expectedElapsedTime);
    }

    [Fact]
    public async Task PauseSession_SetsGameStatusToPaused()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.PauseSession();

        // Assert
        sut.Game.Status.Should().Be(GameStatus.Paused);
    }

    [Fact]
    public async Task PauseSession_PausesTimer()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.PauseSession();

        // Assert
        _mockGameTimer.Verify(x => x.Pause(), Times.Once);
    }

    [Fact]
    public async Task PauseSession_SavesGameStatus()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.PauseSession();

        // Assert
        _mockGameApiClient.VerifySavesGameStatus(_testAlias, _testGameId, GameStatus.Paused, Times.Once);
    }

    [Fact]
    public async Task RecordMove_InvalidMove_RecordsInvalidMove()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        var initialTotalMoves = _testGame.Statistics.TotalMoves;
        var initialInvalidMoves = _testGame.Statistics.InvalidMoves;

        // Act
        await sut.RecordMove(Container.Create<int>(), Container.Create<int>(), Container.Create<int>(), false);

        // Assert
        _testGame.Statistics.TotalMoves.Should().Be(initialTotalMoves + 1);
        _testGame.Statistics.InvalidMoves.Should().Be(initialInvalidMoves + 1);
    }

    [Fact]
    public async Task RecordMove_InvalidMove_SavesGame()
    {
        // Arrange
        var row = Container.Create<int>();
        var column = Container.Create<int>();
        var value = Container.Create<int>();
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.RecordMove(row, column, value, false);

        // Assert
        _mockGameApiClient.VerifyMakesMove(_testAlias, _testGameId, row, column, value, Times.Once);
    }

    [Fact]
    public async Task RecordMove_MixedValidAndInvalidMoves_CalculatesCorrectly()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        var initialTotalMoves = sut.CurrentStatistics.TotalMoves;

        // Act
        await sut.RecordMove(Container.Create<int>(), Container.Create<int>(), Container.Create<int>(), true);
        await sut.RecordMove(Container.Create<int>(), Container.Create<int>(), Container.Create<int>(), false);
        await sut.RecordMove(Container.Create<int>(), Container.Create<int>(), Container.Create<int>(), true);
        await sut.RecordMove(Container.Create<int>(), Container.Create<int>(), Container.Create<int>(), false);

        // Assert
        sut.CurrentStatistics.TotalMoves.Should().Be(initialTotalMoves + 4);
        sut.CurrentStatistics.InvalidMoves.Should().Be(2);
        sut.CurrentStatistics.ValidMoves.Should().Be(initialTotalMoves + 2);
    }

    [Fact]
    public async Task RecordMove_MultipleInvalidMoves_IncrementsCountCorrectly()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        var initialTotalMoves = _testGame.Statistics.TotalMoves;

        // Act
        await sut.RecordMove(Container.Create<int>(), Container.Create<int>(), Container.Create<int>(), false);
        await sut.RecordMove(Container.Create<int>(), Container.Create<int>(), Container.Create<int>(), false);

        // Assert
        _testGame.Statistics.TotalMoves.Should().Be(initialTotalMoves + 2);
        _testGame.Statistics.InvalidMoves.Should().Be(2);
        _testGame.Statistics.ValidMoves.Should().Be(initialTotalMoves);
    }

    [Fact]
    public async Task RecordMove_MultipleValidMoves_IncrementsCountCorrectly()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        var initialTotalMoves = _testGame.Statistics.TotalMoves;

        // Act
        await sut.RecordMove(Container.Create<int>(), Container.Create<int>(), Container.Create<int>(), true);
        await sut.RecordMove(Container.Create<int>(), Container.Create<int>(), Container.Create<int>(), true);
        await sut.RecordMove(Container.Create<int>(), Container.Create<int>(), Container.Create<int>(), true);

        // Assert
        _testGame.Statistics.TotalMoves.Should().Be(initialTotalMoves + 3);
        _testGame.Statistics.InvalidMoves.Should().Be(0);
        _testGame.Statistics.ValidMoves.Should().Be(initialTotalMoves + 3);
    }

    [Fact]
    public async Task RecordMove_ValidMove_RecordsValidMove()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        var initialTotalMoves = _testGame.Statistics.TotalMoves;
        var initialInvalidMoves = _testGame.Statistics.InvalidMoves;

        // Act
        await sut.RecordMove(Container.Create<int>(), Container.Create<int>(), Container.Create<int>(), true);

        // Assert
        _testGame.Statistics.TotalMoves.Should().Be(initialTotalMoves + 1);
        _testGame.Statistics.InvalidMoves.Should().Be(initialInvalidMoves);
    }

    [Fact]
    public async Task RecordMove_ValidMove_SavesGame()
    {
        // Arrange
        var row = Container.Create<int>();
        var column = Container.Create<int>();
        var value = Container.Create<int>();
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.RecordMove(row, column, value, true);

        // Assert
        _mockGameApiClient.VerifyMakesMove(_testAlias, _testGameId, row, column, value, Times.Once);
    }

    [Fact]
    public async Task ResumeSession_ResumesTimerWithPlayDuration()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        var expectedDuration = TimeSpan.FromMinutes(10);
        _testGame.Statistics.SetPlayDuration(expectedDuration);

        // Act
        await sut.ResumeSession();

        // Assert
        _mockGameTimer.Verify(x => x.Resume(expectedDuration), Times.Once);
    }

    [Fact]
    public async Task ResumeSession_SavesGameStatus()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        _testGame.Statistics.SetPlayDuration(TimeSpan.Zero);

        // Act
        await sut.ResumeSession();

        // Assert
        _mockGameApiClient.VerifySavesGameStatus(_testAlias, _testGameId, GameStatus.InProgress, Times.Once);
    }

    [Fact]
    public async Task ResumeSession_SetsGameStatusToInProgress()
    {
        // Arrange
        var game = GameModelFactory
            .Build()
            .WithStatus(GameStatus.Paused)
            .Create();
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        await sut.ResumeSession();

        // Assert
        sut.Game.Status.Should().Be(GameStatus.InProgress);
    }

    [Fact]
    public async Task ResumeSession_WithZeroPlayDuration_ResumesTimerWithZeroDuration()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        _testGame.Statistics.SetPlayDuration(TimeSpan.Zero);

        // Act
        await sut.ResumeSession();

        // Assert
        _mockGameTimer.Verify(x => x.Resume(TimeSpan.Zero), Times.Once);
    }

    [Fact]
    public async Task StartNewSession_ResetsStatistics()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        _testGame.Statistics.RecordMove(true);
        _testGame.Statistics.RecordMove(false);

        // Act
        await sut.StartNewSession();

        // Assert
        _testGame.Statistics.TotalMoves.Should().Be(0);
        _testGame.Statistics.InvalidMoves.Should().Be(0);
        _testGame.Statistics.PlayDuration.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public async Task StartNewSession_ResetsTimer()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.StartNewSession();

        // Assert
        _mockGameTimer.Verify(x => x.Reset(), Times.Once);
    }

    [Fact]
    public async Task StartNewSession_SetsGameStatusToInProgress()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.StartNewSession();

        // Assert
        sut.Game.Status.Should().Be(GameStatus.InProgress);
    }

    [Fact]
    public async Task StartNewSession_StartsTimer()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.StartNewSession();

        // Assert
        _mockGameTimer.Verify(x => x.Start(), Times.Once);
    }

    [Fact]
    public async Task StartNewSession_SubscribesToTimerTick()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.StartNewSession();

        // Assert
        _mockGameTimer.VerifyAdd(x => x.OnTick += It.IsAny<EventHandler<TimeSpan>>(), Times.Once);
    }

    [Fact]
    public async Task StartNewSession_SavesGameStatus()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.StartNewSession();

        // Assert
        _mockGameApiClient.VerifySavesGameStatus(_testAlias, _testGameId, GameStatus.InProgress, Times.Once);
    }

    private void SetGameProperty(IGameStatisticsManager gameStatisticsManager, GameModel game)
    {
        // Use reflection to set the private Game property for testing
        // Cast to the concrete type to access the property
        if (gameStatisticsManager is GameManager gameManager)
        {
            var gameProperty = typeof(GameManager).GetProperty("Game", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            gameProperty?.SetValue(gameManager, game);
        }
    }
}