using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Infrastructure.Services.Strategies;

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
			puzzle.GetCell(0, 0).PossibleValues.Should().BeEquivalentTo([7,8,9]);
			puzzle.GetCell(1, 0).PossibleValues.Should().BeEquivalentTo([7,8,9]);
			puzzle.GetCell(2, 0).PossibleValues.Should().BeEquivalentTo([7,8,9]);
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
				.Should().NotContain(4)
				.And.NotContain(5)
				.And.NotContain(6);
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
		var puzzle = PuzzleFactory.GetEmptyPuzzle();
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
		puzzle.GetCell(3, 4).Value = 7;
		puzzle.GetCell(3, 8).Value = 9;
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

		values = PuzzleFactory.RotateGrid(values);

		return PuzzleFactory.PopulateCells(values);
	}
}