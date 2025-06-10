namespace Sudoku.Web.Server.Services.Abstractions;

public interface IGameTimer
{
    TimeSpan ElapsedTime { get; }
    bool IsRunning { get; }
    void Start();
    void Pause();
    void Resume();
    void Resume(TimeSpan initialDuration);
    void Reset();
    event EventHandler<TimeSpan> OnTick;
}