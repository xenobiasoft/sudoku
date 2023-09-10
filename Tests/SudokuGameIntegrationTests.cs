using UnitTests.Helpers;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.PuzzleSolver;
using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;

namespace UnitTests;

public class SudokuGameIntegrationTests
{
	[Fact]
	public void SudokuGame_WhenSolvingPuzzle_ReturnsPuzzleCompletelySolved()
	{
		// Arrange
		var puzzleSolver = new PuzzleSolver(GetStrategies());
		var sut = new SudokuGame(new GameStateMemory(), puzzleSolver);
		var puzzle = PuzzleFactory.GetPuzzle(Level.ExtremelyHard);
		sut.LoadPuzzle(puzzle);

		// Act
		sut.SolvePuzzle();

		// Assert
		puzzleSolver.IsSolved(sut.Puzzle).Should().BeTrue();
	}

	[Theory]
	[InlineData(Level.Easy, 40, 45)]
	[InlineData(Level.Medium, 46, 49)]
	[InlineData(Level.Hard, 50, 53)]
	[InlineData(Level.ExtremelyHard, 54, 58)]
	public void SudokuGame_WhenSolvingPuzzle_ReturnsExpectedScoreForEachLevel(Level level, int minExpectedScore, int maxExpectedScore)
	{
		// Arrange
		var puzzleSolver = new PuzzleSolver(GetStrategies());
		var sut = new SudokuGame(new GameStateMemory(), puzzleSolver);
		var puzzle = PuzzleFactory.GetPuzzle(level);
		sut.LoadPuzzle(puzzle);

		// Act
		sut.SolvePuzzle();

		// Assert
		sut.Score.Should().BeInRange(minExpectedScore, maxExpectedScore);
	}

	private IEnumerable<SolverStrategy> GetStrategies()
	{
		return new List<SolverStrategy>
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
	}
}