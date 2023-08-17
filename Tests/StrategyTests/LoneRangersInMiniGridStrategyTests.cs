using UnitTests.Helpers;
using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;

namespace UnitTests.StrategyTests;

public class LoneRangersInMiniGridStrategyTests
{
	[Theory]
	[InlineData(Level.Easy, 5, 5, 1)]
	[InlineData(Level.Medium, 5, 1, 3)]
	[InlineData(Level.Hard, 0, 5, 1)]
	[InlineData(Level.ExtremelyHard, 6, 1, 1)]
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