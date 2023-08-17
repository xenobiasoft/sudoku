using FluentAssertions;
using XenobiaSoft.Sudoku;

namespace UnitTests;

public class SudokuGameTests
{
	[Fact]
	public void SudokuGame_WhenInitialized_HasValidSize()
	{
		// Arrange
		var sut = new SudokuGame();

		// Act
		sut.Initialize();

		// Assert
		sut.Columns.Should().Be(9);
		sut.Rows.Should().Be(9);
	}

	[Fact]
	public void SudokuGame_WhenInitialized_IsValidBoard()
	{
		// Arrange
		var sut = new SudokuGame();

		// Act
		sut.Initialize();

		// Assert
		sut.IsValid().Should().BeTrue();
	}

	// Validate:
	//		Rows
	//		Columns
	//		SubGrids
}