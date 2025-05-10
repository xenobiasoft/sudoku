using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Components;

public partial class GameStats : ComponentBase
{
    [Parameter] public int TotalMoves { get; set; }
    [Parameter] public int InvalidMoves { get; set; }
    [Parameter] public TimeSpan PlayDuration { get; set; }
}