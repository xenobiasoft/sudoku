using DepenMock.XUnit;
using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;
using UnitTests.Helpers;

namespace UnitTests.StrategyTests;

public class TripletsInMiniGridsStrategyTests : BaseTestByAbstraction<TripletsInMiniGridsStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenThreeCellsHavePossibleValuesWithLengthOfThreeInMiniGrid_ThenSetAsTriplets()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Multiple(() =>
		{
			puzzle.GetCell(0, 0).PossibleValues.Should().Be("123");
			puzzle.GetCell(1, 1).PossibleValues.Should().Be("123");
			puzzle.GetCell(2, 2).PossibleValues.Should().Be("123");
		});
	}

	[Fact]
	public void SolvePuzzle_WhenTripletsAreFoundInMiniGrid_RemovesTripletsPossibleValuesFromOtherCells()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var col = 0; col < 3; col++)
		{
			for (var row = 0; row < 3; row++)
			{
				if ((col == 0 && row == 0) || (col == 1 && row == 1) || (col == 2 && row == 2)) continue;

				puzzle.GetCell(row, col).PossibleValues
					.Should().NotContain("1")
					.And.NotContain("2")
					.And.NotContain("3");
			}
		}
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsExpectedScore()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var initialCellsWithValues = puzzle.Count(x => x.Value.HasValue);
		var sut = ResolveSut();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		var expectedScore = (puzzle.Count(x => x.Value.HasValue) - initialCellsWithValues) * 4;
		score.Should().Be(expectedScore);
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsNotSet_ReturnsScoreOfZero()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetEmptyPuzzle();
		var sut = ResolveSut();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		score.Should().Be(0);
	}

	private Cell[] GetTripletsPuzzle()
	{
		var values = new int?[,] {
			{ null, 4, 5, 6, 7, 8, 9, null, null },
			{ 9, null, 8, 7, 6, 5, 4, null, null },
			{ 8, 7, null, 6, 5, 4, null, null, null },
			{ null, null, null, null, null, null, null, null, null },
			{ null, null, null, null, null, null, null, null, null },
			{ null, null, null, null, null, null, null, null, null },
			{ null, null, null, null, null, null, 3, 2, 1 },
			{ null, null, null, null, null, null, 2, 1, 3 },
			{ null, null, null, null, null, null, 1, 3, 2 }
		};

		return PuzzleFactory.PopulateCells(values);
	}
}