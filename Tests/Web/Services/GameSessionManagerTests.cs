using DepenMock.XUnit;
using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
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
    public async Task ResumeSession_ShouldReloadGameState()
    {
        // Arrange
        var initialGameState = new GameStateMemory
        {
            PuzzleId = "puzzle-id",
            Board = PuzzleFactory.GetPuzzle(Level.Easy).GetAllCells()
        };
        var sut = ResolveSut();
        await sut.StartNewSession(Container.Create<GameStateMemory>());
        await sut.PauseSession();

        // Act
        await sut.ResumeSession(initialGameState);

        // Assert
        sut.CurrentSession.VerifyGameSessionReloaded(initialGameState);
    }

    [Fact]
    public async Task ResumeSession_ShouldResumeTimer()
    {
        // Arrange
        var gameStateMemory = Container.Create<GameStateMemory>();
        var sut = ResolveSut();
        await sut.StartNewSession(gameStateMemory);
        await sut.PauseSession();

        // Act
        await sut.ResumeSession(gameStateMemory);

        // Assert
        _mockTimer.VerifyResumed(Times.Once);
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

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RecordMove_CallsSessionRecordMove(bool isValidMove)
    {
        // Arrange
        var gameState = Container.Create<GameStateMemory>();
        var expectedInvalidMoves = isValidMove ? gameState.InvalidMoves : gameState.InvalidMoves + 1;
        var expectedTotalMoves = gameState.TotalMoves + 1;
        var sut = ResolveSut();
        await sut.StartNewSession(gameState);

        // Act
        await sut.RecordMove(isValidMove);

        // Assert
        sut.CurrentSession.VerifyRecordMoveCalled(expectedInvalidMoves, expectedTotalMoves);
    }

    [Fact]
    public async Task StartNewSession_ShouldInitializeNewSession()
    {
        // Arrange
        var puzzleId = "puzzle1";
        var gameState = Container.Build<GameStateMemory>()
            .With(g => g.PuzzleId, puzzleId)
            .With(g => g.Board, PuzzleFactory.GetPuzzle(Level.Easy).GetAllCells())
            .Create();
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
}