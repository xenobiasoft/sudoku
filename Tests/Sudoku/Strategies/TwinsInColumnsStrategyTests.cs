using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Sudoku.Strategies;

public class TwinsInColumnsStrategyTests : BaseTestByAbstraction<TwinsInColumnsStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenTwoCellsHaveSamePossibleValuesWithLengthOfTwoInColumn_ThenSetAsTwins()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Medium, rotateGrid: true);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Multiple(() =>
		{
			puzzle.GetCell(7, 7).PossibleValues.Should().Be("15");
			puzzle.GetCell(8, 7).PossibleValues.Should().Be("15");
		});
	}

	[Fact]
	public void SolvePuzzle_WhenTwinsAreFoundInColumn_RemovesTwinsNumbersFromOtherCells()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Medium, rotateGrid: true);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var row = 0; row < 9; row++)
		{
			if (row is 7 or 8) continue;

			puzzle.GetCell(row, 7).PossibleValues.Should().NotContain("1").And.NotContain("5");
		}
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsTrue()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Medium, rotateGrid: true);
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
	public void SolvePuzzle_WhenNonTwinCellPossibleValuesIsEmpty_ThrowsException()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy, rotateGrid: true);
		puzzle.GetCell(2, 4).Value = 7;
		var sut = ResolveSut();

		// Act
		void SolvePuzzle() => sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Throws<InvalidMoveException>(SolvePuzzle);
	}
}