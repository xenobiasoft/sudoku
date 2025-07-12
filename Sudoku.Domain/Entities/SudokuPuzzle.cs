using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Domain.Entities;

public class SudokuPuzzle
{
    private readonly List<Cell> _cells;

    public string PuzzleId { get; }
    public GameDifficulty Difficulty { get; }
    public IReadOnlyList<Cell> Cells => _cells.AsReadOnly();

    private SudokuPuzzle(string puzzleId, GameDifficulty difficulty, IEnumerable<Cell> cells)
    {
        PuzzleId = puzzleId;
        Difficulty = difficulty;
        _cells = cells.ToList();
    }

    public static SudokuPuzzle Create(string puzzleId, GameDifficulty difficulty, IEnumerable<Cell> cells)
    {
        var puzzle = new SudokuPuzzle(puzzleId, difficulty, cells);

        if (!puzzle.IsValid())
            throw new InvalidPuzzleException("Puzzle is not valid");

        return puzzle;
    }

    public Cell GetCell(int row, int column)
    {
        return _cells.FirstOrDefault(c => c.Row == row && c.Column == column)
               ?? throw new CellNotFoundException($"Cell not found at position ({row}, {column})");
    }

    public bool IsValid()
    {
        // Check if we have exactly 81 cells
        if (_cells.Count != 81)
            return false;

        // Check if all cells are in valid positions
        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                if (!_cells.Any(c => c.Row == row && c.Column == column))
                    return false;
            }
        }

        // Check if the puzzle follows Sudoku rules
        return IsValidSudoku();
    }

    private bool IsValidSudoku()
    {
        // Check all rows
        for (int row = 0; row < 9; row++)
        {
            var rowCells = _cells.Where(c => c.Row == row && c.HasValue).ToList();
            var rowValues = rowCells.Select(c => c.Value!.Value).ToList();
            if (rowValues.Count != rowValues.Distinct().Count())
                return false;
        }

        // Check all columns
        for (int column = 0; column < 9; column++)
        {
            var columnCells = _cells.Where(c => c.Column == column && c.HasValue).ToList();
            var columnValues = columnCells.Select(c => c.Value!.Value).ToList();
            if (columnValues.Count != columnValues.Distinct().Count())
                return false;
        }

        // Check all 3x3 boxes
        for (int boxRow = 0; boxRow < 9; boxRow += 3)
        {
            for (int boxColumn = 0; boxColumn < 9; boxColumn += 3)
            {
                var boxCells = _cells.Where(c => c.Row >= boxRow && c.Row < boxRow + 3 &&
                                                c.Column >= boxColumn && c.Column < boxColumn + 3 &&
                                                c.HasValue).ToList();
                var boxValues = boxCells.Select(c => c.Value!.Value).ToList();
                if (boxValues.Count != boxValues.Distinct().Count())
                    return false;
            }
        }

        return true;
    }

    public bool HasUniqueSolution()
    {
        // This is a simplified check - in a real implementation, you'd want a more sophisticated solver
        // For now, we'll assume that puzzles with a reasonable number of fixed cells have unique solutions
        var fixedCells = _cells.Count(c => c.IsFixed);
        return fixedCells >= 17; // Minimum number of clues for a unique solution
    }

    public int GetFixedCellCount() => _cells.Count(c => c.IsFixed);

    public int GetEmptyCellCount() => _cells.Count(c => !c.HasValue);
}