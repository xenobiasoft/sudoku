using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Components
{
    public partial class ButtonGroup
    {
        [Parameter] public EventCallback<int?> NumberClicked { get; set; }
        [Parameter] public string? PuzzleId { get; set; }
        [Parameter] public EventCallback OnUndo { get; set; }

        private async Task SetValue(int? value)
        {
            await NumberClicked.InvokeAsync(value);
        }

        private async Task Undo()
        {
            await OnUndo.InvokeAsync();
        }
    }
}
