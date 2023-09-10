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
			puzzle.GetCell(0, 0).PossibleValues.Should().Be("789");
			puzzle.GetCell(1, 0).PossibleValues.Should().Be("789");
			puzzle.GetCell(2, 0).PossibleValues.Should().Be("789");
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

	[Fact]
	public void SolvePuzzle_WhenNonTripletPossibleValuesIsEmpty_ThrowsException()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		puzzle.GetCell(3, 3).Value = 4;
		puzzle.GetCell(3, 4).Value = 7;
		puzzle.GetCell(3, 8).Value = 9;
		var sut = ResolveSut();

		// Act
		void SolvePuzzle() => sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Throws<InvalidOperationException>(SolvePuzzle);
	}

	private Cell[] GetTripletsPuzzle()
	{
		var values = new int?[,] {
			{null, null, null, null, null, null, 3, 2, 1},
			{null, null, null, 5, null, null, 4, 3, 2},
			{null, null, null, 6, null, null, 5, 4, 3},
			{null, null, null, null, null, null, 6, 5, 4},
			{null, null, null, null, null, null, 7, 6, 5},
			{null, null, null, null, null, null, 8, 7, 6},
			{4, 5, 6, null, null, null, 9, 8, 7},
			{5, 6, 4, null, null, null, 1, 9, 8},
			{6, 4, 5, null, null, null, 2, 1, 9}
		};

		values = PuzzleFactory.RotateGrid(values);

		return PuzzleFactory.PopulateCells(values);
	}
}