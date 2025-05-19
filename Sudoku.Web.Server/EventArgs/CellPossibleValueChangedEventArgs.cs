namespace Sudoku.Web.Server.EventArgs;

public record CellPossibleValueChangedEventArgs(int Value)
{
    public CellPossibleValueChangedEventArgs(int row, int column, int Value) : this(Value)
    {
        Row = row;
        Column = column;
    }

    public int? Row { get; private set; }
    public int? Column { get; private set; }
}