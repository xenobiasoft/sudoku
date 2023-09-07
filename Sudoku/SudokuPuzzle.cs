namespace XenobiaSoft.Sudoku;

public class SudokuPuzzle
{
	public const int Rows = 9;
	public const int Columns = 9;

	public SudokuPuzzle()
	{
		Initialize();
	}

	public Cell[] Cells { get; set; } = new Cell[Columns * Rows];

	public IEnumerable<Cell> GetRowCells(int row)
	{
		for (var col = 0; col < Columns; col++)
		{
			yield return Cells[col * 9 + row];
		}
	}

	public IEnumerable<Cell> GetColumnCells(int col)
	{
		for (var row = 0; row < Rows; row++)
		{
			yield return Cells[col * 9 + row];
		}
	}

	public IEnumerable<Cell> GetMiniGridCells(int row, int col)
	{
		var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);
		var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartCol(row);

		for (var miniGridRow = miniGridStartRow; miniGridRow <= miniGridStartRow + 2; miniGridRow++)
		{
			for (var miniGridCol = miniGridStartCol; miniGridCol <= miniGridStartCol + 2; miniGridCol++)
			{
				yield return Cells[miniGridCol * 9 + miniGridRow];
			}
		}
	}

	public Cell GetCell(int row, int col)
	{
		return Cells.First(x => x.Column == col && x.Row == row);
	}

	public string[,] GetCellPossibleValues()
	{
		var possibleValues = new string[9, 9];

		Cells
			.ToList()
			.ForEach(x => possibleValues[x.Row, x.Column] = x.PossibleValues);
		
		return possibleValues;
	}

	public int[,] GetCellValues()
	{
		var cellValues = new int[9, 9];

		Cells
			.ToList()
			.ForEach(x => cellValues[x.Row, x.Column] = x.Value.GetValueOrDefault());

		return cellValues;
	}

	public void RestoreValues(int[,] values)
	{
		for (var col = 0; col < values.GetLength(0); col++)
		{
			for (var row = 0; row < values.GetLength(1); row++)
			{
				GetCell(row, col).Value = values[row, col] > 0 ? values[row, col] : null;
			}
		}
	}

	public void PopulatePossibleValues()
	{
		foreach (var cell in Cells)
		{
			cell.PossibleValues = cell.Value.HasValue ? string.Empty : CalculatePossibleValues(cell);
		}
	}

	public Cell FindCellWithFewestPossibleValues()
	{
		var cellPossibleValues = Cells
			.Where(x => !x.Value.HasValue)
			.OrderBy(x => x.PossibleValues.Length)
			.ThenBy(x => x.Column)
			.ThenBy(x => x.Row)
			.ToList();

		return cellPossibleValues.First();
	}

	public void SetCellWithFewestPossibleValues()
	{
		var cell = FindCellWithFewestPossibleValues();
		var possibleValues = cell.PossibleValues.Randomize();

		if (string.IsNullOrWhiteSpace(possibleValues))
		{
			throw new InvalidOperationException("An invalid move was made");
		}

		cell.Value = int.Parse(possibleValues[0].ToString());
	}

	public void Reset()
	{
		Initialize();
	}

	private string CalculatePossibleValues(Cell cell)
	{
		var possibleValues = string.IsNullOrWhiteSpace(cell.PossibleValues) ? "123456789" : cell.PossibleValues;

		foreach (var columnCell in GetColumnCells(cell.Column))
		{
			possibleValues = possibleValues.Replace(columnCell.Value.GetValueOrDefault().ToString(), string.Empty);
		}

		foreach (var rowCell in GetRowCells(cell.Row))
		{
			possibleValues = possibleValues.Replace(rowCell.Value.GetValueOrDefault().ToString(), string.Empty);
		}

		foreach (var gridCell in GetMiniGridCells(cell.Column, cell.Row))
		{
			possibleValues = possibleValues.Replace(gridCell.Value.GetValueOrDefault().ToString(), string.Empty);
		}

		return possibleValues;
	}

	private void Initialize()
	{
		var index = 0;
		for (var col = 0; col < Columns; col++)
		{
			for (var row = 0; row < Rows; row++)
			{
				Cells[index++] = new Cell(row, col);
			}
		}
	}

	public void Restore(IEnumerable<Cell> cells)
	{
		Cells = cells.ToArray();
	}
}