using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.EventArgs;

namespace Sudoku.Web.Server.Components
{
    public partial class GameControls
    {
        [Parameter] public EventCallback<CellValueChangedEventArgs> OnValueChanged { get; set; }
        [Parameter] public EventCallback<CellPossibleValueChangedEventArgs> OnPossibleValueChanged { get; set; }
        [Parameter] public string? PuzzleId { get; set; }
        [Parameter] public EventCallback OnReset { get; set; }
        [Parameter] public EventCallback OnUndo { get; set; }
        [Parameter] public EventCallback<bool> OnPencilMode { get; set; }
        [Parameter] public EventCallback OnHome { get; set; }
        [Parameter] public int TotalMoves { get; set; }
        [Parameter] public bool IsPencilMode { get; set; }

        private async Task SetValue(int? value)
        {
            if (IsPencilMode)
            {
                await OnPossibleValueChanged.InvokeAsync(new CellPossibleValueChangedEventArgs(value.GetValueOrDefault()));
            }
            else
            {
                await OnValueChanged.InvokeAsync(new CellValueChangedEventArgs(value));
            }
        }

        private async Task Reset()
        {
            await OnReset.InvokeAsync();
        }

        private async Task TogglePencilMode()
        {
            IsPencilMode = !IsPencilMode;
            await OnPencilMode.InvokeAsync(IsPencilMode);
        }

        private async Task Undo()
        {
            await OnUndo.InvokeAsync();
        }

        private async Task Home()
        {
            await OnHome.InvokeAsync();
        }
    }
}
