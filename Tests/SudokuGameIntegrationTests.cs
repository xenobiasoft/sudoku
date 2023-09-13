using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Generator;
using XenobiaSoft.Sudoku.Solver;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests;

public class SudokuGameIntegrationTests
{
	//[Fact]
	public async Task SudokuGame_WhenSolvingPuzzle_ReturnsPuzzleCompletelySolved()
	{
		// Arrange
		var sut = GetSudokuGame();
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		await sut.LoadPuzzle(puzzle);

		// Act
		await sut.SolvePuzzle();

		// Assert
		sut.Puzzle.IsSolved().Should().BeTrue();
	}

	[Fact]
	public async Task New_ReturnsSolvedPuzzle()
	{
		// Arrange
		var sut = GetSudokuGame();

		// Act
		await sut.New(Level.Easy);

		// Assert
		sut.Puzzle.Count(x => !x.Value.HasValue).Should().BeGreaterOrEqualTo(40).And.BeLessOrEqualTo(45);
	}

	private SudokuGame GetSudokuGame()
	{
		var solverStrategies = (IEnumerable<SolverStrategy>)new List<SolverStrategy>
		{
			new ColumnRowMiniGridEliminationStrategy(),
			new LoneRangersInMiniGridsStrategy(),
			new LoneRangersInRowsStrategy(),
			new LoneRangersInColumnsStrategy(),
			new TwinsInMiniGridsStrategy(),
			new TwinsInRowsStrategy(),
			new TwinsInColumnsStrategy(),
			new TripletsInMiniGridsStrategy(),
			new TripletsInRowsStrategy(),
			new TripletsInColumnsStrategy(),
		};
		var puzzleSolver = new PuzzleSolver(solverStrategies, new GameStateMemory());
		var puzzleGenerator = new PuzzleGenerator(puzzleSolver);
		var sut = new SudokuGame(puzzleSolver, puzzleGenerator);

		return sut;
	}
}