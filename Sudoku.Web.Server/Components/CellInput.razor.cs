using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Components;

public partial class CellInput : IDisposable
{
    [Inject] private ICellFocusedNotificationService? CellFocusedNotificationService { get; set; }
    [Inject] private IInvalidCellNotificationService? InvalidCellNotificationService { get; set; }

    [Parameter] public bool IsPencilMode { get; set; }
    [Parameter] public Cell Cell { get; set; } = new(0, 0);
    [Parameter] public EventCallback<Cell> OnCellFocus { get; set; }
    [Parameter] public EventCallback<CellChangedEventArgs> OnCellChanged { get; set; }
    [Parameter] public EventCallback<CellPossibleValueChangedEventArgs> OnPossibleValueChanged { get; set; }
    [Parameter] public ISudokuPuzzle? Puzzle { get; set; }

    private string CssClass { get; set; } = string.Empty;

    private ElementReference _element;

    protected override void OnInitialized()
    {
        CellFocusedNotificationService!.SetCellFocus += HandleCellSetFocus;
        InvalidCellNotificationService!.NotifyInvalidCells += HandleInvalidCells;
    }

    private void HandleInvalidCells(object? sender, IEnumerable<Cell> e)
    {
        CssClass = e.Contains(Cell) ? "invalid" : string.Empty;
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
        CellFocusedNotificationService!.Notify(Cell);
    }

    public void Dispose()
    {
        CellFocusedNotificationService!.SetCellFocus -= HandleCellSetFocus;
        InvalidCellNotificationService!.NotifyInvalidCells -= HandleInvalidCells;
    }

    private bool ShouldHighlight(Cell? cell)
    {
        return cell != null &&
               (Cell.Row == cell.Row ||
               Cell.Column == cell.Column ||
               Cell.IsInSameMiniGrid(cell));
    }

    private async Task KeyPressAsync(KeyboardEventArgs e)
    {
        int.TryParse(e.Key, out var cellValue);

        if (cellValue != Cell.Value)
        {

            if (IsPencilMode)
            {
                await OnPossibleValueChanged.InvokeAsync(
                    new CellPossibleValueChangedEventArgs(Cell.Row, Cell.Column, cellValue));
            }
            else
            {
                Cell.Value = cellValue;
                Cell.PossibleValues = [];
                await OnCellChanged.InvokeAsync(new CellChangedEventArgs(Cell));
            }
        }
    }
}