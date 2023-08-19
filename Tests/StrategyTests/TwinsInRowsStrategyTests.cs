using UnitTests.Helpers;
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
		var sut = GetStrategyInstance();

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
		var sut = GetStrategyInstance();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var col = 0; col < 9; col++)
		{
			if (col is 0 or 2) continue;

			puzzle.PossibleValues[col, 1].Should().NotContain("4").And.NotContain("5");
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
					1, 0, 2, 3, 6, 7, 8, 9, 0
				},
				{
					0, 0, 0, 0, 0, 0, 0, 0, 0
				},
				{
					2, 0, 3, 6, 7, 8, 9, 1, 0
				},
				{
					3, 0, 6, 7, 8, 9, 1, 0, 0
				},
				{
					0, 0, 0, 0, 0, 0, 0, 0, 0
				},
				{
					0, 0, 0, 0, 0, 0, 0, 0, 0
				},
				{
					0, 0, 0, 0, 0, 0, 0, 0, 0
				},
				{
					0, 0, 0, 0, 0, 0, 0, 0, 0
				},
				{
					0, 0, 0, 0, 0, 0, 0, 0, 0
				}
			}
		};
	}

	private static TwinsInRowsStrategy GetStrategyInstance()
	{
		return new TwinsInRowsStrategy();
	}
}