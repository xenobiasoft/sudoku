using System.Text.Json.Serialization;

namespace XenobiaSoft.Sudoku.GameState;

public class GameStateMemory
{
    [JsonConstructor]
    public GameStateMemory(string puzzleId, Cell[] board)
    {
        PuzzleId = puzzleId;
        Board = board;
        LastUpdated = DateTime.UtcNow;
    }

    public GameStateMemory()
    {}

    public string PuzzleId { get; init; }
    public Cell[] Board { get; set; }
    public DateTime LastUpdated { get; init; }
}