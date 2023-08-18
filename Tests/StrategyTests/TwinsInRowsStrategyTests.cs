using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class TwinsInRowsStrategyTests
{
	[Fact]
	public void SolvePuzzle_WhenTwoCellsHaveSamePossibleValuesWithLengthOfTwoInRow_ThenSetAsTwins()
	{
		// Arrange
		var puzzle = GetTwinsPuzzle();
		var sut = new TwinsInRowsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.PossibleValues[0, 1].Should().Be(puzzle.PossibleValues[2, 1]).And.Be("45");
	}

	[Fact]
	public void SolvePuzzle_WhenTwinsAreFoundInRow_RemovesTwinsNumbersFromOtherCells()
	{
		// Arrange
		var puzzle = GetTwinsPuzzle();
		var sut = new TwinsInRowsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var col = 0; col < 9; col++)
		{
			if (col is 0 or 2) continue;

			puzzle.PossibleValues[col, 1].Should().NotContain("4").And.NotContain("5");
		}
	}

	private SudokuPuzzle GetTwinsPuzzle()
	{
		return new SudokuPuzzle
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
	}
}