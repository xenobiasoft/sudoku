using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// Represents a null game session that does nothing
/// </summary>
public class NullGameSession : IGameSession
{
    public static NullGameSession Instance { get; } = new();

    public bool IsNull => true;
    public string Alias => string.Empty;
    public string PuzzleId => string.Empty;
    public Cell[] Board => [];
    public int InvalidMoves => 0;
    public int TotalMoves => 0;
    public TimeSpan PlayDuration => TimeSpan.Zero;
    public IGameTimer Timer => new NullGameTimer();

    public event EventHandler? OnMoveRecorded;

    public void ChangeState(IGameSessionState sessionState) { }
    public void End() { }
    public void IncrementInvalidMoves() { }
    public void IncrementTotalMoves() { }
    public void Pause() { }
    public void RecordMove(bool isValid) { }
    public void ReloadBoard(GameStateMemory gameState) { }
    public void ReloadGameState(GameStateMemory gameState) { }
    public void Resume() { }
    public void Start() { }
}