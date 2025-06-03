using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Components;

public partial class GameStats : ComponentBase, IDisposable
{
    [Inject] public required IGameSessionManager SessionManager { get; set; } = null!;

    private int _totalMoves = 0;
    private int _invalidMoves = 0;
    private TimeSpan _playDuration = TimeSpan.Zero;
    private bool _isCollapsed = true;

    protected override void OnInitialized()
    {
        SubscribeToSessionEvents();
    }

    protected override void OnParametersSet()
    {
        UpdateStats();
    }

    private void SubscribeToSessionEvents()
    {
        if (SessionManager.CurrentSession is not { } session) return;

        session.Timer.OnTick += OnTimerTick;
        session.OnMoveRecorded += OnMoveRecorded;
    }

    private void UnsubscribeFromSessionEvents()
    {
        if (SessionManager.CurrentSession is not { } session) return;

        session.Timer.OnTick -= OnTimerTick;
        session.OnMoveRecorded -= OnMoveRecorded;
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
        UnsubscribeFromSessionEvents();
    }

    private void ToggleCollapse()
    {
        _isCollapsed = !_isCollapsed;
        StateHasChanged();
    }

    private void UpdateStats()
    {
        if (SessionManager.CurrentSession is not { } session) return;

        _invalidMoves = session.InvalidMoves;
        _totalMoves = session.TotalMoves;
        _playDuration = session.PlayDuration;
    }
}