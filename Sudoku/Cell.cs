namespace XenobiaSoft.Sudoku;

[Obsolete("Use Sudoku.Domain.Cell instead")]
public record Cell(int Row, int Column)
{
    public List<int> PossibleValues { get; set; } = new();
	public int? Value { get; set; }
    public bool Locked { get; set; }

	public Cell Copy()
	{
		return new Cell(Row, Column)
		{
			Value = Value,
			Locked = Locked,
            PossibleValues = PossibleValues,
		};
	}
}