using Sudoku.Domain.ValueObjects;

namespace UnitTests.Helpers.Factories;

public static class CellsFactory
{
    public static IEnumerable<Cell> CreateCompletedCells()
    {
        int?[,] completedCells = {
            {5, 3, 4, 6, 7, 8, 9, 1, 2},
            {6, 7, 2, 1, 9, 5, 3, 4, 8},
            {1, 9, 8, 3, 4, 2, 5, 6, 7},
            {8, 5, 9, 7, 6, 1, 4, 2, 3},
            {4, 2, 6, 8, 5, 3, 7, 9, 1},
            {7, 1, 3, 9, 2, 4, 8, 5, 6},
            {9, 6, 1, 5, 3, 7, 2, 8, 4},
            {2, 8, 7, 4, 1, 9, 6, 3, 5},
            {3, 4, 5, 2, 8, 6, 1, 7, 9}
        };

        return GetCells(completedCells);
    }

    public static IEnumerable<Cell> CreateEmptyCells()
    {
        var emptyCells = new int?[9, 9];

        return GetCells(emptyCells);
    }

    public static IEnumerable<Cell> CreateIncompleteCells()
    {
        int?[,] partiallyCompleted = {
            {null, 3, 4, 6, 7, 8, 9, 1, 2},
            {6, null, 2, 1, 9, 5, 3, 4, 8},
            {1, 9, 8, 3, 4, 2, 5, 6, 7},
            {8, 5, 9, 7, 6, 1, 4, 2, 3},
            {4, 2, 6, 8, 5, 3, 7, 9, 1},
            {7, 1, 3, 9, 2, 4, 8, 5, 6},
            {9, 6, 1, 5, 3, 7, 2, 8, 4},
            {2, 8, 7, 4, 1, 9, 6, 3, 5},
            {3, 4, 5, 2, 8, 6, 1, 7, 9}
        };

        return GetCells(partiallyCompleted);
    }

    public static IEnumerable<Cell> CreateInvalidCells()
    {
        int?[,] partiallyCompleted = {
            {null, 3, 4, 6, 7, 8, 9, 1, 2},
            {6, 8, 2, 1, 9, 5, 3, 4, 8},
            {1, 9, 8, 3, 4, 2, 5, 6, 7},
            {8, 5, 9, 7, 6, 1, 4, 2, 3},
            {4, 2, 6, 8, 5, 3, 7, 9, 1},
            {7, 1, 3, 9, 2, 4, 8, 5, 6},
            {9, 6, 1, 5, 3, 7, 2, 8, 4},
            {2, 8, 7, 4, 1, 9, 6, 3, 5},
            {3, 4, 5, 2, 8, 6, 1, 7, 9}
        };

        return GetCells(partiallyCompleted);
    }

    private static IEnumerable<Cell> GetCells(int?[,] cellValues)
    {
        var cells = new List<Cell>();

        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                var cellValue = cellValues[row, col];
                cells.Add(Cell.Create(row, col, cellValue, cellValue.HasValue));
            }
        }

        return cells;
    }
}