using DepenMock.XUnit;
using XenobiaSoft.Sudoku.Strategies;
using XenobiaSoft.Sudoku;
using UnitTests.Helpers;

namespace UnitTests.StrategyTests;

public class TwinsInColumnsStrategyTests : BaseTestByAbstraction<TwinsInColumnsStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenTwoCellsHaveSamePossibleValuesWithLengthOfTwoInColumn_ThenSetAsTwins()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Multiple(() =>
		{
			puzzle.GetCell(3, 7).PossibleValues.Should().Be("25");
			puzzle.GetCell(4, 7).PossibleValues.Should().Be("25");
		});
	}

	[Fact]
	public void SolvePuzzle_WhenTwinsAreFoundInColumn_RemovesTwinsNumbersFromOtherCells()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var col = 0; col < 9; col++)
		{
			if (col is 3 or 4) continue;

			puzzle.GetCell(col, 7).PossibleValues.Should().NotContain("2").And.NotContain("5");
		}
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsScoreGreaterThanZero()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = ResolveSut();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		score.Should().Be(3);
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
	public void SolvePuzzle_WhenNonTwinCellPossibleValuesIsEmpty_ThrowsException()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		puzzle.GetCell(5, 3).Value = 3;
		var sut = ResolveSut();

		// Act
		void SolvePuzzle() => sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Throws<InvalidOperationException>(SolvePuzzle);
	}
}