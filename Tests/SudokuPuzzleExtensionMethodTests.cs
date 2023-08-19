﻿using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

namespace UnitTests;

public class SudokuPuzzleExtensionMethodTests
{
	[Fact]
	public void PopulatePossibleValues_WhenCellDoesNotHaveValue_PopulatesPossibleValues()
	{
		// Arrange
		var expectedPossibleValues = new[,]
		{
			{"", "", "124", "26", "", "2468", "1489", "1249", "248"},
			{"", "247", "247", "", "", "", "3478", "234", "2478"},
			{"12", "", "", "23", "34", "24", "13457", "", "247"},
			{"", "125", "1259", "579", "", "147", "4579", "2459", ""},
			{"", "25", "2569", "", "5", "", "579", "259", ""},
			{"", "15", "1359", "59", "", "14", "4589", "459", ""},
			{"139", "", "134579", "357", "35", "7", "", "", "4"},
			{"23", "278", "237", "", "", "", "36", "3", ""},
			{"123", "1245", "12345", "2356", "", "26", "1346", "", ""}
		};
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);

		// Act
		puzzle.PopulatePossibleValues();

		// Assert
		puzzle.PossibleValues.Should().BeEquivalentTo(expectedPossibleValues);
	}

	[Fact]
	public void IsValid_WhenGivenValidPuzzle_ReturnsTrue()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetSolvedPuzzle();

		// Act
		var isValid = puzzle.IsValid();

		// Assert
		isValid.Should().BeTrue();
	}

	[Fact]
	public void IsSolved_WhenPuzzleIsValidAndAllValuesPopulatedWithNumber_ReturnsTrue()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetSolvedPuzzle();

		// Act
		var isSolved = puzzle.IsSolved();

		// Assert
		isSolved.Should().BeTrue();
	}
}