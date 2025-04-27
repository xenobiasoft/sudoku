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
    public async Task SolvePuzzle_ShouldUndoOnInvalidMoveException()
    {
        // Arrange
        var mockGameStateMemory = Container.ResolveMock<IGameStateMemory>();
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.ThrowsInvalidMoveThenPuzzleSolved();
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
	public async Task TrySolvePuzzle_SavesGameState_OnEachLoop()
    {
        // Arrange
        var mockGameStateMemory = Container.ResolveMock<IGameStateMemory>();
		var sut = ResolveSut();

		// Act
		await sut.SolvePuzzle(PuzzleFactory.GetSolvedPuzzle());

		// Assert
        mockGameStateMemory.VerifySaveAsyncCalled(Times.Once);
    }

    [Fact]
	public async Task TrySolvePuzzle_WhenStrategyThrowsInvalidMoveException_GamePopsSavedStateOffStack()
    {
        // Arrange
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.ThrowsInvalidMoveThenPuzzleSolved();
		var mockGameStateMemory = Container.ResolveMock<IGameStateMemory>();
		var sut = ResolveSut();

		// Act
		await sut.SolvePuzzle(mockPuzzle.Object);

		// Assert
        mockGameStateMemory.VerifyUndoAsyncCalled(Times.Once);
    }
}