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
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsExpectedScore()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Medium, rotateGrid: true);
		var initialCellsWithValues = puzzle.Count(x => x.Value.HasValue);
		var sut = ResolveSut();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		var expectedScore = (puzzle.Count(x => x.Value.HasValue) - initialCellsWithValues) * 3;
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
	public void SolvePuzzle_WhenNonTwinCellPossibleValuesIsEmpty_ThrowsException()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy, rotateGrid: true);
		puzzle.GetCell(2, 4).Value = 7;
		var sut = ResolveSut();

		// Act
		void SolvePuzzle() => sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Throws<InvalidOperationException>(SolvePuzzle);
	}
}