using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class NullGameSession : IGameSession
{
    public static NullGameSession Instance = new NullGameSession();
    
    public string PuzzleId => string.Empty;
    public Cell[] Board => [];
    public int InvalidMoves => 0;
    public int TotalMoves => 0;
    public DateTime StartTime => DateTime.MinValue;
    public TimeSpan PlayDuration => TimeSpan.Zero;
    public IGameTimer Timer => new NullGameTimer();

    public event EventHandler? OnMoveRecorded;

    public void RecordMove(bool isValid) { }
    public void ReloadBoard(GameStateMemory gameState) { }
}