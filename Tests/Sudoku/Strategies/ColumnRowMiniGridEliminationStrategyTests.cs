using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Sudoku.Strategies;

public class ColumnRowMiniGridEliminationStrategyTests : BaseTestByAbstraction<ColumnRowMiniGridEliminationStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenOnlyOnePossibleValue_ThenValueEqualsThatNumber()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.GetCell(4, 4).Value.Should().Be(5);
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsExpectedScore()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var initialCellsWithValues = puzzle.GetAllCells().Count(x => x.Value.HasValue);
		var sut = ResolveSut();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		var expectedScore = puzzle.GetAllCells().Count(x => x.Value.HasValue) - initialCellsWithValues;
		score.Should().Be(expectedScore);
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsNotSet_ReturnsScoreOfZero()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.ExtremelyHard);
		var sut = ResolveSut();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		score.Should().Be(0);
	}
}