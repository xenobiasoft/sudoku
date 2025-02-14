namespace XenobiaSoft.Sudoku;

public record Cell(int Row, int Column)
{
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