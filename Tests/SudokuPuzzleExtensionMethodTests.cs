using UnitTests.CustomAssertions;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Helpers;

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
	public void FindCellWithFewestPossibleValues_DoesWhatItsNamed()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetEmptyPuzzle();
		puzzle.Values[2, 4] = 5;
		puzzle.PopulatePossibleValues();

		// Act
		var cellWithFewestPossibleValues = puzzle.FindCellWithFewestPossibleValues();

		// Assert
		Assert.Multiple(() =>
		{
			cellWithFewestPossibleValues.Item1.Should().Be(0);
			cellWithFewestPossibleValues.Item2.Should().Be(3);
		});
	}

	[Fact]
	public void SetCellWithFewestPossibleValues_FindsCellWithFewestPossibleValues_AndSetsValueBasedOnOneOfThoseValues()
	{
		// Arrange
		var puzzle = PuzzleFactory
			.GetPuzzle(Level.ExtremelyHard)
			.PopulatePossibleValues();

		// Act
		puzzle.SetCellWithFewestPossibleValues();

		// Assert
		puzzle.Values[3, 5].Should().BeOneOf(5, 9);
	}

	[Fact]
	public void Reset_SetsValuesAllToZero()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetSolvedPuzzle();

		// Act
		puzzle.Reset();

		// Assert
		puzzle.Values.Should().BeEmpty();
	}

	[Fact]
	public void Reset_SetsPossibleValuesAllToEmptyString()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetSolvedPuzzle();

		// Act
		puzzle.Reset();

		// Assert
		puzzle.PossibleValues.Should().BeEmpty();
	}
}