using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class TwinsInMiniGridsStrategyTests : BaseTestByAbstraction<TwinsInMiniGridsStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenTwoCellsHaveSamePossibleValuesWithLengthOfTwoInMiniGrid_ThenSetAsTwins()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Medium);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Multiple(() =>
		{
			puzzle.GetCell(3, 2).PossibleValues.Should().Be("46");
			puzzle.GetCell(4, 2).PossibleValues.Should().Be("46");
		});
	}

	[Fact]
	public void SolvePuzzle_WhenTwinsAreFoundInMiniGrid_RemovesTwinsNumbersFromOtherCells()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Medium);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);
		
		// Assert
		for (var col = 0; col < 3; col++)
		{
			for (var row = 3; row < 6; row++)
			{
				if (col is 2 && row is 3 or 4) continue;

				puzzle.GetCell(row, col).PossibleValues.Should().NotContain("4").And.NotContain("6");
			}
		}
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsScoreGreaterThanZero()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Medium);
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
	public void SolvePuzzle_WhenNonTripletPossibleValuesIsEmpty_ThrowsException()
	{
		// Arrange
		var puzzle = new SudokuPuzzle();
		var values = new[,] {
			{6, 0, 9, 1, 0, 0, 0, 0, 0},
			{0, 8, 3, 0, 0, 0, 0, 0, 0},
			{7, 0, 1, 6, 0, 0, 0, 0, 0},
			{0, 7, 6, 5, 0, 0, 0, 0, 0},
			{8, 0, 4, 0, 0, 0, 0, 0, 0},
			{1, 0, 5, 7, 0, 0, 0, 0, 0},
			{0, 1, 2, 8, 0, 0, 0, 0, 0},
			{0, 6, 8, 9, 0, 0, 0, 0, 0},
			{0, 9, 7, 0, 0, 0, 0, 0, 0}
		};
		puzzle.RestoreValues(values);
		var sut = ResolveSut();

		// Act
		void SolvePuzzle() => sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Throws<InvalidOperationException>(SolvePuzzle);
	}
}