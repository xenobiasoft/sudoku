using DepenMock.XUnit;
using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;
using UnitTests.Helpers;

namespace UnitTests.StrategyTests;

public class TripletsInRowsStrategyTests : BaseTestByAbstraction<TripletsInRowsStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenThreeCellsHaveSamePossibleValuesWithLengthOfThreeInRow_ThenSetAsTriplets()
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
			puzzle.GetCell(0, 1).PossibleValues.Should().Be("789");
			puzzle.GetCell(0, 2).PossibleValues.Should().Be("789");
		});
	}

	[Fact]
	public void SolvePuzzle_WhenTripletsAreFoundInRow_RemovesTripletsPossibleValuesFromOtherCells()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var col = 3; col < 9; col++)
		{
			puzzle.GetCell(0, col).PossibleValues.Should().NotContain("7")
				.And.NotContain("8")
				.And.NotContain("9");
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

		var puzzle = new SudokuPuzzle();
		var values = new[,] {
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
		};
		puzzle.RestoreValues(values);
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
		puzzle.GetCell(4, 3).Value = 7;
		puzzle.GetCell(8, 3).Value = 9;
		var sut = ResolveSut();

		// Act
		void SolvePuzzle() => sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Throws<InvalidOperationException>(SolvePuzzle);
	}

	private SudokuPuzzle GetTripletsPuzzle()
	{
		var puzzle = new SudokuPuzzle();
		var values = new[,] {
			{0, 0, 0, 0, 0, 0, 3, 2, 1},
			{0, 0, 0, 5, 0, 0, 4, 3, 2},
			{0, 0, 0, 6, 0, 0, 5, 4, 3},
			{0, 0, 0, 0, 0, 0, 6, 5, 4},
			{0, 0, 0, 0, 0, 0, 7, 6, 5},
			{0, 0, 0, 0, 0, 0, 8, 7, 6},
			{4, 5, 6, 0, 0, 0, 9, 8, 7},
			{5, 6, 4, 0, 0, 0, 1, 9, 8},
			{6, 4, 5, 0, 0, 0, 2, 1, 9}
		};
		puzzle.RestoreValues(values);

		return puzzle;
	}
}