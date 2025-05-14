namespace Sudoku.Web.Server.Services;

public class GameTimer : IGameTimer, IDisposable
{
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts;
    private DateTime? _startTime;
    private DateTime? _pauseTime;
    private TimeSpan _accumulatedTime;
    private bool _isRunning;

    public TimeSpan ElapsedTime => _accumulatedTime + GetCurrentSessionTime();
    public bool IsRunning => _isRunning;

    public event EventHandler<TimeSpan>? OnTick;

    public GameTimer(TimeSpan tickInterval)
    {
        _timer = new PeriodicTimer(tickInterval);
        _cts = new CancellationTokenSource();
        _accumulatedTime = TimeSpan.Zero;
        _isRunning = false;
    }

    public void Start()
    {
        if (_isRunning) return;
        
        _startTime = DateTime.UtcNow;
        _isRunning = true;
        _ = StartTimerLoop();
    }

    public void Pause()
    {
        if (!_isRunning) return;
        
        _pauseTime = DateTime.UtcNow;
        _accumulatedTime += GetCurrentSessionTime();
        _isRunning = false;
    }

    public void Resume()
    {
        if (_isRunning) return;
        
        _startTime = DateTime.UtcNow;
        _pauseTime = null;
        _isRunning = true;
        _ = StartTimerLoop();
    }

    public void Reset()
    {
        _startTime = null;
        _pauseTime = null;
        _accumulatedTime = TimeSpan.Zero;
        _isRunning = false;
    }

    private TimeSpan GetCurrentSessionTime()
    {
        if (!_startTime.HasValue) return TimeSpan.Zero;
        return DateTime.UtcNow - _startTime.Value;
    }

    private async Task StartTimerLoop()
    {
        while (_isRunning && await _timer.WaitForNextTickAsync(_cts.Token))
        {
            OnTick?.Invoke(this, ElapsedTime);
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _timer.Dispose();
    }
}