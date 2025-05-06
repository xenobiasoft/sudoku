using DepenMock.XUnit;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Solver;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Sudoku.Solver;

public class PuzzleSolverTests : BaseTestByAbstraction<PuzzleSolver, IPuzzleSolver>
{
    [Fact]
    public async Task SolvePuzzle_ShouldUndoOnInvalidMove()
    {
        // Arrange
        var mockGameStateMemory = Container.ResolveMock<IGameStateStorage>();
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.SetupInvalidMove();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(mockPuzzle.Object);

        // Assert
        mockGameStateMemory.VerifyUndoAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task SolvePuzzle_ShouldUseBruteForceWhenStrategiesFail()
    {
        // Arrange
        Container
            .ResolveMock<SolverStrategy>()
            .DoesNotMakeChanges();
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(mockPuzzle.Object);

        // Assert
        mockPuzzle.VerifyUsedBruteForce();
    }

    [Fact]
    public async Task SolvePuzzle_SavesGameState_OnEachLoop()
    {
        // Arrange
        var mockGameStateMemory = Container.ResolveMock<IGameStateStorage>();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(PuzzleFactory.GetSolvedPuzzle());

        // Assert
        mockGameStateMemory.VerifySaveAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task SolvePuzzle_WhenStrategyMakesInvalidMove_GamePopsSavedStateOffStack()
    {
        // Arrange
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.SetupInvalidMove();
        var mockGameStateMemory = Container.ResolveMock<IGameStateStorage>();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(mockPuzzle.Object);

        // Assert
        mockGameStateMemory.VerifyUndoAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task SolvePuzzle_WhenPuzzleIsSolved_ClearsMemoryState()
    {
        // Arrange
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.SetupPuzzleIsSolved();
        var mockGameStateMemory = Container.ResolveMock<IGameStateStorage>();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(PuzzleFactory.GetSolvedPuzzle());

        // Assert
        mockGameStateMemory.VerifyDeleteAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task SolvePuzzle_WhenGameStateIsEmpty_AndUndoCalled_ThrowsInvalidBoardException()
    {
        // Arrange
        Container.ResolveMock<IGameStateStorage>().SetupEmptyStack();
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.SetupInvalidMove();
        var sut = ResolveSut();

        // Act
        Task SolvePuzzle() => sut.SolvePuzzle(mockPuzzle.Object);

        // Assert
        await Assert.ThrowsAsync<InvalidBoardException>(SolvePuzzle);
    }
}