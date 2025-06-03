using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Abstractions;

public interface IGameSessionState
{
    string Alias { get; }
    string PuzzleId { get; }
    Cell[] Board { get; }
    int InvalidMoves { get; }
    int TotalMoves { get; }
    TimeSpan PlayDuration { get; }
    IGameTimer Timer { get; }
    event EventHandler? OnMoveRecorded;

    void RecordMove(bool isValid);
    void ReloadBoard(GameStateMemory gameState);
    void Start();
    void Pause();
    void Resume();
    void End();
}