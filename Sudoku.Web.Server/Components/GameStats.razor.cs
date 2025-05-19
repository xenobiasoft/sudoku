using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Components;

public partial class GameStats : ComponentBase, IDisposable
{
    [Inject] private IGameSessionManager SessionManager { get; set; } = null!;

    private int _totalMoves = 0;
    private int _invalidMoves = 0;
    private TimeSpan _playDuration = TimeSpan.Zero;
    private bool _isCollapsed = true;

    protected override void OnInitialized()
    {
        if (SessionManager.CurrentSession is not { } session) return;

        session.Timer.OnTick += OnTimerTick;
        session.OnMoveRecorded += OnMoveRecorded;
    }

    private void OnTimerTick(object? sender, TimeSpan elapsedTime)
    {
        UpdateStats();
        InvokeAsync(StateHasChanged);
    }

    private void OnMoveRecorded(object? sender, System.EventArgs e)
    {
        UpdateStats();
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        if (SessionManager.CurrentSession is not { } session) return;

        session.Timer.OnTick -= OnTimerTick;
        session.OnMoveRecorded -= OnMoveRecorded;
    }

    private void ToggleCollapse()
    {
        _isCollapsed = !_isCollapsed;
    }

    private void UpdateStats()
    {
        _invalidMoves = SessionManager.CurrentSession.InvalidMoves;
        _totalMoves = SessionManager.CurrentSession.TotalMoves;
        _playDuration = SessionManager.CurrentSession.PlayDuration;
    }
}