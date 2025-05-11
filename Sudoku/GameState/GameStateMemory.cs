using System.Text.Json.Serialization;

namespace XenobiaSoft.Sudoku.GameState;

[method: JsonConstructor]
public class GameStateMemory(string puzzleId, Cell[] board) : PuzzleState(puzzleId, board)
{
    public GameStateMemory() : this(string.Empty, [])
    { }

    public int InvalidMoves { get; set; } = 0;
    public DateTime? LastResumeTime { set; get; }
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
    public TimeSpan PlayDuration { get; set; } = TimeSpan.Zero;
    public DateTime StartTime { get; set; }
    public int TotalMoves { get; set; } = 0;

    public void Pause()
    {
        if (LastResumeTime.HasValue)
        {
            PlayDuration += DateTime.UtcNow - LastResumeTime.Value;
        }
    }

    public void Resume()
    {
        LastResumeTime ??= DateTime.UtcNow;
    }

    public TimeSpan GetTotalPlayDuration()
    {
        return PlayDuration + (LastResumeTime.HasValue ? DateTime.UtcNow - LastResumeTime.Value : TimeSpan.Zero);
    }
}