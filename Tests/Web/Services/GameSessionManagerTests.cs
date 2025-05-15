using DepenMock.XUnit;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Services;

public class GameSessionManagerTests : BaseTestByAbstraction<GameSessionManager, IGameSessionManager>
{
    private readonly Mock<IGameTimer> _mockTimer;
    private readonly Mock<IGameStateManager> _mockGameStateManager;

    public GameSessionManagerTests()
    {
        _mockTimer = Container.ResolveMock<IGameTimer>();
        _mockGameStateManager = Container.ResolveMock<IGameStateManager>();
    }

    [Fact]
    public async Task StartNewSession_ShouldInitializeNewSession()
    {
        // Arrange
        var puzzleId = "puzzle1";
        var gameState = new GameStateMemory(puzzleId, new Cell[81]);
        var sut = ResolveSut();

        // Act
        await sut.StartNewSession(gameState);

        // Assert
        sut.CurrentSession.VerifyNewSession(puzzleId);
    }

    [Fact]
    public async Task StartNewSession_ShouldStartTimer()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.StartNewSession(Container.Create<GameStateMemory>());

        // Assert
        _mockTimer.VerifyStarted(Times.Once);
    }

    [Fact]
    public async Task PauseSession_ShouldPauseTimer()
    {
        // Arrange
        var sut = ResolveSut();
        await sut.StartNewSession(Container.Create<GameStateMemory>());

        // Act
        await sut.PauseSession();

        // Assert
        _mockTimer.VerifyPaused(Times.Once);
    }

    [Fact]
    public async Task PauseSession_ShouldSaveSession()
    {
        // Arrange
        var sut = ResolveSut();
        await sut.StartNewSession(Container.Create<GameStateMemory>());
        _mockGameStateManager.Reset();

        // Act
        await sut.PauseSession();

        // Assert
        _mockGameStateManager.VerifySaveAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task ResumeSession_ShouldResumeTimer()
    {
        // Arrange
        var sut = ResolveSut();
        await sut.StartNewSession(Container.Create<GameStateMemory>());

        // Act
        sut.ResumeSession();

        // Assert
        _mockTimer.VerifyResumed(Times.Once);
    }

    [Fact]
    public async Task EndSession_ShouldPauseTimer()
    {
        // Arrange
        var sut = ResolveSut();
        await sut.StartNewSession(Container.Create<GameStateMemory>());

        // Act
        await sut.EndSession();

        // Assert
        _mockTimer.VerifyPaused(Times.Once);
    }

    [Fact]
    public async Task EndSession_ShouldSaveSession()
    {
        // Arrange
        var sut = ResolveSut();
        await sut.StartNewSession(Container.Create<GameStateMemory>());
        _mockGameStateManager.Reset();

        // Act
        await sut.EndSession();

        // Assert
        _mockGameStateManager.VerifySaveAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task EndSession_ShouldResetSession()
    {
        // Arrange
        var sut = ResolveSut();
        await sut.StartNewSession(Container.Create<GameStateMemory>());

        // Act
        await sut.EndSession();

        // Assert
        sut.CurrentSession.VerifySessionReset();
    }
    
    [Fact]
    public async Task RecordMove_ShouldSaveSession()
    {
        // Arrange
        var sut = ResolveSut();
        await sut.StartNewSession(Container.Create<GameStateMemory>());
        _mockGameStateManager.Reset();

        // Act
        await sut.RecordMove(true);

        // Assert
        _mockGameStateManager.VerifySaveAsyncCalled(Times.Once);
    }
    
    [Fact]
    public async Task RecordMove_CallsSessionRecordMove()
    {
        // Arrange
        var recordMoveCalled = false;
        var sut = ResolveSut();
        await sut.StartNewSession(Container.Create<GameStateMemory>());
        sut.CurrentSession.OnMoveRecorded += (sender, args) => recordMoveCalled = true;

        // Act
        await sut.RecordMove(true);

        // Assert
        recordMoveCalled.Should().BeTrue();
    }
}