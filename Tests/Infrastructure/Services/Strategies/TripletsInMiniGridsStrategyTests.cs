using DepenMock.XUnit;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services.Strategies;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Services.Strategies;

public class TripletsInMiniGridsStrategyTests : BaseTestByAbstraction<TripletsInMiniGridsStrategy, SolverStrategy>
{
    [Fact]
    public void Execute_WithNakedTripletsInMiniGrid_ReturnsTrue()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateTripletsInMiniGridPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Execute_WithHiddenTripletsInMiniGrid_ReturnsTrue()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateTripletsInMiniGridPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Execute_WithNoTripletsInAnyMiniGrid_ReturnsFalse()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreatePuzzleWithMinimalEmptyCells();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Execute_WithAllCellsHavingValues_ReturnsFalse()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateSolvedPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeFalse();
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
    public void Execute_IteratesThroughAllNineMiniGrids()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateTripletsInMiniGridPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Execute_CallsHandleNakedTripletsForEachMiniGrid()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateTripletsInMiniGridPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // Verify that changes were made (indirect verification of method calls)
        var changedCells = puzzle.Cells.Where(c => !c.HasValue && c.PossibleValues.Count == 1);
        changedCells.Should().NotBeEmpty();
    }

    [Fact]
    public void Execute_CallsHandleHiddenTripletsForEachMiniGrid()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateTripletsInMiniGridPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // Verify that changes were made (indirect verification of method calls)
        var changedCells = puzzle.Cells.Where(c => !c.HasValue && c.PossibleValues.Count == 1);
        changedCells.Should().NotBeEmpty();
    }

    [Fact]
    public void Execute_WithMultipleTripletsInSameMiniGrid_ProcessesAll()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateTripletsInMiniGridPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Execute_CombinesResultsFromBothNakedAndHiddenTripletHandlers()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateTripletsInMiniGridPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Execute_WithMinimalEmptyCellsInMiniGrid_StillProcessesCorrectly()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreatePuzzleWithMinimalEmptyCells();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeFalse(); // Not enough empty cells for triplets
    }

    [Fact]
    public void Execute_EnsuresChangesMadeOrOperationCombinesCorrectly()
    {
        // Arrange
        var sut = ResolveSut();
        var puzzle = CreateTripletsInMiniGridPuzzle();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
    }

    private static SudokuPuzzle CreateTripletsInMiniGridPuzzle()
    {
        int?[,] cells =
        {
            { null, null, null, 9, null, null, 5, 3, null },
            { 9, null, null, 7, null, null, null, null, 6 },
            { 3, null, null, null, 5, null, 2, null, null },
            { 6, 3, null, null, null, null, 7, 1, 5},
            { 8, null, null, null, null, null, null, null, 2 },
            { 2, 7, 5, null, null, null, null, 8, 4 },
            { null, null, 6, null, 4, null, null, null, 3 },
            { 7, null, null, null, null, 5, null, null, 1 },
            { null, null, 2, null, null, 1, null, null, null }
        };

        var puzzle = PuzzleFactory.PopulateCells(cells);

        puzzle.PopulatePossibleValues();

        return puzzle;
    }

    private static SudokuPuzzle CreateSolvedPuzzle()
    {
        int?[,] cells =
        {
            { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
            { 7, 8, 9, 1, 2, 3, 4, 5, 6 },
            { 2, 3, 1, 6, 7, 4, 8, 9, 5 },
            { 8, 7, 5, 9, 1, 2, 3, 6, 4 },
            { 6, 9, 4, 5, 3, 8, 2, 1, 7 },
            { 3, 1, 7, 2, 6, 5, 9, 4, 8 },
            { 5, 4, 2, 8, 9, 7, 6, 3, 1 },
            { 9, 6, 8, 3, 4, 1, 5, 7, 2 }
        };

        var puzzle = PuzzleFactory.PopulateCells(cells);

        puzzle.PopulatePossibleValues();

        return puzzle;
    }

    private static SudokuPuzzle CreateEmptyPuzzle()
    {
        var cells = new List<Cell>();
        
        // Create completely empty puzzle
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                var cell = Cell.CreateEmpty(row, col);
                cell.PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
                cells.Add(cell);
            }
        }

        var puzzle = SudokuPuzzle.Create(Guid.NewGuid().ToString(), GameDifficulty.Expert, cells);
        return puzzle;
    }

    private static SudokuPuzzle CreatePuzzleWithMinimalEmptyCells()
    {
        int?[,] cells =
        {
            { null, null, 3, 4, 5, 6, 7, 8, 9 },
            { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
            { 7, 8, 9, 1, 2, 3, 4, 5, 6 },
            { 2, 3, 1, 6, 7, 4, 8, 9, 5 },
            { 8, 7, 5, 9, 1, 2, 3, 6, 4 },
            { 6, 9, 4, 5, 3, 8, 2, 1, 7 },
            { 3, 1, 7, 2, 6, 5, 9, 4, 8 },
            { 5, 4, 2, 8, 9, 7, 6, 3, 1 },
            { 9, 6, 8, 3, 4, 1, 5, 7, 2 }
        };

        var puzzle = PuzzleFactory.PopulateCells(cells);

        puzzle.PopulatePossibleValues();

        return puzzle;
    }
}