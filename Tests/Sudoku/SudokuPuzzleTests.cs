using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

namespace UnitTests.Sudoku;

public class SudokuPuzzleTests : BaseTestByAbstraction<SudokuPuzzle, ISudokuPuzzle>
{
    [Fact]
    public void FindCellWithFewestPossibleValues_ReturnsFirstCellWithLeastPossibleValues()
    {
        // Arrange
        var expectedCell = new Cell(2, 0)
        {
            PossibleValues = "12346789"
        };
        var sut = ResolveSut();
        sut.SetCell(2, 4, 5);
        sut.PopulatePossibleValues();

        // Act
        var actualCell = sut.FindCellWithFewestPossibleValues();

        // Assert
        actualCell.Should().BeEquivalentTo(expectedCell);
    }

    [Fact]
    public void GetCell_ReturnsCellThatMatchesRowColumn()
    {
        // Arrange
        var col = RandomGenerator.RandomNumber(0, 9);
        var row = RandomGenerator.RandomNumber(0, 9);
        var sut = ResolveSut();

        // Act
        var cell = sut.GetCell(row, col);

        // Assert
        Assert.Multiple(() =>
        {
            cell.Row.Should().Be(row);
            cell.Column.Should().Be(col);
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
    public void GetColumnCells_ReturnsAllCellsInSpecifiedColumn(int col)
    {
        // Arrange
        var sut = ResolveSut();

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
    [InlineData(0, 2, 0, 2, 0, 2)]
    [InlineData(1, 6, 0, 2, 6, 8)]
    [InlineData(4, 4, 3, 5, 3, 5)]
    [InlineData(8, 2, 6, 8, 0, 2)]
    [InlineData(5, 3, 3, 5, 3, 5)]
    public void GetMiniGridCells_ReturnsAllCellsLocatedWithinMiniGrid(int row, int col, int minRow, int maxRow, int minCol, int maxCol)
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var miniGridCells = sut.GetMiniGridCells(row, col);

        // Assert
        Assert.Multiple(() =>
        {
            miniGridCells.Count().Should().Be(9);
            miniGridCells.ToList().ForEach(x =>
            {
                x.Row.Should().BeGreaterThanOrEqualTo(minRow).And.BeLessThanOrEqualTo(maxRow);
                x.Column.Should().BeGreaterThanOrEqualTo(minCol).And.BeLessThanOrEqualTo(maxCol);
            });
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
        var sut = ResolveSut();

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
    [InlineData(Level.Easy)]
    [InlineData(Level.Medium)]
    [InlineData(Level.Hard)]
    [InlineData(Level.ExtremelyHard)]
    public void IsValid_WhenGivenValidPuzzle_ReturnsTrue(Level level)
    {
        // Arrange
        var sut = PuzzleFactory.GetPuzzle(level);

        // Act
        var isValid = sut.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenGivenEmptyPuzzle_ReturnsTrue()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var isValid = sut.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenGivenCompletedValidPuzzle_ReturnsTrue()
    {
        // Arrange
        var sut = PuzzleFactory.GetSolvedPuzzle();

        // Act
        var isValid = sut.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void Load_SetsPuzzleIdOnCurrentPuzzle()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
        var sut = ResolveSut();

        // Act
        sut.Load(puzzle.PuzzleId, puzzle.GetAllCells());

        // Assert
        sut.PuzzleId.Should().Be(puzzle.PuzzleId);
    }

    [Fact]
    public void PopulatePossibleValues_WhenCellDoesNotHaveValue_PopulatesPossibleValues()
    {
        // Arrange
        var expectedPossibleValues = new[,]
        {
            {"","","124","26","","2468","1489","1249","248"},
            {"","247","247","","","","3478","234","2478"},
            {"12","","","23","34","24","13457","","247"},
            {"","125","1259","579","","147","4579","2459",""},
            {"","25","2569","","5","","579","259",""},
            {"","15","1359","59","","14","4589","459",""},
            {"139","","134579","357","35","7","","","4"},
            {"23","278","237","","","","36","3",""},
            {"123","1245","12345","2356","","26","1346","",""}
        };
        var sut = PuzzleFactory.GetPuzzle(Level.Easy);

        // Act
        sut.PopulatePossibleValues();

        // Assert
        GetAllPossibleValues(sut.GetAllCells()).Should().BeEquivalentTo(expectedPossibleValues);
    }

    [Fact]
    public void SetCellWithFewestPossibleValues_FindsCellWithFewestPossibleValues_AndSetsValueBasedOnOneOfThoseValues()
    {
        // Arrange
        var sut = PuzzleFactory.GetPuzzle(Level.ExtremelyHard);
        sut.PopulatePossibleValues();
        var cell = sut.FindCellWithFewestPossibleValues();
        var possibleValues = cell
            .PossibleValues
            .ToArray()
            .Select(x => int.Parse(x.ToString()));

        // Act
        sut.SetCellWithFewestPossibleValues();

        // Assert
        sut.GetCell(cell.Row, cell.Column).Value.Should().BeOneOf(possibleValues);
    }

    [Fact]
    public void Validate_WhenInvalidNumberIsEntered_ReturnsTheConflictingCells()
    {
        // Arrange
        var sut = PuzzleFactory.GetPuzzle(Level.Easy);
        sut.SetCell(0, 2, 5);

        // Act
        var results = sut.Validate().ToList();

        // Assert
        Assert.Multiple(() =>
        {
            results.Count.Should().Be(2);
            results[0].Row.Should().Be(0);
            results[0].Column.Should().Be(0);
            results[1].Row.Should().Be(0);
            results[1].Column.Should().Be(2);
        });
    }

    private static string[,] GetAllPossibleValues(Cell[] cells)
    {
        var possibleValues = new string[9, 9];

        cells.ToList().ForEach(x => possibleValues[x.Row, x.Column] = x.PossibleValues);

        return possibleValues;
    }
}