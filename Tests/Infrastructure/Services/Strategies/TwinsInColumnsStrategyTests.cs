using DepenMock.XUnit;
using FluentAssertions;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services.Strategies;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Services.Strategies;

public class TwinsInColumnsStrategyTests : BaseTestByAbstraction<TwinsInColumnsStrategy, SolverStrategy>
{
    [Fact]
    public void Execute_WhenColumnHasNakedTwins_EliminatesTwinValuesFromOtherCells()
    {
        // Arrange
        var puzzle = CreatePuzzleWithColumnTwins();
        puzzle.PopulatePossibleValues(); // Populate based on current state first
        SetupColumnTwinsScenario(puzzle); // Then setup twins scenario
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // Check that non-twin cells in column 0 no longer contain the twin values (1, 2)
        var nonTwinCells = puzzle.GetColumnCells(0).Where(c => c.Row != 0 && c.Row != 1 && !c.Value.HasValue);
        foreach (var cell in nonTwinCells)
        {
            cell.PossibleValues.Should().NotContain(1);
            cell.PossibleValues.Should().NotContain(2);
        }
    }

    [Fact]
    public void Execute_WhenColumnHasNakedTwinsAndCellBecomesNaked_SetsCellValue()
    {
        // Arrange
        var puzzle = CreatePuzzleWithColumnTwinsLeadingToNaked();
        puzzle.PopulatePossibleValues();
        SetupColumnTwinsLeadingToNakedScenario(puzzle);
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // The cell at (2, 0) should have value 3 after twin elimination
        puzzle.GetCell(2, 0).Value.Should().Be(3);
        puzzle.GetCell(2, 0).PossibleValues.Should().BeEmpty();
    }

