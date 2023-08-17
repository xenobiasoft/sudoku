using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class ColumnRowMiniGridEliminationStrategyTests
{
	[Theory]
	[InlineData(Level.Easy, 0, 3, 2)]
	[InlineData(Level.Medium, 5, 5, 5)]
	[InlineData(Level.Hard, 4, 3, 9)]
	public void SolvePuzzle_WhenOnlyOnePossibleValue_ThenValueEqualsThatNumber(Level level, int col, int row, int expectedValue)
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(level);
		var sut = new ColumnRowMiniGridEliminationStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.Values[col, row].Should().Be(expectedValue);
	}
}