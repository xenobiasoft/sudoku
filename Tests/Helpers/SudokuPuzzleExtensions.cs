using XenobiaSoft.Sudoku;

namespace UnitTests.Helpers;

public static class SudokuPuzzleExtensions
{
    public static ISudokuPuzzle AssertAllCellsEmpty(this ISudokuPuzzle puzzle)
    {
        var allCells = puzzle.GetAllCells().ToList();
        var emptyCells = allCells.Where(x => !x.Value.HasValue).ToList();
        
        emptyCells.Should().HaveCount(allCells.Count);

        return puzzle;
    }

    public static ISudokuPuzzle AssertHasExpectedNumberEmptyCells(this ISudokuPuzzle puzzle, int min, int max)
    {
        var emptyCells = puzzle.GetAllCells().Count(x => !x.Value.HasValue);

        emptyCells.Should().BeInRange(min, max);

        return puzzle;
    }

    public static ISudokuPuzzle AssertPopulatedCellsLocked(this ISudokuPuzzle puzzle)
    {
        var populatedCells = puzzle.GetAllCells().Where(x => x.Value.HasValue).ToList();

        Assert.All(populatedCells, x => x.Locked.Should().BeTrue());

        return puzzle;
    }
}