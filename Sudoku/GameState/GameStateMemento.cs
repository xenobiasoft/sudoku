namespace XenobiaSoft.Sudoku.GameState;

public class GameStateMemento
{
	public GameStateMemento(IEnumerable<Cell> cells, int score)
	{
		Cells = cells;
		Score = score;
	}

	public IEnumerable<Cell> Cells { get; private set; }
	public int Score { get; private set; }
}