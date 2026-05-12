using Microsoft.AspNetCore.Components;

namespace Sudoku.Blazor.Components;

public partial class VictoryDisplay
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Parameter] public bool IsVictory { get; set; }
    [Parameter] public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;

    private void Start()
    {
        NavigationManager.NavigateTo("/");
    }
}