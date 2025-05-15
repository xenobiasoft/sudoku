using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class GameSession(GameStateMemory gameState, IGameTimer timer) : IGameSession
{
    public string PuzzleId { get; } = gameState.PuzzleId;
    public Cell[] Board { get; } = gameState.Board;
    public int InvalidMoves { get; private set; } = gameState.InvalidMoves;
    public int TotalMoves { get; private set; } = gameState.TotalMoves;
    public DateTime StartTime { get; } = gameState.StartTime;
    public TimeSpan PlayDuration => timer.ElapsedTime;
    public IGameTimer Timer => timer;

    public event EventHandler? OnMoveRecorded;

    public void RecordMove(bool isValid)
    {
        TotalMoves++;
        if (!isValid) InvalidMoves++;
        OnMoveRecorded?.Invoke(this, System.EventArgs.Empty);
    }
}