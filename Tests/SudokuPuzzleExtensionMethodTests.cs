using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

namespace UnitTests;

public class SudokuPuzzleExtensionMethodTests
{
	[Theory]
	[InlineData("124", Level.Easy, 0, 2)]
	[InlineData("68", Level.Medium, 0, 2)]
	[InlineData("34569", Level.Hard, 0, 1)]
	[InlineData("1269", Level.ExtremelyHard, 0, 2)]
	public void PopulatePossibleValues_WhenCellDoesNotHaveValue_PopulatesPossibleValues(string expectedValues, Level level, int col, int row)
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(level);

		// Act
		puzzle.PopulatePossibleValues();

		// Assert
		puzzle.PossibleValues[col, row].Should().Be(expectedValues);
	}
}