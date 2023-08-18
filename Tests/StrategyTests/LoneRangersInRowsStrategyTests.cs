using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;
namespace UnitTests.StrategyTests;

public class LoneRangersInRowsStrategyTests
{
	[Theory]
	[InlineData(Level.Easy, 6, 0, 9)]
	[InlineData(Level.Medium, 0, 2, 8)]
	[InlineData(Level.Hard, 5, 1, 8)]
	public void SolvePuzzle_WhenPossibleNumberOccursOnlyOnceInRow_SetValueToThatNumber(Level level, int col, int row, int expectedValue)
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(level);
		var sut = new LoneRangersInRowsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.Values[col, row].Should().Be(expectedValue);
	}
}