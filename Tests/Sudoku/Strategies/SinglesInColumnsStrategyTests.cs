using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Sudoku.Strategies;

public class SinglesInColumnsStrategyTests : BaseTestByAbstraction<SinglesInColumnsStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenPossibleNumberOccursOnlyOnceInColumn_SetValueToThatNumber()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.GetCell(6, 0).Value.Should().Be(9);
	}

	[Fact]
	public void SolvePuzzle_WhenCellValueIsSet_ReturnsExpectedScore()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var initialCellsWithValues = puzzle.GetAllCells().Count(x => x.Value.HasValue);
		var sut = ResolveSut();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		var expectedScore = (puzzle.GetAllCells().Count(x => x.Value.HasValue) - initialCellsWithValues) * 2;
		score.Should().Be(expectedScore);
	}

	[Fact]
	public void SolvePuzzle_WhenCellValueIsSet_RemovesThatValueFromColumnCellsPossibleValues()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var loneRangerCell = puzzle.GetCell(6, 0);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.GetColumnCells(loneRangerCell.Column).ToList().ForEach(x => x.PossibleValues.Should().NotContain("9"));
	}

	[Fact]
	public void SolvePuzzle_WhenCellValueIsNotSet_ReturnsScoreOfZero()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetEmptyPuzzle();
		var sut = ResolveSut();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		score.Should().Be(0);
	}
}