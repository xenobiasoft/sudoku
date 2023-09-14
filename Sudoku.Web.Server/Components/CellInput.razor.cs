using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Components;

public partial class CellInput
{
    [Parameter] 
    public Cell Cell { get; set; }
}