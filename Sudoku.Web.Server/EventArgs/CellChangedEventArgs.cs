using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.EventArgs;

public record CellChangedEventArgs(int Row, int Column, int Value)
{
    public CellChangedEventArgs(CellModel cell) : this(cell.Row, cell.Column, cell.Value.GetValueOrDefault())
    {
    }
}