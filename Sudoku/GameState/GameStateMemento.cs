namespace XenobiaSoft.Sudoku.GameState;

public class GameStateMemento(IEnumerable<Cell> cells, int score)
{
    public IEnumerable<Cell> Cells { get; private set; } = cells;
    public int Score { get; private set; } = score;
}