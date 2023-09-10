namespace XenobiaSoft.Sudoku;

public class Cell : ICloneable
{
	public Cell(int row, int col)
	{
		Column = col;
		Row = row;
	}

	public int Column { get; }
	public int Row { get; }
	public string PossibleValues { get; set; } = string.Empty;
	public int? Value { get; set; }

	public object Clone()
	{
		return new Cell(Row, Column)
		{
			Value = Value,
			PossibleValues = PossibleValues,
		};
	}
}