using System.Collections;

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
        return _cells.FirstOrDefault(c => c.Row == row && c.Column == column) ?? throw new CellNotFoundException($"Cell not found at position ({row}, {column})");
    }

    public bool IsValid()
    {
        if (_cells.Count != 81)
        {
            return false;
        }

        for (var row = 0; row < 9; row++)
        {
            for (var column = 0; column < 9; column++)
            {
                if (!_cells.Any(c => c.Row == row && c.Column == column))
                {
                    return false;
                }
            }
        }

        return IsValidSudoku();
    }

    private bool IsValidSudoku()
    {
        for (var row = 0; row < 9; row++)
        {
            var rowCells = _cells.Where(c => c.Row == row && c.HasValue).ToList();
            var rowValues = rowCells.Select(c => c.Value!.Value).ToList();
            if (rowValues.Count != rowValues.Distinct().Count())
            {
                return false;
            }
        }

        for (var column = 0; column < 9; column++)
        {
            var columnCells = _cells.Where(c => c.Column == column && c.HasValue).ToList();
            var columnValues = columnCells.Select(c => c.Value!.Value).ToList();
            if (columnValues.Count != columnValues.Distinct().Count())
            {
                return false;
            }
        }

        for (var boxRow = 0; boxRow < 9; boxRow += 3)
        {
            for (var boxColumn = 0; boxColumn < 9; boxColumn += 3)
            {
                var boxCells = _cells.Where(c => c.Row >= boxRow && c.Row < boxRow + 3 &&
                                                c.Column >= boxColumn && c.Column < boxColumn + 3 &&
                                                c.HasValue).ToList();
                var boxValues = boxCells.Select(c => c.Value!.Value).ToList();
                if (boxValues.Count != boxValues.Distinct().Count())
                {
                    return false;
                }
            }
        }

        return true;
    }

    private const int MinimumCluesForUniqueSolution = 17; // Based on Sudoku rules, 17 is the minimum number of clues required for a puzzle to have a unique solution.

    public bool HasUniqueSolution()
    {
        // This is a simplified check - in a real implementation, you'd want a more sophisticated solver
        // For now, we'll assume that puzzles with a reasonable number of fixed cells have unique solutions
        var fixedCells = _cells.Count(c => c.IsFixed);
        return fixedCells >= MinimumCluesForUniqueSolution;
    }

    public int GetFixedCellCount() => _cells.Count(c => c.IsFixed);

    public int GetEmptyCellCount() => _cells.Count(c => !c.HasValue);

    public IEnumerable<Cell> GetColumnCells(int col)
    {
        return _cells.Where(c => c.Column == col).OrderBy(c => c.Row);
    }

    public IEnumerable<Cell> GetRowCells(int row)
    {
        return _cells.Where(c => c.Row == row).OrderBy(c => c.Column);
    }

    public IEnumerable<Cell> GetMiniGridCells(int boxRow, int boxColumn)
    {
        return _cells.Where(c => c.Row >= boxRow * 3 && c.Row < (boxRow + 1) * 3 &&
                                 c.Column >= boxColumn * 3 && c.Column < (boxColumn + 1) * 3)
                     .OrderBy(c => c.Row).ThenBy(c => c.Column);
    }
}