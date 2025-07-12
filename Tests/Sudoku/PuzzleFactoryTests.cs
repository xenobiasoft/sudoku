using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

namespace UnitTests.Sudoku;

public class PuzzleFactoryTests
{
    [Fact]
    public void FindCellWithFewestPossibleValues_ReturnsFirstCellWithLeastPossibleValues()
    {
        // Arrange
        var expectedCell = new Cell(2, 0)
        {
            PossibleValues = [1, 2, 3, 4, 6, 7, 8, 9]
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
    public void GetCell_ReturnsCellThatMatchesRowColumn()
    {
        // Arrange
        var col = RandomGenerator.RandomNumber(0, 9);
        var row = RandomGenerator.RandomNumber(0, 9);
        var sut = PuzzleFactory.GetEmptyPuzzle();

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
        var sut = PuzzleFactory.GetEmptyPuzzle();

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
        var sut = PuzzleFactory.GetEmptyPuzzle();

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
        var sut = PuzzleFactory.GetEmptyPuzzle();

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
    [InlineData(GameDifficulty.Easy)]
    [InlineData(GameDifficulty.Medium)]
    [InlineData(GameDifficulty.Hard)]
    [InlineData(GameDifficulty.ExtremelyHard)]
    public void IsValid_WhenGivenValidPuzzle_ReturnsTrue(GameDifficulty difficulty)
    {
        // Arrange
        var puzzle = PuzzleFactory.GetPuzzle(difficulty);

        // Act
        var isValid = puzzle.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenGivenEmptyPuzzle_ReturnsTrue()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetEmptyPuzzle();

        // Act
        var isValid = puzzle.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenGivenCompletedValidPuzzle_ReturnsTrue()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetSolvedPuzzle();

        // Act
        var isValid = puzzle.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void PopulatePossibleValues_WhenCellDoesNotHaveValue_PopulatesPossibleValues()
    {
        // Arrange
        var expectedPossibleValues = new List<int>[,]
        {
            { [], [], [1, 2, 4], [2, 6], [], [2, 4, 6, 8], [1, 4, 8, 9], [1, 2, 4, 9], [2, 4, 8] },
            { [], [2, 4, 7], [2, 4, 7], [], [], [], [3, 4, 7, 8], [2, 3, 4], [2, 4, 7, 8] },
            { [1, 2], [], [], [2, 3], [3, 4], [2, 4], [1, 3, 4, 5, 7], [], [2, 4, 7] },
            { [], [1, 2, 5], [1, 2, 5, 9], [5, 7, 9], [], [1, 4, 7], [4, 5, 7, 9], [2, 4, 5, 9], [] },
            { [], [2, 5], [2, 5, 6, 9], [], [5], [], [5, 7, 9], [2, 5, 9], [] },
            { [], [1, 5], [1, 3, 5, 9], [5, 9], [], [1, 4], [4, 5, 8, 9], [4, 5, 9], [] },
            { [1, 3, 9], [], [1, 3, 4, 5, 7, 9], [3, 5, 7], [3, 5], [7], [], [], [4] },
            { [2, 3], [2, 7, 8], [2, 3, 7], [], [], [], [3, 6], [3], [] },
            { [1, 2, 3], [1, 2, 4, 5], [1, 2, 3, 4, 5], [2, 3, 5, 6], [], [2, 6], [1, 3, 4, 6], [], [] }
        };
        var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Easy);

        // Act
        puzzle.PopulatePossibleValues();

        // Assert
        GetAllPossibleValues(puzzle).Should().BeEquivalentTo(expectedPossibleValues);
    }

    [Fact]
    public void SetCellWithFewestPossibleValues_FindsCellWithFewestPossibleValues_AndSetsValueBasedOnOneOfThoseValues()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.ExtremelyHard);
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
    public void Validate_WhenInvalidNumberIsEntered_ReturnsTheConflictingCells()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Easy);
        puzzle.SetCell(0, 2, 5);

        // Act
        var results = puzzle.Validate().ToList();

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

    private static List<int>[,] GetAllPossibleValues(ISudokuPuzzle puzzle)
    {
        var possibleValues = new List<int>[9, 9];

        puzzle.GetAllCells().ToList().ForEach(x => possibleValues[x.Row, x.Column] = x.PossibleValues);

        return possibleValues;
    }
}