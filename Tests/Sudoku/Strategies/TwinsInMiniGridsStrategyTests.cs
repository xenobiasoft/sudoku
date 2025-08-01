﻿using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Sudoku.Strategies;

public class TwinsInMiniGridsStrategyTests : BaseTestByAbstraction<TwinsInMiniGridsStrategy, SolverStrategy>
{
	[Fact]
	public void SolvePuzzle_WhenTwoCellsHaveSamePossibleValuesWithLengthOfTwoInMiniGrid_ThenSetAsTwins()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Medium);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Multiple(() =>
		{
			puzzle.GetCell(7, 7).PossibleValues.Should().BeEquivalentTo([1, 5]);
			puzzle.GetCell(7, 8).PossibleValues.Should().BeEquivalentTo([1,5]);
		});
	}

	[Fact]
	public void SolvePuzzle_WhenTwinsAreFoundInMiniGrid_RemovesTwinsNumbersFromOtherCells()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Medium);
		var sut = ResolveSut();
		var twinCells = new List<Cell>
		{
			puzzle.GetCell(7, 7),
			puzzle.GetCell(7, 8)
		};

		// Act
		sut.SolvePuzzle(puzzle);
		
		// Assert
		puzzle
			.GetMiniGridCells(twinCells.First().Row, twinCells.First().Column)
			.Where(x => !twinCells.Contains(x))
			.ToList()
			.ForEach(x => x.PossibleValues.Should().NotContain(1).And.NotContain(5));
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
		var values = new int?[,] {
			{6, null, 9, 1, null, null, null, null, null},
			{null, 8, 3, null, null, null, null, null, null},
			{7, null, 1, 6, null, null, null, null, null},
			{null, 7, 6, 5, null, null, 3, 9, null},
			{8, null, 4, null, null, null, null, null, null},
			{1, null, 5, 7, null, null, null, null, null},
			{null, 1, 2, 8, null, null, null, null, null},
			{null, 6, 8, 9, null, null, null, null, null},
			{null, 9, 7, null, null, null, null, null, null}
		};
		var puzzle = PuzzleFactory.PopulateCells(values);
		var sut = ResolveSut();

		// Act
		void SolvePuzzle() => sut.SolvePuzzle(puzzle);

		// Assert
		Assert.Throws<InvalidMoveException>(SolvePuzzle);
	}
}