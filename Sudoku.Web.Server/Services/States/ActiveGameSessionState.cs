using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.States;

public class ActiveGameSessionState(GameStateMemory gameState, IGameTimer timer) : IGameSessionState
{
    private int _invalidMoves = gameState.InvalidMoves;
    private int _totalMoves = gameState.TotalMoves;
    private Cell[] _board = gameState.Board;

    public string Alias => gameState.Alias ?? string.Empty;
    public string PuzzleId => gameState.PuzzleId;
    public Cell[] Board => _board;
    public int InvalidMoves => _invalidMoves;
    public int TotalMoves => _totalMoves;
    public TimeSpan PlayDuration => timer.ElapsedTime;
    public IGameTimer Timer => timer;

    public event EventHandler? OnMoveRecorded;

    public void End()
    {
        timer.Pause();
    }

    public void Pause()
    {
        timer.Pause();
    }

    public void RecordMove(bool isValid)
    {
        _totalMoves++;
        if (!isValid) _invalidMoves++;
        OnMoveRecorded?.Invoke(this, System.EventArgs.Empty);
    }

    public void ReloadBoard(GameStateMemory gameState)
    {
        _board = gameState.Board;
    }

    public void Resume()
    {
        timer.Resume();
    }

    public void Start()
    {
        // Active game state doesn't start
    }
}