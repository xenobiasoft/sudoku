using System.Text.Json.Serialization;

namespace XenobiaSoft.Sudoku.GameState;

[method: JsonConstructor]
public class GameStateMemory(string puzzleId, Cell[] board) : PuzzleState(puzzleId, board)
{
    public GameStateMemory() : this(string.Empty, [])
    { }

    public int InvalidMoves { get; set; } = 0;
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
    public TimeSpan PlayDuration { get; set; } = TimeSpan.Zero;
    public DateTime StartTime { get; set; }
    public int TotalMoves { get; set; } = 0;
}