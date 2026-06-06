using Sudoku.Domain.Entities;

namespace UnitTests.Helpers;

public static class SudokuPuzzleExtensions
{
    extension(SudokuPuzzle puzzle)
    {
        public SudokuPuzzle AssertAllCellsEmpty()
        {
            var allCells = puzzle.Cells;
            var emptyCells = allCells.Where(x => !x.Value.HasValue).ToList();
        
            emptyCells.Should().HaveCount(allCells.Count);

            return puzzle;
        }

        public SudokuPuzzle AssertHasExpectedNumberEmptyCells(int min, int max)
        {
            var emptyCells = puzzle.Cells.Count(x => !x.Value.HasValue);

            emptyCells.Should().BeInRange(min, max);

            return puzzle;
        }

        public SudokuPuzzle AssertPopulatedCellsLocked()
        {
            var populatedCells = puzzle.Cells.Where(x => x.Value.HasValue).ToList();

            Assert.All(populatedCells, x => x.IsFixed.Should().BeTrue());

            return puzzle;
        }
    }
}