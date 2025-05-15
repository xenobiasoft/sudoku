namespace Sudoku.Web.Server.Services;

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
} 