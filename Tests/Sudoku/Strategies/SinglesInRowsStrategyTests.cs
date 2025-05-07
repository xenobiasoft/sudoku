using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Sudoku.Strategies;

public class SinglesInRowsStrategyTests : BaseTestByAbstraction<SinglesInRowsStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenPossibleNumberOccursOnlyOnceInRow_SetValueToThatNumber()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.GetCell(2, 6).Value.Should().Be(5);
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsTrue()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
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
}