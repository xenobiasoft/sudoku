using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class TwinsInMiniGridsStrategyTests
{
	[Fact]
	public void SolvePuzzle_WhenTwoCellsHaveSamePossibleValuesWithLengthOfTwoInMiniGrid_ThenSetAsTwins()
	{
		// Arrange
		var puzzle = GetTwinsPuzzle();
		var sut = GetStrategyInstance();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.PossibleValues[4, 1].Should().Be(puzzle.PossibleValues[5, 1]).And.Be("23");
	}

	[Fact]
	public void SolvePuzzle_WhenTwinsAreFoundInMiniGrid_RemovesTwinsNumbersFromOtherCells()
	{
		// Arrange
		var puzzle = GetTwinsPuzzle();
		var sut = GetStrategyInstance();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var col = 3; col < 6; col++)
		{
			for (var row = 0; row < 3; row++)
			{
				if (row is 1 && col is 4 or 5) continue;

				puzzle.PossibleValues[col, row].Should().NotContain("2").And.NotContain("3");
			}
		}
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsScoreGreaterThanZero()
	{
		// Arrange
		var puzzle = GetTwinsPuzzle();
		var sut = GetStrategyInstance();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		score.Should().Be(3);
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
					8, 0, 4, 0, 0, 0, 0, 0, 0
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

	private static TwinsInMiniGridsStrategy GetStrategyInstance()
	{
		return new TwinsInMiniGridsStrategy();
	}
}