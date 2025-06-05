using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.States;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// Represents an active game session
/// </summary>
public class GameSession : IGameSession
{
    private IGameSessionState _state;

    /// <summary>
    /// Represents an active game session
    /// </summary>
    public GameSession(GameStateMemory gameState, IGameTimer timer)
    {
        Timer = timer;
        ReloadGameState(gameState);
        _state = new NewGameSessionState(this);
    }

    public event EventHandler? OnMoveRecorded;

    public string Alias { get; private set; } = string.Empty;
    public Cell[] Board { get; private set; } = [];
    public int InvalidMoves { get; private set; }
    public bool IsNull => false;
    public TimeSpan PlayDuration { get; private set; } = TimeSpan.Zero;
    public string PuzzleId { get; private set; } = string.Empty;
    public IGameTimer Timer { get; }
    public int TotalMoves { get; private set; }

    public void ChangeState(IGameSessionState sessionState)
    {
        _state = sessionState;
    }

    public void End()
    {
        _state.End();
    }

    public void IncrementInvalidMoves()
    {
        InvalidMoves++;
        OnMoveRecorded?.Invoke(this, System.EventArgs.Empty);
    }

    public void IncrementTotalMoves()
    {
        TotalMoves++;
        OnMoveRecorded?.Invoke(this, System.EventArgs.Empty);
    }

    public void Pause()
    {
        _state.Pause();
    }

    public void RecordMove(bool isValid)
    {
        _state.RecordMove(isValid);
    }

    public void ReloadBoard(GameStateMemory gameState)
    {
        _state.ReloadBoard(gameState);
    }

    public void ReloadGameState(GameStateMemory gameState)
    {
        Alias = gameState.Alias;
        Board = gameState.Board;
        InvalidMoves = gameState.InvalidMoves;
        PlayDuration = gameState.PlayDuration;
        PuzzleId = gameState.PuzzleId;
        TotalMoves = gameState.TotalMoves;
    }

    public void Resume()
    {
        _state.Resume();
    }

    public void Start()
    {
        _state.Start();
    }
}