using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Components;

public partial class VictoryDisplay
{
    [Parameter] public bool IsVictory { get; set; }
    [Parameter] public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;
    [Parameter] public EventCallback NewGame { get; set; }
}