using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Components;

public partial class GameStats : ComponentBase, IDisposable
{
    [Parameter] public int TotalMoves { get; set; }
    [Parameter] public int InvalidMoves { get; set; }
    [Parameter] public Func<TimeSpan> GetPlayDuration { get; set; }

    private PeriodicTimer _timer;
    private CancellationTokenSource _cts = new();

    protected override async Task OnInitializedAsync()
    {
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        _ = RefreshLoopAsync();
    }

    private async Task RefreshLoopAsync()
    {
        while (await _timer.WaitForNextTickAsync(_cts.Token))
        {
            InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _timer?.Dispose();
    }
}