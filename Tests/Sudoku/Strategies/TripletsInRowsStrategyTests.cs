using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Sudoku.Strategies;

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
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsTrue()
	{
		// Arrange
		var puzzle = GetTripletsPuzzle();
		var sut = ResolveSut();

		// Act
		var changesMade = sut.SolvePuzzle(puzzle);

		// Assert
        changesMade.Should().BeTrue();
    }

	[Fact]
	public void SolvePuzzle_WhenACellValueIsNotSet_ReturnsFalse()
	{
		// Arrange
		var values = new int?[,] {
			{ null, null, null, null, null, null, 4, 5, 6 },
			{ null, null, null, null, null, null, 5, 6, 4 },
			{ null, null, null, null, null, null, 6, 4, 5 },
			{ null, 5, 6, null, null, null, null, null, null },
			{ null, null, null, null, null, null, null, null, null },
			{ null, null, null, null, null, null, null, null, null },
			{ 3, 4, 5, 6, 7, 8, 9, 1, 2 },
			{ 2, 3, 4, 5, 6, 7, 8, 9, 1 },
			{ 1, 2, 3, 4, 5, 6, 7, 8, 9 }
		};
		var puzzle = PuzzleFactory.PopulateCells(values);
		var sut = ResolveSut();

		// Act
		var changesMade = sut.SolvePuzzle(puzzle);

		// Assert
        changesMade.Should().BeFalse();
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
		Assert.Throws<InvalidMoveException>(SolvePuzzle);
	}

	private ISudokuPuzzle GetTripletsPuzzle()
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

		return PuzzleFactory.PopulateCells(values);
	}
}