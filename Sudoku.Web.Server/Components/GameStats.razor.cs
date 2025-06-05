using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Components;

public partial class GameStats : ComponentBase, IDisposable
{
    [Parameter] public required IGameSession Session { get; set; } = null!;

    private int _totalMoves;
    private int _invalidMoves;
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
        if (Session.IsNull) return;

        Session.Timer.OnTick += OnTimerTick;
        Session.OnMoveRecorded += OnMoveRecorded;
    }

    private void UnsubscribeFromSessionEvents()
    {
        if (Session.IsNull) return;

        Session.Timer.OnTick -= OnTimerTick;
        Session.OnMoveRecorded -= OnMoveRecorded;
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
        if (Session.IsNull) return;

        _invalidMoves = Session.InvalidMoves;
        _totalMoves = Session.TotalMoves;
        _playDuration = Session.PlayDuration;
    }
}