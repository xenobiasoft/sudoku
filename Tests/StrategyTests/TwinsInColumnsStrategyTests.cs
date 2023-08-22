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
		var puzzle = GetTwinsPuzzle();
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.PossibleValues[4, 1].Should().Be(puzzle.PossibleValues[4, 3]).And.Be("23");
	}

	[Fact]
	public void SolvePuzzle_WhenTwinsAreFoundInColumn_RemovesTwinsNumbersFromOtherCells()
	{
		// Arrange
		var puzzle = GetTwinsPuzzle();
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var row = 0; row < 9; row++)
		{
			if (row is 1 or 3) continue;

			puzzle.PossibleValues[4, row].Should().NotContain("2").And.NotContain("3");
		}
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsScoreGreaterThanZero()
	{
		// Arrange
		var puzzle = GetTwinsPuzzle();
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
		var puzzle = GetTwinsPuzzle();
		puzzle.Values[2, 4] = 1;
		var sut = ResolveSut();

		// Act
		void SolvePuzzle() => sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Throws<InvalidOperationException>(SolvePuzzle);
	}

	private SudokuPuzzle GetTwinsPuzzle()
	{
		return new SudokuPuzzle
		{
			Values = new[,]
			{
				{
					6, 0, 9, 1, 8, 0, 0, 0, 0
				},
				{
					0, 8, 3, 0, 9, 0, 0, 0, 0
				},
				{
					7, 0, 1, 6, 0, 0, 0, 0, 0
				},
				{
					0, 7, 6, 5, 0, 0, 0, 0, 0
				},
				{
					0, 0, 4, 0, 0, 0, 0, 0, 0
				},
				{
					1, 0, 5, 7, 0, 0, 0, 0, 0
				},
				{
					0, 1, 2, 8, 0, 0, 0, 0, 0
				},
				{
					0, 6, 8, 9, 0, 0, 0, 0, 0
				},
				{
					0, 9, 7, 0, 6, 0, 0, 0, 0
				}
			}
		};
	}
}