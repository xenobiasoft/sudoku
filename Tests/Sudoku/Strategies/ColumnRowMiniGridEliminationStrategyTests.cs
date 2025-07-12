using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Sudoku.Strategies;

public class ColumnRowMiniGridEliminationStrategyTests : BaseTestByAbstraction<ColumnRowMiniGridEliminationStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenOnlyOnePossibleValue_ThenValueEqualsThatNumber()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Easy);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.GetCell(4, 4).Value.Should().Be(5);
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsTrue()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Easy);
		var sut = ResolveSut();

		// Act
		var changesMade = sut.SolvePuzzle(puzzle);

        // Assert
        changesMade.Should().BeTrue();
    }

	[Fact]
	public void SolvePuzzle_WhenACellValueIsNotSet_ReturnsScoreOfZero()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.ExtremelyHard);
		var sut = ResolveSut();

		// Act
		var changesMade = sut.SolvePuzzle(puzzle);

        // Assert
        changesMade.Should().BeFalse();
    }
}