using DepenMock.XUnit;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services.Strategies;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Services.Strategies;

public class SinglesInMiniGridStrategyTests : BaseTestByAbstraction<SinglesInMiniGridsStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenPossibleNumberOccursOnlyOnceInMiniGrid_SetValueToThatNumber()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Medium);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.GetCell(0, 2).Value.Should().Be(8);
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsExpectedScore()
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
	public void SolvePuzzle_WhenACellValueIsNotSet_ReturnsScoreOfZero()
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