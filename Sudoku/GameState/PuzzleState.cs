using System.Text.Json.Serialization;

namespace XenobiaSoft.Sudoku.GameState;

[method: JsonConstructor]
public class PuzzleState(string puzzleId, Cell[] board)
{
    public string PuzzleId { get; init; } = puzzleId;
    public Cell[] Board { get; init; } = board;
}