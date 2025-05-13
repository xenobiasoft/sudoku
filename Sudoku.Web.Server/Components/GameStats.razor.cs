using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Components;

public partial class GameStats : ComponentBase, IDisposable
{
    [Inject] private IGameSessionManager SessionManager { get; set; } = default!;

    [Parameter] public int TotalMoves { get; set; }
    [Parameter] public int InvalidMoves { get; set; }

    protected override void OnInitialized()
    {
        if (SessionManager.CurrentSession is not { } session) return;

        session.Timer.OnTick += OnTimerTick;
        session.OnMoveRecorded += OnMoveRecorded;
    }

    private void OnTimerTick(object? sender, TimeSpan elapsedTime)
    {
        InvokeAsync(StateHasChanged);
    }

    private void OnMoveRecorded(object? sender, System.EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        if (SessionManager.CurrentSession is not { } session) return;

        session.Timer.OnTick -= OnTimerTick;
        session.OnMoveRecorded -= OnMoveRecorded;
    }
}