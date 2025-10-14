using DepenMock.XUnit;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.Abstractions.V2;
using Sudoku.Web.Server.Services.HttpClients;
using Sudoku.Web.Server.Services.V2;

namespace UnitTests.Web.Services.V2;

public class GameStatisticsManagerTests : BaseTestByAbstraction<GameManager, IGameStatisticsManager>
{
    private readonly Mock<IGameApiClient> _mockGameApiClient;
    private readonly Mock<IGameTimer> _mockGameTimer;
    private readonly GameModel _testGame;

    public GameStatisticsManagerTests()
    {
        _mockGameApiClient = Container.ResolveMock<IGameApiClient>();
        _mockGameTimer = Container.ResolveMock<IGameTimer>();

        _testGame = new GameModel
        {
            Id = "test-game-id",
            PlayerAlias = "test-player",
            Difficulty = "Easy",
            Status = "InProgress",
            Statistics = new GameStatisticsModel()
        };
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
    public async Task EndSession_SavesGame()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.EndSession();

        // Assert
        _mockGameApiClient.Verify(x => x.SaveGameAsync(_testGame), Times.Once);
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
    public async Task PauseSession_SavesGame()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.PauseSession();

        // Assert
        _mockGameApiClient.Verify(x => x.SaveGameAsync(_testGame), Times.Once);
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
        await sut.RecordMove(true);

        // Assert
        _testGame.Statistics.TotalMoves.Should().Be(initialTotalMoves + 1);
        _testGame.Statistics.InvalidMoves.Should().Be(initialInvalidMoves);
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
        await sut.RecordMove(false);

        // Assert
        _testGame.Statistics.TotalMoves.Should().Be(initialTotalMoves + 1);
        _testGame.Statistics.InvalidMoves.Should().Be(initialInvalidMoves + 1);
    }

    [Fact]
    public async Task RecordMove_ValidMove_SavesGame()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.RecordMove(true);

        // Assert
        _mockGameApiClient.Verify(x => x.SaveGameAsync(_testGame), Times.Once);
    }

    [Fact]
    public async Task RecordMove_InvalidMove_SavesGame()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.RecordMove(false);

        // Assert
        _mockGameApiClient.Verify(x => x.SaveGameAsync(_testGame), Times.Once);
    }

    [Fact]
    public async Task RecordMove_MultipleValidMoves_IncrementsCountCorrectly()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        var initialTotalMoves = _testGame.Statistics.TotalMoves;

        // Act
        await sut.RecordMove(true);
        await sut.RecordMove(true);
        await sut.RecordMove(true);

        // Assert
        _testGame.Statistics.TotalMoves.Should().Be(initialTotalMoves + 3);
        _testGame.Statistics.InvalidMoves.Should().Be(0);
        _testGame.Statistics.ValidMoves.Should().Be(initialTotalMoves + 3);
    }

    [Fact]
    public async Task RecordMove_MultipleInvalidMoves_IncrementsCountCorrectly()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        var initialTotalMoves = _testGame.Statistics.TotalMoves;

        // Act
        await sut.RecordMove(false);
        await sut.RecordMove(false);

        // Assert
        _testGame.Statistics.TotalMoves.Should().Be(initialTotalMoves + 2);
        _testGame.Statistics.InvalidMoves.Should().Be(2);
        _testGame.Statistics.ValidMoves.Should().Be(initialTotalMoves);
    }

    [Fact]
    public async Task RecordMove_MixedValidAndInvalidMoves_CalculatesCorrectly()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);
        var initialTotalMoves = _testGame.Statistics.TotalMoves;

        // Act
        await sut.RecordMove(true);
        await sut.RecordMove(false);
        await sut.RecordMove(true);
        await sut.RecordMove(false);

        // Assert
        _testGame.Statistics.TotalMoves.Should().Be(initialTotalMoves + 4);
        _testGame.Statistics.InvalidMoves.Should().Be(2);
        _testGame.Statistics.ValidMoves.Should().Be(initialTotalMoves + 2);
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
    public async Task StartNewSession_SavesGame()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, _testGame);

        // Act
        await sut.StartNewSession();

        // Assert
        _mockGameApiClient.Verify(x => x.SaveGameAsync(_testGame), Times.Once);
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