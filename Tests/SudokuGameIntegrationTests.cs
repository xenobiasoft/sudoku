using UnitTests.Helpers;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.PuzzleSolver;
using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;

namespace UnitTests;

public class SudokuGameIntegrationTests
{
	private readonly SudokuGame _sut;
	private readonly PuzzleSolver _puzzleSolver;

	public SudokuGameIntegrationTests()
	{
		var strategies = new List<SolverStrategy>
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
		_puzzleSolver = new PuzzleSolver(strategies);
		_sut = new SudokuGame(new GameStateMemory(), _puzzleSolver);
		var puzzle = PuzzleFactory.GetPuzzle(Level.ExtremelyHard);
		_sut.Restore(puzzle);
	}

	[Fact]
	public void SudokuGame_WhenSolvingPuzzle_ReturnsPuzzleCompletelySolved()
	{
		// Arrange

		// Act
		_sut.SolvePuzzle();

		// Assert
		_puzzleSolver.IsSolved(_sut.Puzzle).Should().BeTrue();
	}

	[Theory]
	[InlineData(Level.Easy, 40, 45)]
	[InlineData(Level.Medium, 46, 49)]
	[InlineData(Level.Hard, 50, 53)]
	[InlineData(Level.ExtremelyHard, 54, 58)]
	public void SudokuGame_WhenSolvingPuzzle_ReturnsExpectedScoreForEachLevel(Level level, int minExpectedScore, int maxExpectedScore)
	{
		// Arrange

		// Act
		_sut.SolvePuzzle();

		// Assert
		_sut.Score.Should().BeInRange(minExpectedScore, maxExpectedScore);
	}
}