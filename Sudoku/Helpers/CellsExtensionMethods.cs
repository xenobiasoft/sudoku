namespace XenobiaSoft.Sudoku.Helpers;

public static class CellsExtensionMethods
{
	private const int Columns = 9;
	private const int Rows = 9;
	
	public static Cell GetCell(this Cell[] cells, int row, int col)
	{
		return cells.First(x => x.Column == col && x.Row == row);
	}

	public static IEnumerable<Cell> GetColumnCells(this Cell[] cells, int col)
	{
		for (var row = 0; row < Rows; row++)
		{
			yield return cells[col * 9 + row];
		}
	}

	public static IEnumerable<Cell> GetRowCells(this Cell[] cells, int row)
	{
		for (var col = 0; col < Columns; col++)
		{
			yield return cells[col * 9 + row];
		}
	}

	public static IEnumerable<Cell> GetMiniGridCells(this Cell[] cells, int row, int col)
	{
		var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);
		var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartCol(row);

		for (var miniGridRow = miniGridStartRow; miniGridRow <= miniGridStartRow + 2; miniGridRow++)
		{
			for (var miniGridCol = miniGridStartCol; miniGridCol <= miniGridStartCol + 2; miniGridCol++)
			{
				yield return cells[miniGridCol * 9 + miniGridRow];
			}
		}
	}

	public static void PopulatePossibleValues(this Cell[] cells)
	{
		foreach (var cell in cells)
		{
			cell.PossibleValues = cell.Value.HasValue ? string.Empty : cells.CalculatePossibleValues(cell);
		}
	}

	private static string CalculatePossibleValues(this Cell[] cells, Cell cell)
	{
		var possibleValues = string.IsNullOrWhiteSpace(cell.PossibleValues) ? "123456789" : cell.PossibleValues;

		possibleValues = cells.GetColumnCells(cell.Column).Aggregate(possibleValues, (current, columnCell) => current.Replace(columnCell.Value.GetValueOrDefault().ToString(), string.Empty));
		possibleValues = cells.GetRowCells(cell.Row).Aggregate(possibleValues, (current, rowCell) => current.Replace(rowCell.Value.GetValueOrDefault().ToString(), string.Empty));
		possibleValues = cells.GetMiniGridCells(cell.Column, cell.Row).Aggregate(possibleValues, (current, gridCell) => current.Replace(gridCell.Value.GetValueOrDefault().ToString(), string.Empty));

		return possibleValues;
	}

	public static string[,] GetAllPossibleValues(this Cell[] cells)
	{
		var possibleValues = new string[9, 9];

		cells.ToList().ForEach(x => possibleValues[x.Row, x.Column] = x.PossibleValues);

		return possibleValues;
	}

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

	public static void SetCellWithFewestPossibleValues(this Cell[] cells)
	{
		var cell = cells.FindCellWithFewestPossibleValues();
		var possibleValues = cell.PossibleValues.Randomize();

		if (string.IsNullOrWhiteSpace(possibleValues))
		{
			throw new InvalidOperationException("An invalid move was made");
		}

		cell.Value = int.Parse(possibleValues[0].ToString());
	}
}