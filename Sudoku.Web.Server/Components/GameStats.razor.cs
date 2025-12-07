using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Components;

public partial class GameStats : ComponentBase, IDisposable
{
    [Parameter] public required IGameStatisticsManager GameManager { get; set; }

    private int _totalMoves;
    private int _invalidMoves;
    private TimeSpan _playDuration = TimeSpan.Zero;
    private bool _isCollapsed = true;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender) return;

        SubscribeToSessionEvents();
    }

    protected override void OnParametersSet()
    {
        UpdateStats();
    }

    private void SubscribeToSessionEvents()
    {
        GameManager.Timer.OnTick += OnTimerTick;
    }

    private void UnsubscribeFromSessionEvents()
    {
        GameManager.Timer.OnTick -= OnTimerTick;
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
        _invalidMoves = GameManager.CurrentStatistics.InvalidMoves;
        _totalMoves = GameManager.CurrentStatistics.TotalMoves;
        _playDuration = GameManager.CurrentStatistics.PlayDuration;
    }
}