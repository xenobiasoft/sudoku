using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Components;

public partial class CellInput : IDisposable
{
    [Inject] 
    private ICellFocusedNotificationService? NotificationService { get; set; }

    [Parameter] 
    public Cell Cell { get; set; } = new(0, 0);

    [Parameter]
    public EventCallback<Cell> OnCellFocus { get; set; }

    private string CssClass { get; set; } = string.Empty;
    private ElementReference _element;

    protected override void OnInitialized()
    {
        NotificationService!.SetCellFocus += HandleCellSetFocus;
    }

    private void HandleCellSetFocus(object? sender, Cell e)
    {
        if (Cell == e)
        {
            _element.FocusAsync();
        }

        CssClass = ShouldHighlight(e) ? "highlight" : string.Empty;
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