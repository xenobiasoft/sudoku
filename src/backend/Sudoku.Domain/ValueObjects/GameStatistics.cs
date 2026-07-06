namespace Sudoku.Domain.ValueObjects;

public record GameStatistics
{
    /// <summary>Maximum number of hints a player may use per game (all difficulties).</summary>
    public const int MaxHints = 3;

    public int TotalMoves { get; private set; }
    public int ValidMoves { get; private set; }
    public int InvalidMoves { get; private set; }
    public int HintsUsed { get; private set; }
    public TimeSpan PlayDuration { get; private set; }
    public DateTime? LastMoveAt { get; private set; }

    private GameStatistics()
    {
        TotalMoves = 0;
        ValidMoves = 0;
        InvalidMoves = 0;
        HintsUsed = 0;
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
    
    public void UndoMove()
    {
        if (TotalMoves <= 0) return;

        TotalMoves--;
        ValidMoves--;
    }

    public void RecordHint()
    {
        HintsUsed++;
    }

    public void UpdatePlayDuration(TimeSpan duration)
    {
        PlayDuration = duration;
    }

    public double AccuracyPercentage => TotalMoves > 0 ? (double)ValidMoves / TotalMoves * 100 : 0;

    public int HintsRemaining => Math.Max(0, MaxHints - HintsUsed);

    public bool HasMoves => TotalMoves > 0;
}