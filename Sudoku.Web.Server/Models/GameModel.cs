namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents a game in the Blazor application
/// </summary>
public class GameModel
{
    public string Id { get; set; } = string.Empty;
    public string PlayerAlias { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public GameStatisticsModel Statistics { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? PausedAt { get; set; }
    public List<CellModel> Cells { get; set; } = new();
    public List<MoveHistoryModel> MoveHistory { get; set; } = new();

    public CellModel GetCell(int row, int col)
    {
        return Cells.First(x => x.Column == col && x.Row == row);
    }

    public IEnumerable<CellModel> GetColumnCells(int col)
    {
        return Cells.Where(x => x.Column == col);
    }

    public IEnumerable<CellModel> GetRowCells(int row)
    {
        return Cells.Where(x => x.Row == row);
    }

    public IEnumerable<CellModel> GetMiniGridCells(int row, int col)
    {
        var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);
        var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartCol(row);

        return Cells.Where(x =>
            x.Column >= miniGridStartCol &&
            x.Column < miniGridStartCol + 3 &&
            x.Row >= miniGridStartRow &&
            x.Row < miniGridStartRow + 3);
    }

    public bool IsSolved()
    {
        if (!Cells.Any())
        {
            return false;
        }

        foreach (var cell in Cells)
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
        foreach (var cell in Cells)
        {
            if (!cell.Value.HasValue) continue;

            var usedNumbers = new List<int?>();

            foreach (var colCell in GetColumnCells(cell.Column))
            {
                if (!colCell.Value.HasValue) continue;

                if (usedNumbers.Contains(colCell.Value))
                {
                    Console.WriteLine($"CurrentGame is not valid. Column cell row: {colCell.Row}, col:{colCell.Column} has value {colCell.Value} that is already taken.");
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
                    Console.WriteLine($"CurrentGame is not valid. Row cell row: {rowCell.Row}, col:{rowCell.Column} has value {rowCell.Value} that is already taken.");
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
                    Console.WriteLine($"CurrentGame is not valid. Mini grid cell row: {miniGridCell.Row}, col:{miniGridCell.Column}  has value  {miniGridCell.Value} that is already taken.");
                    return false;
                }

                usedNumbers.Add(miniGridCell.Value);
            }
        }

        return true;
    }

    public IEnumerable<CellModel> Validate()
    {
        var invalidCells = new List<CellModel>();

        foreach (var cell in Cells)
        {
            if (!cell.Value.HasValue) continue;

            var usedNumbers = new List<int?>();

            foreach (var colCell in GetColumnCells(cell.Column))
            {
                if (!colCell.Value.HasValue) continue;

                if (usedNumbers.Contains(colCell.Value))
                {
                    invalidCells.Add(colCell);
                }

                usedNumbers.Add(colCell.Value);
            }

            usedNumbers.Clear();

            foreach (var rowCell in GetRowCells(cell.Row))
            {
                if (!rowCell.Value.HasValue) continue;

                if (usedNumbers.Contains(rowCell.Value))
                {
                    invalidCells.Add(rowCell);
                }

                usedNumbers.Add(rowCell.Value);
            }

            usedNumbers.Clear();

            foreach (var miniGridCell in GetMiniGridCells(cell.Row, cell.Column))
            {
                if (!miniGridCell.Value.HasValue) continue;

                if (usedNumbers.Contains(miniGridCell.Value))
                {
                    invalidCells.Add(miniGridCell);
                }

                usedNumbers.Add(miniGridCell.Value);
            }
        }

        return invalidCells;
    }
}