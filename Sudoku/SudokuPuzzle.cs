using XenobiaSoft.Sudoku.Exceptions;
using XenobiaSoft.Sudoku.Extensions;
using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku;

public class SudokuPuzzle : ISudokuPuzzle
{
    private readonly Cell[] _cells = new Cell[GameDimensions.Columns * GameDimensions.Rows];

    public SudokuPuzzle()
    {
        Initialize();
    }

    public Cell FindCellWithFewestPossibleValues()
    {
        var cellPossibleValues = _cells
            .Where(x => !x.Value.HasValue)
            .OrderBy(x => x.PossibleValues.Count)
            .ThenBy(x => x.Column)
            .ThenBy(x => x.Row)
            .ToList();

        return cellPossibleValues.First();
    }

    public Cell[] GetAllCells()
    {
        return _cells;
    }

    public Cell GetCell(int row, int col)
    {
        return _cells.First(x => x.Column == col && x.Row == row);
    }

    public IEnumerable<Cell> GetColumnCells(int col)
    {
        return _cells.Where(x => x.Column == col);
    }

    public IEnumerable<Cell> GetRowCells(int row)
    {
        return _cells.Where(x => x.Row == row);
    }

    public IEnumerable<Cell> GetMiniGridCells(int row, int col)
    {
        var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);
        var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartCol(row);

        return _cells.Where(x =>
            x.Column >= miniGridStartCol &&
            x.Column < miniGridStartCol + 3 &&
            x.Row >= miniGridStartRow &&
            x.Row < miniGridStartRow + 3);
    }

    public bool IsSolved()
    {
        foreach (var cell in _cells)
        {
            var pattern = GetColumnCells(cell.Column).Aggregate("123456789", (current, columnCell) => current.Replace(columnCell.Value.GetValueOrDefault().ToString(), string.Empty));

            if (pattern.Length > 0)
            {
                return false;
            }

            pattern = GetRowCells(cell.Row).Aggregate("123456789", (current, rowCell) => current.Replace(rowCell.Value.GetValueOrDefault().ToString(), string.Empty));

            if (pattern.Length > 0)
            {
                return false;
            }

            pattern = GetMiniGridCells(cell.Row, cell.Column).Aggregate("123456789", (current, gridCell) => current.Replace(gridCell.Value.GetValueOrDefault().ToString(), string.Empty));

            if (pattern.Length > 0)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsValid()
    {
        foreach (var cell in _cells)
        {
            if (!cell.Value.HasValue) continue;

            var usedNumbers = new List<int?>();

            foreach (var colCell in GetColumnCells(cell.Column))
            {
                if (!colCell.Value.HasValue) continue;

                if (usedNumbers.Contains(colCell.Value))
                {
                    Console.WriteLine($"Puzzle is not valid. Column cell row: {colCell.Row}, col:{colCell.Column} has value {colCell.Value} that is already taken.");
                    return false;
                }

                usedNumbers.Add(colCell.Value);
            }

            usedNumbers.Clear();

            foreach (var rowCell in GetRowCells(cell.Row))
            {
                if (!rowCell.Value.HasValue) continue;

                if (usedNumbers.Contains(rowCell.Value))
                {
                    Console.WriteLine($"Puzzle is not valid. Row cell row: {rowCell.Row}, col:{rowCell.Column} has value {rowCell.Value} that is already taken.");
                    return false;
                }

                usedNumbers.Add(rowCell.Value);
            }

            usedNumbers.Clear();

            foreach (var miniGridCell in GetMiniGridCells(cell.Row, cell.Column))
            {
                if (!miniGridCell.Value.HasValue) continue;

                if (usedNumbers.Contains(miniGridCell.Value))
                {
                    Console.WriteLine($"Puzzle is not valid. Mini grid cell row: {miniGridCell.Row}, col:{miniGridCell.Column}  has value  {miniGridCell.Value} that is already taken.");
                    return false;
                }

                usedNumbers.Add(miniGridCell.Value);
            }
        }

        return true;
    }

    public void Load(PuzzleState state)
    {
        PuzzleId = state.PuzzleId;
        var cells = state.Board;
        
        for (var i = 0; i < cells.Length; i++)
        {
            _cells[i] = cells[i];
        }
    }

    public void PopulatePossibleValues()
    {
        foreach (var cell in _cells)
        {
            cell.PossibleValues = cell.Value.HasValue ? [] : CalculatePossibleValues(cell);
        }
    }

    public void Restore(Cell[] cells)
    {
        for (var i = 0; i < cells.Length; i++)
        {
            _cells[i].Value = cells[i].Value;
            _cells[i].PossibleValues = cells[i].PossibleValues;
        }
    }

    public void SetCell(int row, int column, int? value)
    {
        var cell = GetCell(row, column);
        
        cell.Value = value;
        cell.PossibleValues = [];
    }

    public void SetCellWithFewestPossibleValues()
    {
        var cell = FindCellWithFewestPossibleValues();
        var possibleValues = cell.PossibleValues.Randomize();

        if (!possibleValues.Any())
        {
            throw new InvalidMoveException();
        }

        foreach (var possibleValue in possibleValues)
        {
            var cellValue = possibleValue;
            Console.WriteLine($"Setting cell:{cell.Row}:{cell.Column} to value {cellValue}");
            cell.Value = cellValue;

            if (IsValid()) break;
        }
    }

    public IEnumerable<Cell> Validate()
    {
        var invalidCells = new List<Cell>();

        foreach (var cell in _cells)
        {
            if (!cell.Value.HasValue) continue;

            GetColumnCells(cell.Column).Where(x => x != cell && x.Value == cell.Value).ToList().ForEach(x =>
            {
                invalidCells.Add(cell);
                invalidCells.Add(x);
            });
            GetRowCells(cell.Row).Where(x => x != cell && x.Value == cell.Value).ToList().ForEach(x =>
            {
                invalidCells.Add(cell);
                invalidCells.Add(x);
            });
            GetMiniGridCells(cell.Row, cell.Column).Where(x => x != cell && x.Value == cell.Value).ToList().ForEach(
                x =>
                {
                    invalidCells.Add(cell);
                    invalidCells.Add(x);
                });
        }
        return invalidCells.Distinct();
    }

    public string PuzzleId { get; set; } = Guid.NewGuid().ToString();

    private List<int> CalculatePossibleValues(Cell cell)
    {
        var allValues = Enumerable.Range(1, 9).ToList();

        foreach (var columnCell in GetColumnCells(cell.Column))
        {
            if (columnCell.Value.HasValue)
            {
                allValues.Remove(columnCell.Value.Value);
            }
        }

        foreach (var rowCell in GetRowCells(cell.Row))
        {
            if (rowCell.Value.HasValue)
            {
                allValues.Remove(rowCell.Value.Value);
            }
        }

        foreach (var gridCell in GetMiniGridCells(cell.Row, cell.Column))
        {
            if (gridCell.Value.HasValue)
            {
                allValues.Remove(gridCell.Value.Value);
            }
        }

        return allValues;
    }

    private void Initialize()
    {
        var index = 0;

        for (var row = 0; row < GameDimensions.Rows; row++)
        {
            for (var col = 0; col < GameDimensions.Columns; col++)
            {
                _cells[index++] = new Cell(row, col).Copy();
            }
        }
    }
}