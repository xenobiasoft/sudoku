namespace XenobiaSoft.Sudoku.GameState;

public class GameStateMemento
{
    public GameStateMemento(string puzzleId, Cell[] board, int score)
    {
        PuzzleId = puzzleId;
        Board = board;
        Score = score;
        LastUpdated = DateTime.UtcNow;
    }

    public GameStateMemento()
    {}

    public string PuzzleId { get; init; }
    public Cell[] Board { get; private set; }
    public int Score { get; private set; }
    public DateTime LastUpdated { get; init; }
}