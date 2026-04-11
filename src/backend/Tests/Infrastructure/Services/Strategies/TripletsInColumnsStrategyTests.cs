using DepenMock.XUnit;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services.Strategies;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Services.Strategies;

public class TripletsInColumnsStrategyTests : BaseTestByAbstraction<TripletsInColumnsStrategy, SolverStrategy>
{
    [Fact]
    public void Execute_WhenRemovalLeadsToEmptyPossibleValues_ThrowsInvalidMoveException()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateInvalidTripletsPuzzle();

        // Act & Assert
        var action = () => sut.Execute(puzzle);
        action.Should().Throw<InvalidMoveException>().WithMessage("Invalid move at row:2, col:0");
    }

    [Fact]
    public void Execute_WithCellsAlreadySet_IgnoresSetCells()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreatePuzzleWithSomeCellsSet();

        // Act
        sut.Execute(puzzle);

        // Assert
        puzzle.GetCell(0, 0).Value.Should().Be(5); // Pre-set value should remain
        puzzle.GetCell(1, 0).Value.Should().Be(6); // Pre-set value should remain
    }

    [Fact]
    public void Execute_WithEmptyPuzzle_ReturnsFalse()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateEmptyPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Execute_WithNakedTriplets_SetsTripletsPossibleValues()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateNakedTripletsWithSingleValueTestPuzzle();

        // Act
        sut.Execute(puzzle);

        // Assert
        puzzle.GetCell(0, 0).PossibleValues.Should().BeEquivalentTo([1, 3]);
        puzzle.GetCell(3, 0).PossibleValues.Should().BeEquivalentTo([1, 2]);
        puzzle.GetCell(6, 0).PossibleValues.Should().BeEquivalentTo([2, 3]);
    }

    [Fact]
    public void Execute_WithHiddenTripletsInColumn_RestrictsCellsToTripletValues()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateHiddenTripletsTestPuzzle();

        // Act
        sut.Execute(puzzle);

        // Assert
        puzzle.GetCell(0, 0).PossibleValues.Should().BeSubsetOf([7, 8, 9]);
        puzzle.GetCell(1, 0).PossibleValues.Should().BeSubsetOf([7, 8, 9]);
        puzzle.GetCell(2, 0).PossibleValues.Should().BeSubsetOf([7, 8, 9]);
    }

    [Fact]
    public void Execute_WithMultipleColumnsWithTriplets_ProcessesAllColumns()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreatePuzzleWithTripletsInMultipleColumns();

        // Act
        sut.Execute(puzzle);

        // Assert
        // Column 0 triplets
        puzzle.GetCell(0, 0).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(1, 0).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(2, 0).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        
        // Column 1 triplets
        puzzle.GetCell(0, 1).PossibleValues.Should().BeEquivalentTo([4, 5, 6]);
        puzzle.GetCell(1, 1).PossibleValues.Should().BeEquivalentTo([4, 5, 6]);
        puzzle.GetCell(2, 1).PossibleValues.Should().BeEquivalentTo([4, 5, 6]);
    }

    [Fact]
    public void Execute_WithNakedTripletsInColumn_RemovesTripletValuesFromOtherCells()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateNakedTripletsTestPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        // Cells at (3,0), (4,0), (5,0) should remain as naked triplets with values [1,2,3]
        puzzle.GetCell(3, 0).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(4, 0).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(5, 0).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        
        // Other cells in column should have triplet values removed
        puzzle.GetCell(6, 0).PossibleValues.Should().NotContain([1, 2, 3]);
        puzzle.GetCell(7, 0).PossibleValues.Should().NotContain([1, 2, 3]);
    }

    [Fact]
    public void Execute_WithHiddenTripletsInColumn_SetsTripletsValues()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateHiddenTripletsWithSingleValueTestPuzzle();

        // Act
        sut.Execute(puzzle);

        // Assert
        puzzle.GetCell(1, 2).Value.Should().Be(2);
        puzzle.GetCell(3, 2).Value.Should().Be(6);
        puzzle.GetCell(4, 2).Value.Should().Be(9);
    }

    [Fact]
    public void Execute_WithNoTripletsInAnyColumn_ReturnsFalse()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreatePuzzleWithoutTriplets();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeFalse();
    }

    private SudokuPuzzle CreateNakedTripletsTestPuzzle()
    {
        var cells = new List<Cell>();
        
        // Create 9x9 grid
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                cells.Add(Cell.CreateEmpty(row, col));
            }
        }
        
        var puzzle = SudokuPuzzle.Create("test", GameDifficulty.Easy, cells);

        puzzle.PopulatePossibleValues();

        // Set up naked triplets in column 0: cells (3,0), (4,0), (5,0) with values [1,2,3]
        puzzle.GetCell(3, 0).PossibleValues.Clear();
        puzzle.GetCell(4, 0).PossibleValues.Clear();
        puzzle.GetCell(5, 0).PossibleValues.Clear();
        puzzle.GetCell(3, 0).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(4, 0).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(5, 0).PossibleValues.UnionWith([1, 2, 3]);

        // Other cells in column 0 have overlapping values that should be removed
        puzzle.GetCell(6, 0).PossibleValues.Clear();
        puzzle.GetCell(7, 0).PossibleValues.Clear();
        puzzle.GetCell(8, 0).PossibleValues.Clear();
        puzzle.GetCell(6, 0).PossibleValues.UnionWith([1, 4, 5]);
        puzzle.GetCell(7, 0).PossibleValues.UnionWith([2, 6, 7]);
        puzzle.GetCell(8, 0).PossibleValues.UnionWith([3, 8, 9]);

        return puzzle;
    }

    private SudokuPuzzle CreateNakedTripletsWithSingleValueTestPuzzle()
    {

        int?[,] cells =
        {
            { null, 2, null, 4, 5, 6, 7, 8, 9 },
            { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
            { 7, 8, 9, 1, 2, 3, 4, 5, 6 },
            { null, 3, null, 6, 7, 4, 8, 9, 5 },
            { 8, 7, 5, 9, 1, 2, 3, 6, 4 },
            { 6, 9, 4, 5, 3, 8, 2, 1, 7 },
            { null, 1, 7, null, 6, 5, 9, 4, 8 },
            { 5, 4, null, 8, 9, 7, 6, 3, 1 },
            { 9, 6, 8, 3, 4, 1, 5, 7, 2 }
        };

        var puzzle = PuzzleFactory.PopulateCells(cells);

        puzzle.PopulatePossibleValues();

        return puzzle;
    }

    private SudokuPuzzle CreateHiddenTripletsTestPuzzle()
    {
        var cells = new List<Cell>();
        
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                cells.Add(Cell.CreateEmpty(row, col));
            }
        }
        
        var puzzle = SudokuPuzzle.Create("test", GameDifficulty.Easy, cells);
        
        // Hidden triplets: numbers 7,8,9 appear only in cells (0,0), (1,0), (2,0)
        puzzle.GetCell(0, 0).PossibleValues.UnionWith([1, 4, 7, 8]);
        puzzle.GetCell(1, 0).PossibleValues.UnionWith([2, 5, 8, 9]);
        puzzle.GetCell(2, 0).PossibleValues.UnionWith([3, 6, 7, 9]);
        
        // Other cells in column don't have 7,8,9
        puzzle.GetCell(3, 0).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);
        puzzle.GetCell(4, 0).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);
        puzzle.GetCell(5, 0).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);
        puzzle.GetCell(6, 0).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);
        puzzle.GetCell(7, 0).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);
        puzzle.GetCell(8, 0).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);

        return puzzle;
    }

    private SudokuPuzzle CreateHiddenTripletsWithSingleValueTestPuzzle()
    {
        int?[,] cells = new int?[9, 9]
        {
            { 5, null, null, 8, null, null, 2, null, null },
            { null, null, 2, null, null, 6, null, null, 7 },
            { null, null, null, null, 3, null, 5, null, null },
            { null, 4, 6, null, null, null, null, 3, null },
            { null, null, 9, null, 7, null, null, null, null },
            { null, null, null, null, null, 1, 9, 6, null },
            { null, 9, null, 3, null, null, null, null, null },
            { 6, null, null, null, null, null, null, 4, null },
            { null, null, null, null, 2, null, null, null, 1 }
        };

        var puzzle = PuzzleFactory.PopulateCells(cells);

        puzzle.PopulatePossibleValues();
        
        return puzzle;
    }

    private SudokuPuzzle CreatePuzzleWithoutTriplets()
    {
        var cells = new List<Cell>();
        
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                cells.Add(Cell.CreateEmpty(row, col));
            }
        }
        
        var puzzle = SudokuPuzzle.Create("test", GameDifficulty.Easy, cells);
        
        // No triplets - just random possible values
        puzzle.GetCell(0, 0).PossibleValues.UnionWith([1, 2]);
        puzzle.GetCell(1, 0).PossibleValues.UnionWith([3, 4]);
        puzzle.GetCell(2, 0).PossibleValues.UnionWith([5, 6]);

        puzzle.PopulatePossibleValues();

        return puzzle;
    }

    private SudokuPuzzle CreateEmptyPuzzle()
    {
        var cells = new List<Cell>();
        
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                cells.Add(Cell.CreateEmpty(row, col));
            }
        }
        
        return SudokuPuzzle.Create("test", GameDifficulty.Easy, cells);
    }

    private SudokuPuzzle CreatePuzzleWithSomeCellsSet()
    {
        var cells = new List<Cell>();
        
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                cells.Add(Cell.CreateEmpty(row, col));
            }
        }
        
        var puzzle = SudokuPuzzle.Create("test", GameDifficulty.Easy, cells);
        
        // Set some cells
        puzzle.GetCell(0, 0).SetValue(5);
        puzzle.GetCell(1, 0).SetValue(6);
        
        // Add triplets in other cells
        puzzle.GetCell(3, 0).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(4, 0).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(5, 0).PossibleValues.UnionWith([1, 2, 3]);

        puzzle.PopulatePossibleValues();
        
        return puzzle;
    }

    private SudokuPuzzle CreateInvalidTripletsPuzzle()
    {
        var cells = new List<Cell>();
        
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                cells.Add(Cell.CreateEmpty(row, col));
            }
        }
        
        var puzzle = SudokuPuzzle.Create("test", GameDifficulty.Easy, cells);
        
        // Naked triplets
        puzzle.GetCell(3, 0).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(4, 0).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(5, 0).PossibleValues.UnionWith([1, 2, 3]);
        
        // Cell that will become empty after triplet removal
        puzzle.GetCell(6, 0).PossibleValues.UnionWith([1, 2, 3]); // Will become empty after removal

        return puzzle;
    }

    private SudokuPuzzle CreatePuzzleWithTripletsInMultipleColumns()
    {
        var cells = new List<Cell>();
        
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                cells.Add(Cell.CreateEmpty(row, col));
            }
        }
        
        var puzzle = SudokuPuzzle.Create("test", GameDifficulty.Easy, cells);

        puzzle.PopulatePossibleValues();

        // Triplets in column 0
        puzzle.GetCell(0, 0).PossibleValues.Clear();
        puzzle.GetCell(1, 0).PossibleValues.Clear();
        puzzle.GetCell(2, 0).PossibleValues.Clear();
        puzzle.GetCell(0, 0).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(1, 0).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(2, 0).PossibleValues.UnionWith([1, 2, 3]);

        // Triplets in column 1
        puzzle.GetCell(0, 1).PossibleValues.Clear();
        puzzle.GetCell(1, 1).PossibleValues.Clear();
        puzzle.GetCell(2, 1).PossibleValues.Clear();
        puzzle.GetCell(0, 1).PossibleValues.UnionWith([4, 5, 6]);
        puzzle.GetCell(1, 1).PossibleValues.UnionWith([4, 5, 6]);
        puzzle.GetCell(2, 1).PossibleValues.UnionWith([4, 5, 6]);

        // Other cells to be affected
        puzzle.GetCell(3, 0).PossibleValues.Clear();
        puzzle.GetCell(3, 1).PossibleValues.Clear();
        puzzle.GetCell(3, 0).PossibleValues.UnionWith([1, 7, 8]);
        puzzle.GetCell(3, 1).PossibleValues.UnionWith([4, 7, 8]);

        return puzzle;
    }
}