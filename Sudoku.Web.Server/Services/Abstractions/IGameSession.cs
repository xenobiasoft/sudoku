using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Abstractions;

public interface IGameSession
{
    string PuzzleId { get; }
    Cell[] Board { get; }
    int InvalidMoves { get; }
    int TotalMoves { get; }
    TimeSpan PlayDuration { get; }
    IGameTimer Timer { get; }
    event EventHandler? OnMoveRecorded;
    void RecordMove(bool isValid);
    void ReloadBoard(GameStateMemory gameState);
} 