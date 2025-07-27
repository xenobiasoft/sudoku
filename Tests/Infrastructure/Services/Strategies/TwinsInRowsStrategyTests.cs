using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Infrastructure.Services.Strategies;

public class TwinsInRowsStrategyTests : BaseTestByAbstraction<TwinsInRowsStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenTwoCellsHaveSamePossibleValuesWithLengthOfTwoInRow_ThenSetAsTwins()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Medium);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Multiple(() =>
		{
			puzzle.GetCell(7, 7).PossibleValues.Should().BeEquivalentTo([1,5]);
			puzzle.GetCell(7, 8).PossibleValues.Should().BeEquivalentTo([1, 5]);
		});
	}

	[Fact]
	public void SolvePuzzle_WhenTwinsAreFoundInRow_RemovesTwinsNumbersFromOtherCells()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Medium);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		for (var col = 0; col < 9; col++)
		{
			if (col is 7 or 8) continue;

			puzzle.GetCell(7, col).PossibleValues.Should().NotContain(1).And.NotContain(5);
		}
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsTrue()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Medium);
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
		var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Easy);
		puzzle.GetCell(4, 2).Value = 7;
		var sut = ResolveSut();

		// Act
		void SolvePuzzle() => sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Throws<InvalidMoveException>(SolvePuzzle);
	}
}