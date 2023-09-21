using XenobiaSoft.Sudoku.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XenobiaSoft.Sudoku.Helpers;

public static class CellsExtensionMethods
{
	public static Cell FindCellWithFewestPossibleValues(this Cell[] cells)
	{
		var cellPossibleValues = cells
			.Where(x => !x.Value.HasValue)
			.OrderBy(x => x.PossibleValues.Length)
			.ThenBy(x => x.Column)
			.ThenBy(x => x.Row)
			.ToList();

		return cellPossibleValues.First();
	}

	public static Cell GetCell(this Cell[] cells, int row, int col)
	{
		return cells.First(x => x.Column == col && x.Row == row);
	}

	public static IEnumerable<Cell> GetColumnCells(this Cell[] cells, int col)
	{
		return cells.Where(x => x.Column == col);
	}

	public static IEnumerable<Cell> GetRowCells(this Cell[] cells, int row)
	{
		return cells.Where(x => x.Row == row);
	}

	public static IEnumerable<Cell> GetMiniGridCells(this Cell[] cells, int row, int col)
	{
		var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);
		var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartCol(row);

		return cells.Where(x =>
			x.Column >= miniGridStartCol && 
			x.Column < miniGridStartCol + 3 && 
			x.Row >= miniGridStartRow &&
			x.Row < miniGridStartRow + 3);
	}

	public static bool IsSolved(this Cell[] cells)
	{
		foreach (var cell in cells)
		{
			var pattern = cells.GetColumnCells(cell.Column).Aggregate("123456789", (current, columnCell) => current.Replace(columnCell.Value.GetValueOrDefault().ToString(), string.Empty));

			if (pattern.Length > 0)
			{
				return false;
			}

			pattern = cells.GetRowCells(cell.Row).Aggregate("123456789", (current, rowCell) => current.Replace(rowCell.Value.GetValueOrDefault().ToString(), string.Empty));

			if (pattern.Length > 0)
			{
				return false;
			}

			pattern = cells.GetMiniGridCells(cell.Row, cell.Column).Aggregate("123456789", (current, gridCell) => current.Replace(gridCell.Value.GetValueOrDefault().ToString(), string.Empty));

			if (pattern.Length > 0)
			{
				return false;
			}
		}

		return true;
	}

	public static bool IsValid(this Cell[] cells)
	{
		foreach (var cell in cells)
		{
			if (!cell.Value.HasValue) continue;

			var usedNumbers = new List<int?>();

			foreach (var colCell in cells.GetColumnCells(cell.Column))
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

			foreach (var rowCell in cells.GetRowCells(cell.Row))
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

			foreach (var miniGridCell in cells.GetMiniGridCells(cell.Row, cell.Column))
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

	public static void PopulatePossibleValues(this Cell[] cells)
	{
		foreach (var cell in cells)
		{
			cell.PossibleValues = cell.Value.HasValue ? string.Empty : cells.CalculatePossibleValues(cell);
		}
	}

	public static void SetCell(this Cell[] cells, int row, int column, int? value)
	{
		cells.GetCell(row, column).Value = value;
	}

	public static void SetCellWithFewestPossibleValues(this Cell[] cells)
	{
		var cell = cells.FindCellWithFewestPossibleValues();
		var possibleValues = cell.PossibleValues.Randomize();

		if (string.IsNullOrWhiteSpace(possibleValues))
		{
			throw new InvalidMoveException();
		}

		foreach (var possibleValue in possibleValues.ToArray())
        {
			var cellValue = int.Parse(possibleValue.ToString());
            Console.WriteLine($"Setting cell:{cell.Row}:{cell.Column} to value {cellValue}");
            cell.Value = cellValue;

			if (cells.IsValid()) break;
		}
	}

    public static IEnumerable<Cell> Validate(this Cell[] cells)
    {
		var invalidCells = new List<Cell>();

        foreach (var cell in cells)
        {
            if (!cell.Value.HasValue) continue;

			cells.GetColumnCells(cell.Column).Where(x => x != cell && x.Value == cell.Value).ToList().ForEach(x =>
            {
                invalidCells.Add(cell);
                invalidCells.Add(x);
            });
			cells.GetRowCells(cell.Row).Where(x => x != cell && x.Value == cell.Value).ToList().ForEach(x =>
            {
				invalidCells.Add(cell);
				invalidCells.Add(x);
            });
			cells.GetMiniGridCells(cell.Row, cell.Column).Where(x => x != cell && x.Value == cell.Value).ToList().ForEach(
                x =>
                {
					invalidCells.Add(cell);
					invalidCells.Add(x);
                });
        }
        return invalidCells.Distinct();
    }

	private static string CalculatePossibleValues(this Cell[] cells, Cell cell)
	{
		var possibleValues = cells.GetColumnCells(cell.Column).Aggregate("123456789", (current, columnCell) => current.Replace(columnCell.Value.GetValueOrDefault().ToString(), string.Empty));

		possibleValues = cells.GetRowCells(cell.Row).Aggregate(possibleValues, (current, rowCell) => current.Replace(rowCell.Value.GetValueOrDefault().ToString(), string.Empty));

		return cells.GetMiniGridCells(cell.Row, cell.Column).Aggregate(possibleValues, (current, gridCell) => current.Replace(gridCell.Value.GetValueOrDefault().ToString(), string.Empty));
	}
}