using DepenMock.XUnit;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services.Strategies;

namespace UnitTests.Infrastructure.Services.Strategies;

public class TripletsInRowsStrategyTests : BaseTestByAbstraction<TripletsInRowsStrategy, SolverStrategy>
{
    [Fact]
    public void Execute_WhenRemovalLeadsToEmptyPossibleValues_ThrowsInvalidMoveException()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateInvalidTripletsPuzzle();

        // Act & Assert
        var action = () => sut.Execute(puzzle);
        action.Should().Throw<InvalidMoveException>().WithMessage("Invalid move at row:0, col:3");
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
        puzzle.GetCell(0, 1).Value.Should().Be(6); // Pre-set value should remain
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
        puzzle.GetCell(0, 3).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(0, 4).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(0, 6).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public void Execute_WithHiddenTripletsInRow_RestrictsCellsToTripletValues()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateHiddenTripletsTestPuzzle();

        // Act
        sut.Execute(puzzle);

        // Assert
        puzzle.GetCell(0, 0).PossibleValues.Should().BeSubsetOf([7, 8, 9]);
        puzzle.GetCell(0, 1).PossibleValues.Should().BeSubsetOf([7, 8, 9]);
        puzzle.GetCell(0, 2).PossibleValues.Should().BeSubsetOf([7, 8, 9]);
    }

    [Fact]
    public void Execute_WithMultipleRowsWithTriplets_ProcessesAllRows()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreatePuzzleWithTripletsInMultipleRows();

        // Act
        sut.Execute(puzzle);

        // Assert
        // Row 0 triplets
        puzzle.GetCell(0, 0).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(0, 1).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(0, 2).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        
        // Row 1 triplets
        puzzle.GetCell(1, 0).PossibleValues.Should().BeEquivalentTo([4, 5, 6]);
        puzzle.GetCell(1, 1).PossibleValues.Should().BeEquivalentTo([4, 5, 6]);
        puzzle.GetCell(1, 2).PossibleValues.Should().BeEquivalentTo([4, 5, 6]);
    }

    [Fact]
    public void Execute_WithNakedTripletsInRow_RemovesTripletValuesFromOtherCells()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateNakedTripletsTestPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        // Cells at (0,3), (0,4), (0,5) should remain as naked triplets with values [1,2,3]
        puzzle.GetCell(0, 3).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(0, 4).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(0, 5).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        
        // Other cells in row should have triplet values removed
        puzzle.GetCell(0, 6).PossibleValues.Should().NotContain([1, 2, 3]);
        puzzle.GetCell(0, 7).PossibleValues.Should().NotContain([1, 2, 3]);
        
        result.Should().BeTrue();
    }

    [Fact]
    public void Execute_WithHiddenTripletsInRow_SetsTripletsValues()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateHiddenTripletsWithSingleValueTestPuzzle();

        // Act
        sut.Execute(puzzle);

        // Assert
        puzzle.GetCell(2, 1).Value.Should().Be(2);
        puzzle.GetCell(2, 3).Value.Should().Be(6);
        puzzle.GetCell(2, 4).Value.Should().Be(1);
    }

    [Fact]
    public void Execute_WithNoTripletsInAnyRow_ReturnsFalse()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreatePuzzleWithoutTriplets();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Execute_IteratesThroughAllNineRows()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreatePuzzleWithTripletsInLastRow();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        // Verify triplets were processed in row 8
        puzzle.GetCell(8, 0).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(8, 1).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(8, 2).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public void Execute_OnlyProcessesEmptyCells()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreatePuzzleWithMixedFilledAndEmptyCells();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        // Filled cells should remain unchanged
        puzzle.GetCell(0, 0).Value.Should().Be(9);
        puzzle.GetCell(0, 1).Value.Should().Be(8);
        puzzle.GetCell(0, 2).Value.Should().Be(7);
        
        // Only empty cells should have been processed for triplets
        puzzle.GetCell(0, 3).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(0, 4).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        puzzle.GetCell(0, 5).PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
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

        // Set up naked triplets in row 0: cells (0,3), (0,4), (0,5) with values [1,2,3]
        puzzle.GetCell(0, 3).PossibleValues.Clear();
        puzzle.GetCell(0, 4).PossibleValues.Clear();
        puzzle.GetCell(0, 5).PossibleValues.Clear();
        puzzle.GetCell(0, 3).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(0, 4).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(0, 5).PossibleValues.UnionWith([1, 2, 3]);

        // Other cells in row 0 have overlapping values that should be removed
        puzzle.GetCell(0, 6).PossibleValues.Clear();
        puzzle.GetCell(0, 7).PossibleValues.Clear();
        puzzle.GetCell(0, 8).PossibleValues.Clear();
        puzzle.GetCell(0, 6).PossibleValues.UnionWith([1, 4, 5]);
        puzzle.GetCell(0, 7).PossibleValues.UnionWith([2, 6, 7]);
        puzzle.GetCell(0, 8).PossibleValues.UnionWith([3, 8, 9]);

        return puzzle;
    }

    private SudokuPuzzle CreateNakedTripletsWithSingleValueTestPuzzle()
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

        // Manually set up specific triplet scenario for testing
        // Set up naked triplets: cells (0,3), (0,4), (0,6) with values [1,2,3]
        puzzle.GetCell(0, 3).PossibleValues.Clear();
        puzzle.GetCell(0, 4).PossibleValues.Clear();
        puzzle.GetCell(0, 6).PossibleValues.Clear();
        puzzle.GetCell(0, 3).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(0, 4).PossibleValues.UnionWith([1, 2, 3]);  
        puzzle.GetCell(0, 6).PossibleValues.UnionWith([1, 2, 3]);

        // Set up cells that will be affected by triplet removal
        puzzle.GetCell(0, 0).PossibleValues.Clear();
        puzzle.GetCell(0, 2).PossibleValues.Clear();
        puzzle.GetCell(0, 0).PossibleValues.UnionWith([1, 4, 5]);   // Should have 1 removed, leaving [4,5]
        puzzle.GetCell(0, 2).PossibleValues.UnionWith([2, 7, 8]);   // Should have 2 removed, leaving [7,8]

        // Add some other cells that won't be affected
        puzzle.GetCell(0, 1).PossibleValues.Clear();
        puzzle.GetCell(0, 5).PossibleValues.Clear();
        puzzle.GetCell(0, 1).PossibleValues.UnionWith([6, 7, 8]);   // No overlap with triplet
        puzzle.GetCell(0, 5).PossibleValues.UnionWith([9]);         // Single value, won't be affected

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
        
        // Hidden triplets: numbers 7,8,9 appear only in cells (0,0), (0,1), (0,2)
        puzzle.GetCell(0, 0).PossibleValues.UnionWith([1, 4, 7, 8]);
        puzzle.GetCell(0, 1).PossibleValues.UnionWith([2, 5, 8, 9]);
        puzzle.GetCell(0, 2).PossibleValues.UnionWith([3, 6, 7, 9]);
        
        // Other cells in row don't have 7,8,9
        puzzle.GetCell(0, 3).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);
        puzzle.GetCell(0, 4).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);
        puzzle.GetCell(0, 5).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);
        puzzle.GetCell(0, 6).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);
        puzzle.GetCell(0, 7).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);
        puzzle.GetCell(0, 8).PossibleValues.UnionWith([1, 2, 3, 4, 5, 6]);

        return puzzle;
    }

    private SudokuPuzzle CreateHiddenTripletsWithSingleValueTestPuzzle()
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

        // Create a scenario where hidden triplets 2, 6, 9 appear only in cells (2,1), (2,3), (2,4)
        // but each cell also has other values that should be removed
        
        // Set up row 2 with a specific pattern
        puzzle.GetCell(2, 0).SetValue(1);
        puzzle.GetCell(2, 2).SetValue(3);
        puzzle.GetCell(2, 5).SetValue(4);
        puzzle.GetCell(2, 6).SetValue(5);
        puzzle.GetCell(2, 7).SetValue(7);
        puzzle.GetCell(2, 8).SetValue(8);

        // Populate possible values
        puzzle.PopulatePossibleValues();

        // Override possible values to create hidden triplets scenario
        // Cells (2,1), (2,3), (2,4) are the only ones that can contain 2, 6, 9
        puzzle.GetCell(2, 1).PossibleValues.Clear();
        puzzle.GetCell(2, 3).PossibleValues.Clear();
        puzzle.GetCell(2, 4).PossibleValues.Clear();

        // Set up the hidden triplets - each cell has the triplet values plus some others
        puzzle.GetCell(2, 1).PossibleValues.UnionWith([2, 5, 8]);    // Will be reduced to just 2
        puzzle.GetCell(2, 3).PossibleValues.UnionWith([6, 7, 9]);    // Will be reduced to just 6  
        puzzle.GetCell(2, 4).PossibleValues.UnionWith([9, 1, 3]);    // Will be reduced to just 9

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
        puzzle.GetCell(0, 1).PossibleValues.UnionWith([3, 4]);
        puzzle.GetCell(0, 2).PossibleValues.UnionWith([5, 6]);

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
        
        // Set some cells in row 0
        puzzle.GetCell(0, 0).SetValue(5);
        puzzle.GetCell(0, 1).SetValue(6);
        
        // Add triplets in other cells of row 0
        puzzle.GetCell(0, 3).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(0, 4).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(0, 5).PossibleValues.UnionWith([1, 2, 3]);

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
        
        // Create a scenario where triplet removal will cause an invalid state
        // Set up naked triplets in row 0: cells (0,0), (0,1), (0,3) with values [1,2,3]
        puzzle.GetCell(0, 0).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(0, 1).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(0, 3).PossibleValues.UnionWith([1, 2, 3]);
        
        // Cell that will become empty after triplet removal - only has triplet values
        puzzle.GetCell(0, 2).PossibleValues.UnionWith([1, 2, 3]); // Will become empty after removal

        return puzzle;
    }

    private SudokuPuzzle CreatePuzzleWithTripletsInMultipleRows()
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

        // Triplets in row 0
        puzzle.GetCell(0, 0).PossibleValues.Clear();
        puzzle.GetCell(0, 1).PossibleValues.Clear();
        puzzle.GetCell(0, 2).PossibleValues.Clear();
        puzzle.GetCell(0, 0).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(0, 1).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(0, 2).PossibleValues.UnionWith([1, 2, 3]);

        // Triplets in row 1
        puzzle.GetCell(1, 0).PossibleValues.Clear();
        puzzle.GetCell(1, 1).PossibleValues.Clear();
        puzzle.GetCell(1, 2).PossibleValues.Clear();
        puzzle.GetCell(1, 0).PossibleValues.UnionWith([4, 5, 6]);
        puzzle.GetCell(1, 1).PossibleValues.UnionWith([4, 5, 6]);
        puzzle.GetCell(1, 2).PossibleValues.UnionWith([4, 5, 6]);

        // Other cells to be affected
        puzzle.GetCell(0, 3).PossibleValues.Clear();
        puzzle.GetCell(1, 3).PossibleValues.Clear();
        puzzle.GetCell(0, 3).PossibleValues.UnionWith([1, 7, 8]);
        puzzle.GetCell(1, 3).PossibleValues.UnionWith([4, 7, 8]);

        return puzzle;
    }

    private SudokuPuzzle CreatePuzzleWithTripletsInLastRow()
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

        // Triplets in row 8 (last row)
        puzzle.GetCell(8, 0).PossibleValues.Clear();
        puzzle.GetCell(8, 1).PossibleValues.Clear();
        puzzle.GetCell(8, 2).PossibleValues.Clear();
        puzzle.GetCell(8, 0).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(8, 1).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(8, 2).PossibleValues.UnionWith([1, 2, 3]);

        // Other cells to be affected
        puzzle.GetCell(8, 3).PossibleValues.Clear();
        puzzle.GetCell(8, 3).PossibleValues.UnionWith([1, 7, 8]);

        return puzzle;
    }

    private SudokuPuzzle CreatePuzzleWithMixedFilledAndEmptyCells()
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

        // Set some cells with values in row 0
        puzzle.GetCell(0, 0).SetValue(9);
        puzzle.GetCell(0, 1).SetValue(8);
        puzzle.GetCell(0, 2).SetValue(7);

        // Populate possible values for empty cells
        puzzle.PopulatePossibleValues();

        // Override specific cells to create triplets in empty cells of row 0
        // Only process empty cells, so cells 3, 4, 5 will form triplets
        puzzle.GetCell(0, 3).PossibleValues.Clear();
        puzzle.GetCell(0, 4).PossibleValues.Clear();
        puzzle.GetCell(0, 5).PossibleValues.Clear();
        puzzle.GetCell(0, 3).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(0, 4).PossibleValues.UnionWith([1, 2, 3]);
        puzzle.GetCell(0, 5).PossibleValues.UnionWith([1, 2, 3]);

        // Add some other empty cells that won't form triplets but won't cause issues
        puzzle.GetCell(0, 6).PossibleValues.Clear();
        puzzle.GetCell(0, 6).PossibleValues.UnionWith([4, 5, 6]);

        return puzzle;
    }
}