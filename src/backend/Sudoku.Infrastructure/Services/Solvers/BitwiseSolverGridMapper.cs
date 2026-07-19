using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Services.Solvers;

/// <summary>
/// Converts between the domain <see cref="SudokuPuzzle"/> and the flat <c>int[]</c> grid
/// used by <see cref="BitwiseSolverEngine"/> (row-major, 0 = empty, 1-N = value), sized from
/// <see cref="BoardSize.CellCount"/>.
/// </summary>
public static class BitwiseSolverGridMapper
{
    public static int[] ToGrid(SudokuPuzzle puzzle)
    {
        var grid = new int[puzzle.Size.CellCount];

        foreach (var cell in puzzle.Cells)
        {
            grid[cell.Row * puzzle.Size.Size + cell.Column] = cell.Value ?? 0;
        }

        return grid;
    }

    /// <summary>
    /// Projects a solved/partial grid back onto a new puzzle, preserving each cell's
    /// fixed status and difficulty. Empty grid positions (0) become empty cells.
    /// </summary>
    public static SudokuPuzzle ApplyTo(SudokuPuzzle puzzle, int[] grid)
    {
        var cells = puzzle.Cells.Select(cell =>
        {
            var value = grid[cell.Row * puzzle.Size.Size + cell.Column];
            return Cell.Create(cell.Row, cell.Column, puzzle.Size, value == 0 ? null : value, cell.IsFixed);
        }).ToList();

        return SudokuPuzzle.Create(puzzle.PuzzleId, puzzle.Difficulty, puzzle.Size, cells);
    }
}
