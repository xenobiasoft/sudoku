using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Components
{
    public partial class SudokuImage
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
