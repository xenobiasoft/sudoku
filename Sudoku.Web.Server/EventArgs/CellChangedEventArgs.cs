namespace Sudoku.Web.Server.EventArgs;

public record CellChangedEventArgs(int Row, int Column, int? Value)
{
    public CellChangedEventArgs(Cell cell) : this(cell.Row, cell.Column, cell.Value)
    {
    }
}