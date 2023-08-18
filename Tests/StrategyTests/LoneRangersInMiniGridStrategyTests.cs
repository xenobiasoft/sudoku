using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class LoneRangersInMiniGridStrategyTests
{
	[Fact]
	public void SolvePuzzle_WhenPossibleNumberOccursOnlyOnceInMiniGrid_SetValueToThatNumber()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = new LoneRangersInMiniGridsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.Values[8, 6]
			.Should().Be(int.Parse(puzzle.PossibleValues[8, 6]))
			.And.Subject?.Should().Be(1);
	}
}