using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class TwinsInRowsStrategyTests
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
		var sut = new TwinsInRowsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.PossibleValues[0, 1].Should().Be(puzzle.PossibleValues[2, 1]).And.Be("45");
	}
}