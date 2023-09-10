using DepenMock.XUnit;
using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;
using UnitTests.Helpers;

namespace UnitTests.StrategyTests;

public class TripletsInColumnsStrategyTests : BaseTestByAbstraction<TripletsInColumnsStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenThreeCellsHaveSamePossibleValuesWithLengthOfThreeInColumn_ThenSetAsTriplets()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Multiple(() =>
		{
			puzzle.GetCell(0, 0).PossibleValues.Should().Be("456");
			puzzle.GetCell(0, 1).PossibleValues.Should().Be("456");
			puzzle.GetCell(0, 2).PossibleValues.Should().Be("456");
		});
	}

	[Fact]
	public void SolvePuzzle_WhenTripletsAreFoundInColumn_RemovesTripletsPossibleValuesFromOtherCells()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var row = 3; row < 9; row++)
		{
			puzzle.GetCell(row, 2).PossibleValues
				.Should().NotContain("4")
				.And.NotContain("5")
				.And.NotContain("6");
		}
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsScoreGreaterThanZero()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = ResolveSut();

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
		var sut = ResolveSut();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		score.Should().Be(0);
	}

	[Fact]
	public void SolvePuzzle_WhenNonTripletPossibleValuesIsEmpty_ThrowsException()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		puzzle.GetCell(4, 2).Value = 4;
		puzzle.GetCell(5, 2).Value = 5;
		puzzle.GetCell(6, 2).Value = 6;
		var sut = ResolveSut();

		// Act
		void SolvePuzzle() => sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Throws<InvalidOperationException>(SolvePuzzle);
	}

	private Cell[] GetTripletsPuzzle()
	{
		var values = new int?[,] {
			{ null, null, null, null, 1, 2, 3, 7, 8 },
			{ 1, 7, null, null, null, null, null, null, null },
			{ 2, 8, null, null, null, null, null, null, null },
			{ 3, 9, null, null, null, null, null, null, null },
			{ 7, 1, null, null, null, null, null, null, null },
			{ 8, 2, null, null, null, null, null, null, null },
			{ 9, 3, null, null, null, null, null, null, null },
			{ null, null, 3, null, null, null, null, null, null },
			{ null, null, 9, null, null, null, null, null, null }
		};

		return PuzzleFactory.PopulateCells(values);
	}
}