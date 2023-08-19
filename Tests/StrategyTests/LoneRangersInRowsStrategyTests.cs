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
		var sut = GetStrategyInstance();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.Values[6, 0]
			.Should().Be(int.Parse(puzzle.PossibleValues[6, 0]))
			.And.Subject?.Should().Be(9);
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsScoreGreaterThanZero()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = GetStrategyInstance();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		score.Should().Be(2);
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsNotSet_ReturnsScoreOfZero()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetEmptyPuzzle();
		var sut = GetStrategyInstance();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		score.Should().Be(0);
	}

	private static LoneRangersInRowsStrategy GetStrategyInstance()
	{
		return new LoneRangersInRowsStrategy();
	}
}