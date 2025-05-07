using System.Text.Json.Serialization;

namespace XenobiaSoft.Sudoku.GameState;

[method: JsonConstructor]
public class GameStateMemory(string puzzleId, Cell[] board) : PuzzleState(puzzleId, board)
{
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
}