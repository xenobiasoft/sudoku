using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;
using UnitTests.Helpers;

namespace UnitTests.StrategyTests;

public class TripletsInMiniGridsStrategyTests
{
	[Fact]
	public void SolvePuzzle_WhenThreeCellsHavePossibleValuesWithLengthOfThreeInMiniGrid_ThenSetAsTriplets()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = GetStrategyInstance();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.PossibleValues[0, 0]
			.Should().Be(puzzle.PossibleValues[1, 1])
			.And.Be(puzzle.PossibleValues[2, 2])
			.And.Be("123");
	}

	[Fact]
	public void SolvePuzzle_WhenTripletsAreFoundInMiniGrid_RemovesTripletsPossibleValuesFromOtherCells()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = GetStrategyInstance();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var col = 0; col < 3; col++)
		{
			for (var row = 0; row < 3; row++)
			{
				if ((col == 0 && row == 0) || (col == 1 && row == 1) || (col == 2 && row == 2)) continue;

				puzzle.PossibleValues[col, row]
					.Should().NotContain("1")
					.And.NotContain("2")
					.And.NotContain("3");
			}
		}
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsScoreGreaterThanZero()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = GetStrategyInstance();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		score.Should().Be(4);
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

	private SudokuPuzzle GetTripletsPuzzle()
	{
		return new SudokuPuzzle
		{
			Values = new[,] {
				{
					0, 4, 5, 6, 7, 8, 9, 0, 0
				},
				{
					9, 0, 8, 7, 6, 5, 4, 0, 0
				},
				{
					8, 7, 0, 6, 5, 4, 0, 0, 0
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
					0, 0, 0, 0, 0, 0, 3, 2, 1
				},
				{
					0, 0, 0, 0, 0, 0, 2, 1, 3
				},
				{
					0, 0, 0, 0, 0, 0, 1, 3, 2
				}
			}
		};
	}

	private static TripletsInMiniGridsStrategy GetStrategyInstance()
	{
		return new TripletsInMiniGridsStrategy();
	}
}