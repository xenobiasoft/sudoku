using Microsoft.AspNetCore.Components;

namespace Sudoku.Blazor.Components.Controls
{
    public partial class SudokuImage
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
