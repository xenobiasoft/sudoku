using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Components;

public partial class CellInput
{
    [Parameter]
    public int Column { get; set; }

    [Parameter]
    public int Row { get; set; }

    [Parameter] public int? Value { get; set; }
}