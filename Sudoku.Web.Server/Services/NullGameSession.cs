using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// Represents a null game session that does nothing
/// </summary>
public class NullGameSession : GameSessionBase
{
    public static NullGameSession Instance { get; } = new();

    public override bool IsNull => true;
    public override string Alias => string.Empty;
    public override string PuzzleId => string.Empty;
    public override Cell[] Board => [];
    public override int InvalidMoves => 0;
    public override int TotalMoves => 0;
    public override TimeSpan PlayDuration => TimeSpan.Zero;
    public override IGameTimer Timer => new NullGameTimer();

    public override event EventHandler? OnMoveRecorded;

    public override void RecordMove(bool isValid) { }
    public override void ReloadBoard(GameStateMemory gameState) { }
    public override void Start() { }
    public override void Pause() { }
    public override void Resume() { }
    public override void End() { }
}