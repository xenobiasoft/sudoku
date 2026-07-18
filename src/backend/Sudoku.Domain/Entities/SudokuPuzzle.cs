using Sudoku.Domain.Helpers;
using Sudoku.Domain.Interfaces;

namespace Sudoku.Domain.Entities;

public class SudokuPuzzle : ICloneable
{
    private readonly List<Cell> _cells;

    public string PuzzleId { get; }
    public GameDifficulty Difficulty { get; }
    public BoardSize Size { get; }
    public IReadOnlyList<Cell> Cells => _cells.AsReadOnly();

    private SudokuPuzzle(string puzzleId, GameDifficulty difficulty, BoardSize size, IEnumerable<Cell> cells)
    {
        PuzzleId = puzzleId;
        Difficulty = difficulty;
        Size = size;
        _cells = cells.ToList();
    }

    public static SudokuPuzzle Create(string puzzleId, GameDifficulty difficulty, BoardSize size, IEnumerable<Cell> cells)
    {
        var puzzle = new SudokuPuzzle(puzzleId, difficulty, size, cells);

        if (!puzzle.IsValid())
        {
            throw new InvalidPuzzleException();
        }

        return puzzle;
    }

    public Cell GetCell(int row, int column)
    {
        return _cells.FirstOrDefault(c => c.Row == row && c.Column == column) ?? throw new CellNotFoundException($"Cell not found at position ({row}, {column})");
    }

    public bool IsValid()
    {
        if (_cells.Count != Size.CellCount)
        {
            return false;
        }

        for (var row = 0; row < Size.Size; row++)
        {
            for (var column = 0; column < Size.Size; column++)
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
        // Check rows
        for (var row = 0; row < Size.Size; row++)
        {
            var usedValues = new HashSet<int>();
            var rowCells = GetRowCells(row).Where(c => c.HasValue);

            foreach (var cell in rowCells)
            {
                if (!usedValues.Add(cell.Value!.Value))
                {
                    return false; // Duplicate found
                }
            }
        }

        // Check columns
        for (var column = 0; column < Size.Size; column++)
        {
            var usedValues = new HashSet<int>();
            var columnCells = GetColumnCells(column).Where(c => c.HasValue);

            foreach (var cell in columnCells)
            {
                if (!usedValues.Add(cell.Value!.Value))
                {
                    return false; // Duplicate found
                }
            }
        }

        // Check boxes
        for (var boxRow = 0; boxRow < Size.BoxSize; boxRow++)
        {
            for (var boxColumn = 0; boxColumn < Size.BoxSize; boxColumn++)
            {
                var usedValues = new HashSet<int>();
                var boxCells = GetMiniGridCells(boxRow, boxColumn).Where(c => c.HasValue);

                foreach (var cell in boxCells)
                {
                    if (!usedValues.Add(cell.Value!.Value))
                    {
                        return false; // Duplicate found
                    }
                }
            }
        }

        return true;
    }

    public bool HasUniqueSolution()
    {
        // This is a simplified check - in a real implementation, you'd want a more sophisticated solver
        // For now, we'll assume that puzzles with a reasonable number of fixed cells have unique solutions
        var fixedCells = _cells.Count(c => c.IsFixed);
        return fixedCells >= Size.MinimumClues;
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
        return _cells.Where(c => c.Row >= boxRow * Size.BoxSize && c.Row < (boxRow + 1) * Size.BoxSize &&
                                 c.Column >= boxColumn * Size.BoxSize && c.Column < (boxColumn + 1) * Size.BoxSize)
                     .OrderBy(c => c.Row).ThenBy(c => c.Column);
    }

    public void PopulatePossibleValues()
    {
        foreach (var cell in _cells)
        {
            if (cell.HasValue)
            {
                cell.PossibleValues.Clear();
                continue;
            }
            var rowValues = GetRowCells(cell.Row).Where(c => c.HasValue).Select(c => c.Value!.Value).ToHashSet();
            var columnValues = GetColumnCells(cell.Column).Where(c => c.HasValue).Select(c => c.Value!.Value).ToHashSet();
            var miniGridValues = GetMiniGridCells(cell.Row / Size.BoxSize, cell.Column / Size.BoxSize)
                .Where(c => c.HasValue)
                .Select(c => c.Value!.Value)
                .ToHashSet();
            var usedValues = rowValues.Union(columnValues).Union(miniGridValues);
            cell.PossibleValues.Clear();
            cell.PossibleValues.AddRange(Size.AllValues.Where(v => !usedValues.Contains(v)).ToList());
        }
    }

    /// <summary>
    /// Creates a deep copy of the current SudokuPuzzle.
    /// </summary>
    /// <returns>A deep copy of the current SudokuPuzzle with all cells and their states copied.</returns>
    public object Clone()
    {
        // Deep clone all cells
        var clonedCells = _cells.Select(cell => cell.DeepCopy()).ToList();

        // Create a new puzzle instance using the private constructor
        return new SudokuPuzzle(PuzzleId, Difficulty, Size, clonedCells);
    }
}
