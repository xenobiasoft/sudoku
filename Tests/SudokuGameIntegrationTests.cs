using UnitTests.Helpers;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Solver;
using XenobiaSoft.Sudoku.Generator;

namespace UnitTests;

public class SudokuGameIntegrationTests
{
	[Fact]
	public void SudokuGame_WhenSolvingPuzzle_ReturnsPuzzleCompletelySolved()
	{
		// Arrange
		var puzzleSolver = new PuzzleSolver(GetStrategies());
		var puzzleGenerator = new PuzzleGenerator();
		var sut = new SudokuGame(new GameStateMemory(), puzzleSolver, puzzleGenerator);
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		sut.LoadPuzzle(puzzle);

		// Act
		sut.SolvePuzzle();

		// Assert
		puzzleSolver.IsSolved(sut.Puzzle).Should().BeTrue();
	}

	[Theory]
	[InlineData(Level.Easy, 90, 98)]
	[InlineData(Level.Medium, 99, 108)]
	[InlineData(Level.Hard, 109, 115)]
	[InlineData(Level.ExtremelyHard, 116, 135)]
	public void SudokuGame_WhenSolvingPuzzle_ReturnsExpectedScoreForEachLevel(Level level, int minExpectedScore, int maxExpectedScore)
	{
		// Arrange
		var puzzleSolver = new PuzzleSolver(GetStrategies());
		var puzzleGenerator = new PuzzleGenerator();
		var sut = new SudokuGame(new GameStateMemory(), puzzleSolver, puzzleGenerator);
		var puzzle = PuzzleFactory.GetPuzzle(level);
		sut.LoadPuzzle(puzzle);

		// Act
		sut.SolvePuzzle();

		// Assert
		sut.Score.Should().BeInRange(minExpectedScore, maxExpectedScore);
	}
	
	[Fact]
	public async Task SudokuGame_GenerateNewPuzzle()
	{
		// Arrange
		var puzzleSolver = new PuzzleSolver(GetStrategies());
		var puzzleGenerator = new PuzzleGenerator();
		var sut = new SudokuGame(new GameStateMemory(), puzzleSolver, puzzleGenerator);

		// Act
		await sut.New(Level.Easy);

		// Assert
		puzzleSolver.IsSolved(sut.Puzzle).Should().BeTrue();
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