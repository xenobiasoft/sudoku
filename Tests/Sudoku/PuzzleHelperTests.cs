using XenobiaSoft.Sudoku.Helpers;

namespace UnitTests.Sudoku;

public class PuzzleHelperTests
{
	[Theory]
	[InlineData(0, 0)]
	[InlineData(1, 0)]
	[InlineData(2, 0)]
	[InlineData(3, 3)]
	[InlineData(4, 3)]
	[InlineData(5, 3)]
	[InlineData(6, 6)]
	[InlineData(7, 6)]
	[InlineData(8, 6)]
	public void CalculateMiniGridStartColumn_WhenGivenCellCoordinates_ReturnsStartLocationForCell(int col, int expectedStartCol)
	{
		// Arrange
		
		// Act
		var actualStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);

		// Assert
		actualStartCol.Should().Be(expectedStartCol);
	}

	[Theory]
	[InlineData(0, 0)]
	[InlineData(1, 0)]
	[InlineData(2, 0)]
	[InlineData(3, 3)]
	[InlineData(4, 3)]
	[InlineData(5, 3)]
	[InlineData(6, 6)]
	[InlineData(7, 6)]
	[InlineData(8, 6)]
	public void CalculateMiniGridStartRow_WhenGivenCellCoordinates_ReturnsStartLocationForCell(int row, int expectedStartRow)
	{
		// Arrange

		// Act
		var actualStartRow = PuzzleHelper.CalculateMiniGridStartRow(row);

		// Assert
		actualStartRow.Should().Be(expectedStartRow);
	}
}