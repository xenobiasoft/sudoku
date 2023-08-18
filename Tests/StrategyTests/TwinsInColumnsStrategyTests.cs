using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;

namespace UnitTests.StrategyTests;

public class TwinsInColumnsStrategyTests
{
	[Fact]
	public void SolvePuzzle_WhenTwoCellsHaveSamePossibleValuesWithLengthOfTwo_ThenSetAsTwins()
	{
		// Arrange
		var puzzle = new SudokuPuzzle
		{
			Values = new[,] {
				{
					6, 0, 9, 1, 0, 0, 0, 0, 0
				},
				{
					0, 8, 3, 0, 0, 0, 0, 0, 0
				},
				{
					7, 0, 1, 6, 0, 0, 0, 0, 0
				},
				{
					0, 7, 6, 5, 0, 0, 0, 0, 0
				},
				{
					0, 0, 4, 0, 0, 0, 0, 0, 0
				},
				{
					1, 0, 5, 7, 0, 0, 0, 0, 0
				},
				{
					0, 1, 2, 8, 0, 0, 0, 0, 0
				},
				{
					0, 6, 8, 9, 0, 0, 0, 0, 0
				},
				{
					0, 9, 7, 0, 0, 0, 0, 0, 0
				}
			}
		};
		var sut = new TwinsInColumnsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.PossibleValues[4, 1].Should().Be(puzzle.PossibleValues[4, 3]).And.Be("23");
	}
}