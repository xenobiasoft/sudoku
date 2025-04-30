using System.Text.Json.Serialization;

namespace XenobiaSoft.Sudoku.GameState;

public class GameStateMemory
{
    [JsonConstructor]
    public GameStateMemory(string puzzleId, Cell[] board, int score)
    {
        PuzzleId = puzzleId;
        Board = board;
        Score = score;
        LastUpdated = DateTime.UtcNow;
    }

    public GameStateMemory()
    {}

    public string PuzzleId { get; init; }
    public Cell[] Board { get; set; }
    public int Score { get; private set; }
    public DateTime LastUpdated { get; init; }
}