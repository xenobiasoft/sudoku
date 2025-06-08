using System.Text.Json.Serialization;

namespace XenobiaSoft.Sudoku.GameState;

[method: JsonConstructor]
public class GameStateMemory
{
    public string Alias { get; set; }
    public Cell[] Board { get; set; } = [];
    public int InvalidMoves { get; set; } = 0;
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
    public TimeSpan PlayDuration { get; set; } = TimeSpan.Zero;
    public string PuzzleId { get; set; } = string.Empty;
    public int TotalMoves { get; set; } = 0;

    public GameStateMemory Clone()
    {
        return new GameStateMemory
        {
            Alias = this.Alias,
            Board = this.Board?.Select(cell => cell?.Copy()).ToArray(),
            InvalidMoves = this.InvalidMoves,
            LastUpdated = this.LastUpdated,
            PlayDuration = this.PlayDuration,
            PuzzleId = this.PuzzleId,
            TotalMoves = this.TotalMoves
        };
    }
}