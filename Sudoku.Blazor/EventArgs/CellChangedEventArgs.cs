using Sudoku.Blazor.Models;

namespace Sudoku.Blazor.EventArgs;

public record CellChangedEventArgs(int Row, int Column, int Value)
{
    public CellChangedEventArgs(CellModel cell) : this(cell.Row, cell.Column, cell.Value.GetValueOrDefault())
    {
    }
}