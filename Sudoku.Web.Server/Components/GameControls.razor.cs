using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.EventArgs;

namespace Sudoku.Web.Server.Components
{
    public partial class GameControls
    {
        [Parameter] public EventCallback<CellValueChangedEventArgs> OnNumberClicked { get; set; }
        [Parameter] public string? PuzzleId { get; set; }
        [Parameter] public EventCallback OnReset { get; set; }
        [Parameter] public EventCallback OnUndo { get; set; }
        [Parameter] public int TotalMoves { get; set; }

        private async Task SetValue(int? value)
        {
            await OnNumberClicked.InvokeAsync(new CellValueChangedEventArgs(value.GetValueOrDefault()));
        }

        private async Task Reset()
        {
            await OnReset.InvokeAsync();
        }

        private async Task Undo()
        {
            await OnUndo.InvokeAsync();
        }
    }
}
