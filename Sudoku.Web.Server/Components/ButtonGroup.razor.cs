using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Components
{
    public partial class ButtonGroup
    {
        [Inject] public IGameStateManager? GameStateManager { get; set; }
        [Parameter] public EventCallback<int?> NumberClicked { get; set; }
        [Parameter] public string? PuzzleId { get; set; }

        private async Task SetValue(int? value)
        {
            await NumberClicked.InvokeAsync(value);
        }

        private async Task Undo()
        {
            await GameStateManager!.UndoAsync(PuzzleId!);
            StateHasChanged();
        }
    }
}
