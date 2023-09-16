using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Components;

public partial class CellInput
{
    [Parameter] 
    public Cell Cell { get; set; }
    
    [Parameter]
    public EventCallback<CellFocusEventArgs> OnCellFocus { get; set; }

    private void OnFocus()
    {
        OnCellFocus.InvokeAsync(new CellFocusEventArgs { Column = Cell.Column, Row = Cell.Row });
    }
}