using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.States;

public class NewGameSessionState(GameStateMemory gameState, IGameTimer timer) : IGameSessionState
{
    public string Alias => gameState.Alias ?? string.Empty;
    public string PuzzleId => gameState.PuzzleId;
    public Cell[] Board => gameState.Board;
    public int InvalidMoves => gameState.InvalidMoves;
    public int TotalMoves => gameState.TotalMoves;
    public TimeSpan PlayDuration => timer.ElapsedTime;
    public IGameTimer Timer => timer;

    public event EventHandler? OnMoveRecorded;

    public void End()
    {
        timer.Pause();
    }

    public void Pause()
    {
        // New game state doesn't pause
    }

    public void RecordMove(bool isValid)
    {
        // New game state doesn't record moves
    }

    public void ReloadBoard(GameStateMemory gameState)
    {
        // New game state doesn't reload board
    }

    public void Resume()
    {
        // New game state doesn't resume
    }

    public void Start()
    {
        timer.Start();
    }
}