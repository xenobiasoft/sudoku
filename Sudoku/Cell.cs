namespace XenobiaSoft.Sudoku;

public class Cell
{
	public Cell(int row, int col)
	{
		Column = col;
		Row = row;
	}

	public int Column { get; }
	public int Row { get; }
	public string PossibleValues { get; set; } = string.Empty;
	public int? Value { get; set; } = null;
}