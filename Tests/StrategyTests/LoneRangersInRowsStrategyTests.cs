using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class LoneRangersInRowsStrategyTests
{
	[Fact]
	public void SolvePuzzle_WhenPossibleNumberOccursOnlyOnceInRow_SetValueToThatNumber()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = new LoneRangersInRowsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.Values[6, 0]
			.Should().Be(int.Parse(puzzle.PossibleValues[6, 0]))
			.And.Subject?.Should().Be(9);
	}
}