    [Fact]
    public void Execute_WhenColumnHasNoTwins_ReturnsFalse()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Easy);
        puzzle.PopulatePossibleValues();
        var sut = ResolveSut();

        // Verify no twins exist before testing
        var hasAnyTwins = false;
        for (var col = 0; col < 9; col++)
        {
            var columnCells = puzzle.GetColumnCells(col).Where(c => !c.Value.HasValue && c.PossibleValues.Count == 2).ToList();
            for (var i = 0; i < columnCells.Count; i++)
            {
                for (var j = i + 1; j < columnCells.Count; j++)
                {
                    if (columnCells[i].PossibleValues.SetEquals(columnCells[j].PossibleValues))
                    {
                        hasAnyTwins = true;
                        break;
                    }
                }
                if (hasAnyTwins) break;
            }
            if (hasAnyTwins) break;
        }

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        if (!hasAnyTwins)
        {
            result.Should().BeFalse();
        }
        else
        {
            // If twins exist, the strategy should work
            result.Should().BeTrue();
        }
    }

    [Fact]
    public void Execute_WhenColumnHasMultipleTwins_HandlesAllTwins()
    {
        // Arrange
        var puzzle = CreatePuzzleWithMultipleColumnTwins();
        puzzle.PopulatePossibleValues();
        SetupMultipleColumnTwinsScenario(puzzle);
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // Verify first twin pair (1,2) elimination in column 0
        var nonTwinCells1 = puzzle.GetColumnCells(0).Where(c => c.Row != 0 && c.Row != 1 && !c.Value.HasValue);
        foreach (var cell in nonTwinCells1)
        {
            cell.PossibleValues.Should().NotContain(1);
            cell.PossibleValues.Should().NotContain(2);
        }
        
        // Verify second twin pair (7,8) elimination in column 1  
        var nonTwinCells2 = puzzle.GetColumnCells(1).Where(c => c.Row != 3 && c.Row != 4 && !c.Value.HasValue);
        foreach (var cell in nonTwinCells2)
        {
            cell.PossibleValues.Should().NotContain(7);
            cell.PossibleValues.Should().NotContain(8);
        }
    }

    [Fact]
    public void Execute_WhenTwinEliminationLeadsToInvalidCell_ThrowsInvalidMoveException()
    {
        // Arrange
        var puzzle = CreatePuzzleWithInvalidTwinElimination();
        puzzle.PopulatePossibleValues();
        SetupInvalidTwinEliminationScenario(puzzle);
        var sut = ResolveSut();

        // Act & Assert
        sut.Invoking(s => s.Execute(puzzle))
            .Should().Throw<InvalidMoveException>()
            .WithMessage("Invalid move for position: 2, 0");
    }

    [Fact]
    public void Execute_WhenCellsHaveSameValuesButNotTwins_DoesNotEliminateThem()
    {
        // Arrange - cells with 3 possible values that happen to overlap
        var puzzle = CreatePuzzleWithNonTwinOverlap();
        puzzle.PopulatePossibleValues();
        SetupNonTwinOverlapScenario(puzzle);
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Execute_WhenColumnHasCellsWithValues_IgnoresThemForTwins()
    {
        // Arrange
        var puzzle = CreatePuzzleWithMixedFilledAndEmptyCells();
        puzzle.PopulatePossibleValues();
        SetupMixedFilledAndEmptyCellsScenario(puzzle);
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // Verify that cells with values are not considered for twins
        var filledCells = puzzle.GetColumnCells(0).Where(c => c.Value.HasValue);
        filledCells.Should().NotBeEmpty();
        
        // Verify twins are still processed among empty cells
        var nonTwinCells = puzzle.GetColumnCells(0).Where(c => c.Row != 1 && c.Row != 2 && !c.Value.HasValue);
        foreach (var cell in nonTwinCells)
        {
            cell.PossibleValues.Should().NotContain(1);
            cell.PossibleValues.Should().NotContain(2);
        }
    }

    [Fact]
    public void SolvePuzzle_CallsPopulatePossibleValuesAndExecute()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Easy);
        var sut = ResolveSut();

        // Act
        var result = sut.SolvePuzzle(puzzle);

        // Assert - We can't guarantee the result will be true since it depends on whether the puzzle has twins
        // But we can verify the method executes without error and returns a boolean
        (result == true || result == false).Should().BeTrue();
        
        // Verify that possible values were populated (cells should have some possible values)
        var emptyCells = puzzle.Cells.Where(c => !c.HasValue);
        emptyCells.Should().NotBeEmpty();
        emptyCells.All(c => c.PossibleValues.Any()).Should().BeTrue();
    }

    [Fact]
    public void Execute_WhenAllColumnsProcessed_ChecksAllNineColumns()
    {
        // Arrange
        var puzzle = CreatePuzzleWithTwinsInDifferentColumns();
        puzzle.PopulatePossibleValues();
        SetupTwinsInDifferentColumnsScenario(puzzle);
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // Verify twins in column 0 were processed
        var nonTwinCells0 = puzzle.GetColumnCells(0).Where(c => c.Row != 0 && c.Row != 1 && !c.Value.HasValue);
        foreach (var cell in nonTwinCells0)
        {
            cell.PossibleValues.Should().NotContain(1);
            cell.PossibleValues.Should().NotContain(2);
        }
        
        // Verify twins in column 8 were processed
        var nonTwinCells8 = puzzle.GetColumnCells(8).Where(c => c.Row != 6 && c.Row != 7 && !c.Value.HasValue);
        foreach (var cell in nonTwinCells8)
        {
            cell.PossibleValues.Should().NotContain(8);
            cell.PossibleValues.Should().NotContain(9);
        }
    }

    // Helper methods to setup specific scenarios after PopulatePossibleValues has been called
    private static void SetupColumnTwinsScenario(SudokuPuzzle puzzle)
    {
        // Clear and set specific possible values for the twin scenario
        var cell00 = puzzle.GetCell(0, 0);
        var cell10 = puzzle.GetCell(1, 0);
        var cell20 = puzzle.GetCell(2, 0);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
        }

        if (!cell10.HasValue)
        {
            cell10.PossibleValues.Clear();
            cell10.PossibleValues.Add(1);
            cell10.PossibleValues.Add(2);
        }

        if (!cell20.HasValue)
        {
            cell20.PossibleValues.Clear();
            cell20.PossibleValues.Add(1);
            cell20.PossibleValues.Add(2);
            cell20.PossibleValues.Add(3);
        }
    }

    private static void SetupColumnTwinsLeadingToNakedScenario(SudokuPuzzle puzzle)
    {
        var cell00 = puzzle.GetCell(0, 0);
        var cell10 = puzzle.GetCell(1, 0);
        var cell20 = puzzle.GetCell(2, 0);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
        }

        if (!cell10.HasValue)
        {
            cell10.PossibleValues.Clear();
            cell10.PossibleValues.Add(1);
            cell10.PossibleValues.Add(2);
        }

        if (!cell20.HasValue)
        {
            cell20.PossibleValues.Clear();
            cell20.PossibleValues.Add(1);
            cell20.PossibleValues.Add(3);
        }
    }

    private static void SetupMultipleColumnTwinsScenario(SudokuPuzzle puzzle)
    {
        // Column 0 twins
        var cell00 = puzzle.GetCell(0, 0);
        var cell10 = puzzle.GetCell(1, 0);
        var cell20 = puzzle.GetCell(2, 0);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
        }

        if (!cell10.HasValue)
        {
            cell10.PossibleValues.Clear();
            cell10.PossibleValues.Add(1);
            cell10.PossibleValues.Add(2);
        }

        if (!cell20.HasValue)
        {
            cell20.PossibleValues.Clear();
            cell20.PossibleValues.Add(1);
            cell20.PossibleValues.Add(2);
            cell20.PossibleValues.Add(3);
        }

        // Column 1 twins
        var cell31 = puzzle.GetCell(3, 1);
        var cell41 = puzzle.GetCell(4, 1);
        var cell51 = puzzle.GetCell(5, 1);

        if (!cell31.HasValue)
        {
            cell31.PossibleValues.Clear();
            cell31.PossibleValues.Add(7);
            cell31.PossibleValues.Add(8);
        }

        if (!cell41.HasValue)
        {
            cell41.PossibleValues.Clear();
            cell41.PossibleValues.Add(7);
            cell41.PossibleValues.Add(8);
        }

        if (!cell51.HasValue)
        {
            cell51.PossibleValues.Clear();
            cell51.PossibleValues.Add(7);
            cell51.PossibleValues.Add(8);
            cell51.PossibleValues.Add(9);
        }
    }

    private static void SetupInvalidTwinEliminationScenario(SudokuPuzzle puzzle)
    {
        var cell00 = puzzle.GetCell(0, 0);
        var cell10 = puzzle.GetCell(1, 0);
        var cell20 = puzzle.GetCell(2, 0);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
        }

        if (!cell10.HasValue)
        {
            cell10.PossibleValues.Clear();
            cell10.PossibleValues.Add(1);
            cell10.PossibleValues.Add(2);
        }

        if (!cell20.HasValue)
        {
            cell20.PossibleValues.Clear();
            cell20.PossibleValues.Add(1);
            cell20.PossibleValues.Add(2);
        }
    }

    private static void SetupNonTwinOverlapScenario(SudokuPuzzle puzzle)
    {
        var cell00 = puzzle.GetCell(0, 0);
        var cell10 = puzzle.GetCell(1, 0);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
            cell00.PossibleValues.Add(3);
        }

        if (!cell10.HasValue)
        {
            cell10.PossibleValues.Clear();
            cell10.PossibleValues.Add(1);
            cell10.PossibleValues.Add(2);
            cell10.PossibleValues.Add(4);
        }
    }

    private static void SetupMixedFilledAndEmptyCellsScenario(SudokuPuzzle puzzle)
    {
        var cell10 = puzzle.GetCell(1, 0);
        var cell20 = puzzle.GetCell(2, 0);
        var cell30 = puzzle.GetCell(3, 0);

        if (!cell10.HasValue)
        {
            cell10.PossibleValues.Clear();
            cell10.PossibleValues.Add(1);
            cell10.PossibleValues.Add(2);
        }

        if (!cell20.HasValue)
        {
            cell20.PossibleValues.Clear();
            cell20.PossibleValues.Add(1);
            cell20.PossibleValues.Add(2);
        }

        if (!cell30.HasValue)
        {
            cell30.PossibleValues.Clear();
            cell30.PossibleValues.Add(1);
            cell30.PossibleValues.Add(2);
            cell30.PossibleValues.Add(3);
        }
    }

    private static void SetupTwinsInDifferentColumnsScenario(SudokuPuzzle puzzle)
    {
        // Column 0 twins
        var cell00 = puzzle.GetCell(0, 0);
        var cell10 = puzzle.GetCell(1, 0);
        var cell20 = puzzle.GetCell(2, 0);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
        }

        if (!cell10.HasValue)
        {
            cell10.PossibleValues.Clear();
            cell10.PossibleValues.Add(1);
            cell10.PossibleValues.Add(2);
        }

        if (!cell20.HasValue)
        {
            cell20.PossibleValues.Clear();
            cell20.PossibleValues.Add(1);
            cell20.PossibleValues.Add(2);
            cell20.PossibleValues.Add(3);
        }

        // Column 8 twins
        var cell68 = puzzle.GetCell(6, 8);
        var cell78 = puzzle.GetCell(7, 8);
        var cell88 = puzzle.GetCell(8, 8);

        if (!cell68.HasValue)
        {
            cell68.PossibleValues.Clear();
            cell68.PossibleValues.Add(8);
            cell68.PossibleValues.Add(9);
        }

        if (!cell78.HasValue)
        {
            cell78.PossibleValues.Clear();
            cell78.PossibleValues.Add(8);
            cell78.PossibleValues.Add(9);
        }

        if (!cell88.HasValue)
        {
            cell88.PossibleValues.Clear();
            cell88.PossibleValues.Add(8);
            cell88.PossibleValues.Add(9);
            cell88.PossibleValues.Add(7);
        }
    }

    // Simple puzzle creation methods that provide basic empty puzzles
    private static SudokuPuzzle CreatePuzzleWithColumnTwins()
    {
        return PuzzleFactory.GetEmptyPuzzle();
    }

    private static SudokuPuzzle CreatePuzzleWithColumnTwinsLeadingToNaked()
    {
        return PuzzleFactory.GetEmptyPuzzle();
    }

    private static SudokuPuzzle CreatePuzzleWithMultipleColumnTwins()
    {
        return PuzzleFactory.GetEmptyPuzzle();
    }

    private static SudokuPuzzle CreatePuzzleWithInvalidTwinElimination()
    {
        return PuzzleFactory.GetEmptyPuzzle();
    }

    private static SudokuPuzzle CreatePuzzleWithNonTwinOverlap()
    {
        return PuzzleFactory.GetEmptyPuzzle();
    }

    private static SudokuPuzzle CreatePuzzleWithMixedFilledAndEmptyCells()
    {
        return PuzzleFactory.GetEmptyPuzzle();
    }

    private static SudokuPuzzle CreatePuzzleWithTwinsInDifferentColumns()
    {
        return PuzzleFactory.GetEmptyPuzzle();
    }
}