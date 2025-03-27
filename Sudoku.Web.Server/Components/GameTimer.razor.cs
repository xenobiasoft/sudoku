using System.Timers;
using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services;
using Timer = System.Timers.Timer;

namespace Sudoku.Web.Server.Components;

public partial class GameTimer : IDisposable
{
    private bool _isRunning = false;
    public TimeSpan ElapsedGameTime = TimeSpan.Zero;
    private DateTime _startTime = DateTime.Now;
    private readonly Timer _timer = new();

    [Inject]
    private IGameNotificationService? GameNotificationService { get; set; }

    protected override void OnInitialized()
    {
        _timer.Interval = 1000;
        _timer.Elapsed += ElapsedTime;
        GameNotificationService!.GameStarted += Start;
        GameNotificationService!.GameEnded += Stop;
    }

    private void ElapsedTime(object? sender, ElapsedEventArgs e)
    {
        InvokeAsync(() =>
        {
            ElapsedGameTime = DateTime.Now - _startTime;
            StateHasChanged();
        });
    }

    private void Start()
    {
        Reset();
        _timer.Start();
        _startTime = DateTime.Now;
        _isRunning = true;
    }

    private void Stop()
    {
        if (!_isRunning) return;

        _timer.Stop();
        ElapsedGameTime = DateTime.Now - _startTime;
        _isRunning = false;
    }

    private void Reset()
    {
        ElapsedGameTime = TimeSpan.Zero;
        _isRunning = false;
    }

    public void Dispose()
    {
        Stop();
        _timer.Elapsed -= ElapsedTime;
        _timer.Dispose();
        GameNotificationService!.GameStarted -= Start;
        GameNotificationService!.GameEnded -= Stop;
    }
}
