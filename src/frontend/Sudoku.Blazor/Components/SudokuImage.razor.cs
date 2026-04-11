using Microsoft.AspNetCore.Components;

namespace Sudoku.Blazor.Components
{
    public partial class SudokuImage
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
