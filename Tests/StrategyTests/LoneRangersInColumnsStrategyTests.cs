using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class LoneRangersInColumnsStrategyTests
{
	[Theory]
	[InlineData(Level.Easy, 2, 6, 5)]
	[InlineData(Level.Medium, 0, 2, 8)]
	[InlineData(Level.Hard, 0, 1, 4)]
	[InlineData(Level.ExtremelyHard, 6, 7, 3)]
	public void SolvePuzzle_WhenPossibleNumberOccursOnlyOnceInColumn_SetValueToThatNumber(Level level, int col, int row, int expectedValue)
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(level);
		var sut = new LoneRangersInColumnsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.Values[col, row].Should().Be(expectedValue);
	}
}