using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.States;

public class NullGameSessionState : IGameSessionState
{
    public static NullGameSessionState Instance { get; } = new();

    public string Alias => string.Empty;
    public string PuzzleId => string.Empty;
    public Cell[] Board => [];
    public int InvalidMoves => 0;
    public int TotalMoves => 0;
    public TimeSpan PlayDuration => TimeSpan.Zero;
    public IGameTimer Timer => new NullGameTimer();

    public event EventHandler? OnMoveRecorded;

    public void RecordMove(bool isValid) { }
    public void ReloadBoard(GameStateMemory gameState) { }
    public void Start() { }
    public void Pause() { }
    public void Resume() { }
    public void End() { }
}