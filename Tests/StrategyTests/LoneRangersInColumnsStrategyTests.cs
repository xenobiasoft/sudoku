using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class LoneRangersInColumnsStrategyTests
{
	[Fact]
	public void SolvePuzzle_WhenPossibleNumberOccursOnlyOnceInColumn_SetValueToThatNumber()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = new LoneRangersInColumnsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.Values[2, 6]
			.Should().Be(int.Parse(puzzle.PossibleValues[2, 6]))
			.And.Subject?.Should().Be(5);
	}
}