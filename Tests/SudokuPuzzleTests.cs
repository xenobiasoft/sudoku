using UnitTests.CustomAssertions;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

namespace UnitTests;

public class SudokuPuzzleTests
{
	[Fact]
	public void PopulatePossibleValues_WhenCellDoesNotHaveValue_PopulatesPossibleValues()
	{
		// Arrange
		var expectedPossibleValues = new[,]
		{
			{"","","124","269","","126","1489","1249","248"},
			{"","247","247","","","","3478","234","2478"},
			{"12","","","235","35","12","13457","","247"},
			{"","24","24","579","","147","57","25",""},
			{"","2","26","","5","","567","25",""},
			{"","48","34","59","","14","35","35",""},
			{"139","","134579","579","45","47","","","4"},
			{"23","278","237","","","","36","3",""},
			{"123","1245","12345","25","","24","1346","",""},
		};
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);

		// Act
		puzzle.PopulatePossibleValues();

		// Assert
		puzzle.GetCellPossibleValues().Should().BeEquivalentTo(expectedPossibleValues);
	}

	[Fact]
	public void FindCellWithFewestPossibleValues_ReturnsFirstCellWithLeastPossibleValues()
	{
		// Arrange
		var expectedCell = new Cell(2, 0)
		{
			PossibleValues = "12346789"
		};
		var puzzle = PuzzleFactory.GetEmptyPuzzle();
		puzzle.GetCell(2, 4).Value = 5;
		puzzle.PopulatePossibleValues();

		// Act
		var cellWithFewestPossibleValues = puzzle.FindCellWithFewestPossibleValues();

		// Assert
		cellWithFewestPossibleValues.Should().BeEquivalentTo(expectedCell);
	}

	[Fact]
	public void SetCellWithFewestPossibleValues_FindsCellWithFewestPossibleValues_AndSetsValueBasedOnOneOfThoseValues()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.ExtremelyHard);
		puzzle.PopulatePossibleValues();
		var cell = puzzle.FindCellWithFewestPossibleValues();
		var possibleValues = cell
			.PossibleValues
			.ToArray()
			.Select(x => int.Parse(x.ToString()));

		// Act
		puzzle.SetCellWithFewestPossibleValues();

		// Assert
		puzzle.GetCell(cell.Row, cell.Column).Value.Should().BeOneOf(possibleValues);
	}

	[Fact]
	public void Reset_SetsValuesAllToNull()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetSolvedPuzzle();

		// Act
		puzzle.Reset();

		// Assert
		puzzle.Cells.ToList().ForEach(x => x.Value.Should().BeNull());
	}

	[Fact]
	public void Reset_SetsPossibleValuesAllToEmptyString()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetSolvedPuzzle();

		// Act
		puzzle.Reset();

		// Assert
		puzzle.GetCellPossibleValues().Should().BeEmpty();
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	[InlineData(4)]
	[InlineData(5)]
	[InlineData(6)]
	[InlineData(7)]
	[InlineData(8)]
	public void GetColumnCells_ReturnsAllCellsInSpecifiedColumn(int col)
	{
		// Arrange
		var sut = new SudokuPuzzle();

		// Act
		var columnCells = sut.GetColumnCells(col);

		// Assert
		Assert.Multiple(() =>
		{
			columnCells.Count().Should().Be(9);
			columnCells.ToList().ForEach(x => x.Column.Should().Be(col));
		});
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	[InlineData(4)]
	[InlineData(5)]
	[InlineData(6)]
	[InlineData(7)]
	[InlineData(8)]
	public void GetRowCells_ReturnsAllCellsInSpecifiedRow(int row)
	{
		// Arrange
		var sut = new SudokuPuzzle();

		// Act
		var rowCells = sut.GetRowCells(row);

		// Assert
		Assert.Multiple(() =>
		{
			rowCells.Count().Should().Be(9);
			rowCells.ToList().ForEach(x => x.Row.Should().Be(row));
		});
	}

	[Theory]
	[InlineData(0, 2, 0, 2, 0, 2)]
	[InlineData(1, 6, 0, 2, 6, 8)]
	[InlineData(4, 4, 3, 5, 3, 5)]
	[InlineData(8, 2, 6, 8, 0, 2)]
	[InlineData(5, 3, 3, 5, 3, 5)]
	public void GetMiniGridCells_ReturnsAllCellsLocatedWithinMiniGrid(int row, int col, int minRow, int maxRow, int minCol, int maxCol)
	{
		// Arrange
		var sut = new SudokuPuzzle();

		// Act
		var miniGridCells = sut.GetMiniGridCells(row, col);

		// Assert
		Assert.Multiple(() =>
		{
			miniGridCells.Count().Should().Be(9);
			miniGridCells.ToList().ForEach(x =>
			{
				x.Row.Should().BeGreaterOrEqualTo(minRow).And.BeLessOrEqualTo(maxRow);
				x.Column.Should().BeGreaterOrEqualTo(minCol).And.BeLessOrEqualTo(maxCol);
			});
		});
	}

	[Fact]
	public void GetCell_ReturnsCellThatMatchesRowColumn()
	{
		// Arrange
		var rnd = new Random();
		var col = rnd.Next(0, 9);
		var row = rnd.Next(0, 9);
		var sut = new SudokuPuzzle();

		// Act
		var cell = sut.GetCell(row, col);

		// Assert
		Assert.Multiple(() =>
		{
			cell.Row.Should().Be(row);
			cell.Column.Should().Be(col);
		});
	}
}