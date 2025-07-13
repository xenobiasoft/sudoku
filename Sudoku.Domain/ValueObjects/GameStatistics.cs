namespace Sudoku.Domain.ValueObjects;

public record GameStatistics
{
    public int TotalMoves { get; private set; }
    public int ValidMoves { get; private set; }
    public int InvalidMoves { get; private set; }
    public TimeSpan PlayDuration { get; private set; }
    public DateTime? LastMoveAt { get; private set; }

    private GameStatistics()
    {
        TotalMoves = 0;
        ValidMoves = 0;
        InvalidMoves = 0;
        PlayDuration = TimeSpan.Zero;
        LastMoveAt = null;
    }

    public static GameStatistics Create() => new();

    public void RecordMove(bool isValid)
    {
        TotalMoves++;
        if (isValid)
        {
            ValidMoves++;
        }
        else
        {
            InvalidMoves++;
        }

        LastMoveAt = DateTime.UtcNow;
    }

    public void UpdatePlayDuration(TimeSpan duration)
    {
        PlayDuration = duration;
    }

    public double AccuracyPercentage => TotalMoves > 0 ? (double)ValidMoves / TotalMoves * 100 : 0;

    public bool HasMoves => TotalMoves > 0;
}