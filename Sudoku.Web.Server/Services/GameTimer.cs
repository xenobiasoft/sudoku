using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Services;

public class GameTimer(TimeSpan tickInterval) : IGameTimer, IDisposable
{
    private PeriodicTimer? _timer;
    private CancellationTokenSource? _cts;
    private DateTime? _startTime;
    private TimeSpan _accumulatedTime = TimeSpan.Zero;
    private bool _isRunning = false;
    private readonly Lock _lock = new();

    public TimeSpan ElapsedTime => _accumulatedTime + GetCurrentSessionTime();
    public bool IsRunning => _isRunning;

    public event EventHandler<TimeSpan>? OnTick;

    public void Start()
    {
        lock (_lock)
        {
            DisposeTimer();
            _startTime = DateTime.UtcNow;
            _isRunning = true;
            ResumeTimer();
        }
    }

    public void Pause()
    {
        lock (_lock)
        {
            if (!_isRunning) return;
            _accumulatedTime += GetCurrentSessionTime();
            _isRunning = false;
        }
    }

    public void Resume()
    {
        Resume(TimeSpan.Zero);
    }

    public void Resume(TimeSpan initialDuration)
    {
        lock (_lock)
        {
            if (_isRunning) return;
            _startTime = DateTime.UtcNow;
            _accumulatedTime = initialDuration;
            _isRunning = true;
            ResumeTimer();
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            DisposeTimer();
            _startTime = null;
            _accumulatedTime = TimeSpan.Zero;
            _isRunning = false;
        }
    }

    private TimeSpan GetCurrentSessionTime()
    {
        if (!_startTime.HasValue) return TimeSpan.Zero;
        return DateTime.UtcNow - _startTime.Value;
    }

    private void ResumeTimer()
    {
        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(tickInterval);
        Task.Run(() => StartTimerLoop(_cts.Token, _timer));
    }

    private async Task StartTimerLoop(CancellationToken token, PeriodicTimer? timer)
    {
        try
        {
            while (true)
            {
                if (!_isRunning || timer == null) break;
                if (!await timer.WaitForNextTickAsync(token)) break;
                OnTick?.Invoke(this, ElapsedTime);
            }
        }
        catch (OperationCanceledException) { }
    }

    private void DisposeTimer()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        _timer?.Dispose();
        _timer = null;
        _isRunning = false;
    }

    public void Dispose()
    {
        DisposeTimer();
    }
}