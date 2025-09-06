namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents game statistics
/// </summary>
public class GameStatisticsModel
{
    public int TotalMoves { get; private set; }

    public int ValidMoves => TotalMoves - InvalidMoves;

    public int InvalidMoves { get; private set; }

    public TimeSpan PlayDuration { get; private set; }

    public void RecordMove(bool isValid)
    {
        TotalMoves++;
        if (!isValid) InvalidMoves++;
    }

    public void Reset()
    {
        TotalMoves = 0;
        InvalidMoves = 0;
        PlayDuration = TimeSpan.Zero;
    }

    public void SetPlayDuration(TimeSpan duration) => PlayDuration = duration;
}