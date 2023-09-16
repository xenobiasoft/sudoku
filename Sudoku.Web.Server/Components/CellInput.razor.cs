using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Components;

public partial class CellInput
{
    [Parameter] 
    public Cell Cell { get; set; }
    
    [Parameter]
    public EventCallback<Cell> OnCellFocus { get; set; }

    private void OnFocus()
    {
        OnCellFocus.InvokeAsync(Cell);
    }
}