using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;
using UnitTests.Helpers;

namespace UnitTests.StrategyTests;

public class TripletsInRowsStrategyTests
{
	[Fact]
	public void SolvePuzzle_WhenTwoCellsHaveSamePossibleValuesWithLengthOfThreeInRow_ThenSetAsTriplets()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = GetStrategyInstance();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.PossibleValues[0, 0]
			.Should().Be(puzzle.PossibleValues[1, 0])
			.And.Be(puzzle.PossibleValues[2, 0])
			.And.Be("789");
	}

	[Fact]
	public void SolvePuzzle_WhenTripletsAreFoundInRow_RemovesTripletsPossibleValuesFromOtherCells()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = GetStrategyInstance();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var col = 3; col < 9; col++)
		{
			puzzle.PossibleValues[col, 0]
				.Should().NotContain("7")
				.And.NotContain("8")
				.And.NotContain("9");
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
					0, 0, 0, 0, 0, 0, 4, 5, 6
				},
				{
					0, 0, 0, 0, 0, 0, 5, 6, 4
				},
				{
					0, 0, 0, 0, 0, 0, 6, 4, 5
				},
				{
					0, 5, 6, 0, 0, 0, 0, 0, 0
				},
				{
					0, 0, 0, 0, 0, 0, 0, 0, 0
				},
				{
					0, 0, 0, 0, 0, 0, 0, 0, 0
				},
				{
					3, 4, 5, 6, 7, 8, 9, 1, 2
				},
				{
					2, 3, 4, 5, 6, 7, 8, 9, 1
				},
				{
					1, 2, 3, 4, 5, 6, 7, 8, 9
				}
			}
		};
	}

	private static TripletsInRowsStrategy GetStrategyInstance()
	{
		return new TripletsInRowsStrategy();
	}
}