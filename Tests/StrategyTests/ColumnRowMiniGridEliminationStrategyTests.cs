﻿using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class ColumnRowMiniGridEliminationStrategyTests
{
	[Fact]
	public void SolvePuzzle_WhenOnlyOnePossibleValue_ThenValueEqualsThatNumber()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = GetStrategyInstance();

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		puzzle.Values[4, 4].Should().Be(int.Parse(puzzle.PossibleValues[4, 4]));
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsSet_ReturnsScoreGreaterThanZero()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = GetStrategyInstance();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		score.Should().Be(1);
	}

	[Fact]
	public void SolvePuzzle_WhenACellValueIsNotSet_ReturnsScoreOfZero()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.ExtremelyHard);
		var sut = GetStrategyInstance();

		// Act
		var score = sut.SolvePuzzle(puzzle);

		// Assert
		score.Should().Be(0);
	}

	private static ColumnRowMiniGridEliminationStrategy GetStrategyInstance()
	{
		return new ColumnRowMiniGridEliminationStrategy();
	}
}