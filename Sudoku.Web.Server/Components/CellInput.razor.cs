using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Components;

public partial class CellInput : IDisposable
{
    [Parameter] 
    public Cell Cell { get; set; } = new(0, 0);

    [Parameter] 
    public string CssClass { get; set; } = string.Empty;
    
    [Parameter]
    public EventCallback<Cell> OnCellFocus { get; set; }

    [Inject] 
    private ICellFocusedNotificationService? NotificationService { get; set; }

    protected override void OnInitialized()
    {
        NotificationService!.SetCellFocus += HandleCellSetFocus;
    }

    private void HandleCellSetFocus(object? sender, Cell e)
    {
        if (ShouldHighlight(e))
        {
            CssClass = "highlight";
        }
        else
        {
            CssClass = string.Empty;
        }
    }

    private void OnFocus()
    {
        OnCellFocus.InvokeAsync(Cell);
    }

    public void Dispose()
    {
        NotificationService!.SetCellFocus -= HandleCellSetFocus;
    }

    private bool ShouldHighlight(Cell? cell)
    {
        return cell != null &&
               (Cell.Row == cell.Row ||
               Cell.Column == cell.Column ||
               Cell.IsInSameMiniGrid(cell));
    }
}