namespace Sudoku.Web.Server.Services;

public class GameSession : IGameSession
{
    private readonly IGameTimer _timer;

    public GameSession(string puzzleId, Cell[] board, IGameTimer timer)
    {
        _timer = timer;
        PuzzleId = puzzleId;
        Board = board;
    }

    public string PuzzleId { get; }
    public Cell[] Board { get; }
    public int InvalidMoves { get; private set; }
    public int TotalMoves { get; private set; }
    public DateTime StartTime { get; } = DateTime.UtcNow;
    public TimeSpan PlayDuration => _timer.ElapsedTime;
    public IGameTimer Timer => _timer;

    public event EventHandler? OnMoveRecorded;

    public void RecordMove(bool isValid)
    {
        TotalMoves++;
        if (!isValid) InvalidMoves++;
        OnMoveRecorded?.Invoke(this, System.EventArgs.Empty);
    }
}