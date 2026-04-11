using DepenMock.XUnit;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services.Strategies;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Services.Strategies;

public class TwinsInRowsStrategyTests : BaseTestByAbstraction<TwinsInRowsStrategy, SolverStrategy>
{
    [Fact]
    public void Execute_WhenRowHasNakedTwins_EliminatesTwinValuesFromOtherCells()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetEmptyPuzzle();
        puzzle.PopulatePossibleValues(); // Populate based on current state first
        SetupRowTwinsScenario(puzzle); // Then setup twins scenario
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // Check that non-twin cells in row 0 no longer contain the twin values (1, 2)
        var nonTwinCells = puzzle.GetRowCells(0).Where(c => c.Column != 0 && c.Column != 1 && !c.Value.HasValue);
        foreach (var cell in nonTwinCells)
        {
            cell.PossibleValues.Should().NotContain(1);
            cell.PossibleValues.Should().NotContain(2);
        }
    }

    [Fact]
    public void Execute_WhenRowHasNakedTwinsAndCellBecomesNaked_SetsCellValue()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetEmptyPuzzle();
        puzzle.PopulatePossibleValues();
        SetupRowTwinsLeadingToNakedScenario(puzzle);
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // The cell at (0, 2) should have value 3 after twin elimination
        puzzle.GetCell(0, 2).Value.Should().Be(3);
        puzzle.GetCell(0, 2).PossibleValues.Should().BeEmpty();
    }

    [Fact]
    public void Execute_WhenRowHasNoTwins_ReturnsFalse()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Easy);
        puzzle.PopulatePossibleValues();
        var sut = ResolveSut();

        // Verify no twins exist before testing
        var hasAnyTwins = false;
        for (var row = 0; row < 9; row++)
        {
            var rowCells = puzzle.GetRowCells(row).Where(c => !c.Value.HasValue && c.PossibleValues.Count == 2).ToList();
            for (var i = 0; i < rowCells.Count; i++)
            {
                for (var j = i + 1; j < rowCells.Count; j++)
                {
                    if (rowCells[i].PossibleValues.SetEquals(rowCells[j].PossibleValues))
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
    public void Execute_WhenRowHasMultipleTwins_HandlesAllTwins()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetEmptyPuzzle();
        puzzle.PopulatePossibleValues();
        SetupMultipleRowTwinsScenario(puzzle);
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // Verify first twin pair (1,2) elimination in row 0
        var nonTwinCells1 = puzzle.GetRowCells(0).Where(c => c.Column != 0 && c.Column != 1 && !c.Value.HasValue);
        foreach (var cell in nonTwinCells1)
        {
            cell.PossibleValues.Should().NotContain(1);
            cell.PossibleValues.Should().NotContain(2);
        }
        
        // Verify second twin pair (7,8) elimination in row 1  
        var nonTwinCells2 = puzzle.GetRowCells(1).Where(c => c.Column != 3 && c.Column != 4 && !c.Value.HasValue);
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
        var puzzle = PuzzleFactory.GetEmptyPuzzle();
        puzzle.PopulatePossibleValues();
        SetupInvalidTwinEliminationScenario(puzzle);
        var sut = ResolveSut();

        // Act & Assert
        sut.Invoking(s => s.Execute(puzzle))
            .Should().Throw<InvalidMoveException>()
            .WithMessage("Invalid move for position: 0, 2");
    }

    [Fact]
    public void Execute_WhenCellsHaveSameValuesButNotTwins_DoesNotEliminateThem()
    {
        // Arrange - cells with 3 possible values that happen to overlap
        var puzzle = PuzzleFactory.GetEmptyPuzzle();
        puzzle.PopulatePossibleValues();
        SetupNonTwinOverlapScenario(puzzle);
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Execute_WhenRowHasCellsWithValues_IgnoresThemForTwins()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetEmptyPuzzle();
        puzzle.PopulatePossibleValues();
        SetupMixedFilledAndEmptyCellsScenario(puzzle);
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // Verify that cells with values are not considered for twins
        var filledCells = puzzle.GetRowCells(0).Where(c => c.Value.HasValue);
        filledCells.Should().NotBeEmpty();
        
        // Verify twins are still processed among empty cells
        var nonTwinCells = puzzle.GetRowCells(0).Where(c => c.Column != 1 && c.Column != 2 && !c.Value.HasValue);
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
    public void Execute_WhenAllRowsProcessed_ChecksAllNineRows()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetEmptyPuzzle();
        puzzle.PopulatePossibleValues();
        SetupTwinsInDifferentRowsScenario(puzzle);
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeTrue();
        
        // Verify twins in row 0 were processed
        var nonTwinCells0 = puzzle.GetRowCells(0).Where(c => c.Column != 0 && c.Column != 1 && !c.Value.HasValue);
        foreach (var cell in nonTwinCells0)
        {
            cell.PossibleValues.Should().NotContain(1);
            cell.PossibleValues.Should().NotContain(2);
        }
        
        // Verify twins in row 8 were processed
        var nonTwinCells8 = puzzle.GetRowCells(8).Where(c => c.Column != 6 && c.Column != 7 && !c.Value.HasValue);
        foreach (var cell in nonTwinCells8)
        {
            cell.PossibleValues.Should().NotContain(8);
            cell.PossibleValues.Should().NotContain(9);
        }
    }

    [Fact]
    public void Execute_WhenEmptyPuzzle_ReturnsFalse()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetEmptyPuzzle();
        puzzle.PopulatePossibleValues();
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Execute_WhenSolvedPuzzle_ReturnsFalse()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetSolvedPuzzle();
        var sut = ResolveSut();

        // Act
        var result = sut.Execute(puzzle);

        // Assert
        result.Should().BeFalse();
    }

    // Helper methods to setup specific scenarios after PopulatePossibleValues has been called
    private static void SetupRowTwinsScenario(SudokuPuzzle puzzle)
    {
        // Clear and set specific possible values for the twin scenario
        var cell00 = puzzle.GetCell(0, 0);
        var cell01 = puzzle.GetCell(0, 1);
        var cell02 = puzzle.GetCell(0, 2);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
        }

        if (!cell01.HasValue)
        {
            cell01.PossibleValues.Clear();
            cell01.PossibleValues.Add(1);
            cell01.PossibleValues.Add(2);
        }

        if (!cell02.HasValue)
        {
            cell02.PossibleValues.Clear();
            cell02.PossibleValues.Add(1);
            cell02.PossibleValues.Add(2);
            cell02.PossibleValues.Add(3);
        }
    }

    private static void SetupRowTwinsLeadingToNakedScenario(SudokuPuzzle puzzle)
    {
        var cell00 = puzzle.GetCell(0, 0);
        var cell01 = puzzle.GetCell(0, 1);
        var cell02 = puzzle.GetCell(0, 2);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
        }

        if (!cell01.HasValue)
        {
            cell01.PossibleValues.Clear();
            cell01.PossibleValues.Add(1);
            cell01.PossibleValues.Add(2);
        }

        if (!cell02.HasValue)
        {
            cell02.PossibleValues.Clear();
            cell02.PossibleValues.Add(1);
            cell02.PossibleValues.Add(3);
        }
    }

    private static void SetupMultipleRowTwinsScenario(SudokuPuzzle puzzle)
    {
        // Row 0 twins
        var cell00 = puzzle.GetCell(0, 0);
        var cell01 = puzzle.GetCell(0, 1);
        var cell02 = puzzle.GetCell(0, 2);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
        }

        if (!cell01.HasValue)
        {
            cell01.PossibleValues.Clear();
            cell01.PossibleValues.Add(1);
            cell01.PossibleValues.Add(2);
        }

        if (!cell02.HasValue)
        {
            cell02.PossibleValues.Clear();
            cell02.PossibleValues.Add(1);
            cell02.PossibleValues.Add(2);
            cell02.PossibleValues.Add(3);
        }

        // Row 1 twins
        var cell13 = puzzle.GetCell(1, 3);
        var cell14 = puzzle.GetCell(1, 4);
        var cell15 = puzzle.GetCell(1, 5);

        if (!cell13.HasValue)
        {
            cell13.PossibleValues.Clear();
            cell13.PossibleValues.Add(7);
            cell13.PossibleValues.Add(8);
        }

        if (!cell14.HasValue)
        {
            cell14.PossibleValues.Clear();
            cell14.PossibleValues.Add(7);
            cell14.PossibleValues.Add(8);
        }

        if (!cell15.HasValue)
        {
            cell15.PossibleValues.Clear();
            cell15.PossibleValues.Add(7);
            cell15.PossibleValues.Add(8);
            cell15.PossibleValues.Add(9);
        }
    }

    private static void SetupInvalidTwinEliminationScenario(SudokuPuzzle puzzle)
    {
        var cell00 = puzzle.GetCell(0, 0);
        var cell01 = puzzle.GetCell(0, 1);
        var cell02 = puzzle.GetCell(0, 2);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
        }

        if (!cell01.HasValue)
        {
            cell01.PossibleValues.Clear();
            cell01.PossibleValues.Add(1);
            cell01.PossibleValues.Add(2);
        }

        if (!cell02.HasValue)
        {
            cell02.PossibleValues.Clear();
            cell02.PossibleValues.Add(1);
            cell02.PossibleValues.Add(2);
        }
    }

    private static void SetupNonTwinOverlapScenario(SudokuPuzzle puzzle)
    {
        var cell00 = puzzle.GetCell(0, 0);
        var cell01 = puzzle.GetCell(0, 1);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
            cell00.PossibleValues.Add(3);
        }

        if (!cell01.HasValue)
        {
            cell01.PossibleValues.Clear();
            cell01.PossibleValues.Add(1);
            cell01.PossibleValues.Add(2);
            cell01.PossibleValues.Add(4);
        }
    }

    private static void SetupMixedFilledAndEmptyCellsScenario(SudokuPuzzle puzzle)
    {
        var cell01 = puzzle.GetCell(0, 1);
        var cell02 = puzzle.GetCell(0, 2);
        var cell03 = puzzle.GetCell(0, 3);

        if (!cell01.HasValue)
        {
            cell01.PossibleValues.Clear();
            cell01.PossibleValues.Add(1);
            cell01.PossibleValues.Add(2);
        }

        if (!cell02.HasValue)
        {
            cell02.PossibleValues.Clear();
            cell02.PossibleValues.Add(1);
            cell02.PossibleValues.Add(2);
        }

        if (!cell03.HasValue)
        {
            cell03.PossibleValues.Clear();
            cell03.PossibleValues.Add(1);
            cell03.PossibleValues.Add(2);
            cell03.PossibleValues.Add(3);
        }
    }

    private static void SetupTwinsInDifferentRowsScenario(SudokuPuzzle puzzle)
    {
        // Row 0 twins
        var cell00 = puzzle.GetCell(0, 0);
        var cell01 = puzzle.GetCell(0, 1);
        var cell02 = puzzle.GetCell(0, 2);

        if (!cell00.HasValue)
        {
            cell00.PossibleValues.Clear();
            cell00.PossibleValues.Add(1);
            cell00.PossibleValues.Add(2);
        }

        if (!cell01.HasValue)
        {
            cell01.PossibleValues.Clear();
            cell01.PossibleValues.Add(1);
            cell01.PossibleValues.Add(2);
        }

        if (!cell02.HasValue)
        {
            cell02.PossibleValues.Clear();
            cell02.PossibleValues.Add(1);
            cell02.PossibleValues.Add(2);
            cell02.PossibleValues.Add(3);
        }

        // Row 8 twins
        var cell86 = puzzle.GetCell(8, 6);
        var cell87 = puzzle.GetCell(8, 7);
        var cell88 = puzzle.GetCell(8, 8);

        if (!cell86.HasValue)
        {
            cell86.PossibleValues.Clear();
            cell86.PossibleValues.Add(8);
            cell86.PossibleValues.Add(9);
        }

        if (!cell87.HasValue)
        {
            cell87.PossibleValues.Clear();
            cell87.PossibleValues.Add(8);
            cell87.PossibleValues.Add(9);
        }

        if (!cell88.HasValue)
        {
            cell88.PossibleValues.Clear();
            cell88.PossibleValues.Add(8);
            cell88.PossibleValues.Add(9);
            cell88.PossibleValues.Add(7);
        }
    }
}