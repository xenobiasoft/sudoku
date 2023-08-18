using UnitTests.Helpers;
using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;

namespace UnitTests.StrategyTests;

public class LoneRangersInMiniGridStrategyTests
{
	[Theory]
	[InlineData(Level.Easy, 8, 6, 1)]
	[InlineData(Level.Medium, 0, 3, 3)]
	[InlineData(Level.Hard, 7, 7, 1)]
	[InlineData(Level.ExtremelyHard, 8, 2, 3)]
	public void SolvePuzzle_WhenPossibleNumberOccursOnlyOnceInMiniGrid_SetValueToThatNumber(Level level, int col, int row, int expectedValue)
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(level);
		var sut = new LoneRangersInMiniGridsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.Values[col, row].Should().Be(expectedValue);
	}
}