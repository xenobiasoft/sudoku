namespace Sudoku.Web.Server.Services;

public class NullGameTimer : IGameTimer
{
    public TimeSpan ElapsedTime => TimeSpan.Zero;
    public bool IsRunning => false;

    public void Start() { }
    public void Pause() { }
    public void Resume() { }
    public void Reset() { }

    public event EventHandler<TimeSpan>? OnTick;
}