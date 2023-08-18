using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;

namespace UnitTests.StrategyTests;

public class TripletsInColumnsStrategyTests
{
	[Fact]
	public void SolvePuzzle_WhenThreeCellsHaveSamePossibleValuesWithLengthOfThreeInColumn_ThenSetAsTriplets()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = new TripletsInColumnsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.PossibleValues[0, 0]
			.Should().Be(puzzle.PossibleValues[0, 1])
			.And.Be(puzzle.PossibleValues[0, 2])
			.And.Be("456");
	}

	[Fact]
	public void SolvePuzzle_WhenTripletsAreFoundInColumn_RemovesTripletsPossibleValuesFromOtherCells()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = new TripletsInColumnsStrategy();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var row = 3; row < 9; row++)
		{
			puzzle.PossibleValues[0, row]
				.Should().NotContain("4")
				.And.NotContain("5")
				.And.NotContain("6");
		}
	}

	private SudokuPuzzle GetTripletsPuzzle()
	{
		return new SudokuPuzzle
		{
			Values = new[,] {
				{
					0, 0, 0, 0, 0, 0, 0, 0, 0
				},
				{
					1, 7, 0, 0, 0, 0, 0, 0, 0
				},
				{
					2, 8, 0, 0, 0, 0, 0, 0, 0
				},
				{
					3, 9, 0, 0, 0, 0, 0, 0, 0
				},
				{
					7, 1, 0, 0, 0, 0, 0, 0, 0
				},
				{
					8, 2, 0, 0, 0, 0, 0, 0, 0
				},
				{
					9, 3, 0, 0, 0, 0, 0, 0, 0
				},
				{
					0, 0, 3, 0, 0, 0, 0, 0, 0
				},
				{
					0, 0, 9, 0, 0, 0, 0, 0, 0
				}
			}
		};
	}
}