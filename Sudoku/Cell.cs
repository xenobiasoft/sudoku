namespace XenobiaSoft.Sudoku;

public record Cell
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
    public bool Locked { get; set; }

	public Cell Copy()
	{
		return new Cell(Row, Column)
		{
			Value = Value,
			PossibleValues = PossibleValues,
		};
	}
}