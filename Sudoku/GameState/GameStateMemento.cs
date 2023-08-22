namespace XenobiaSoft.Sudoku.GameState;

public class GameStateMemento
{
	public GameStateMemento(string[,] possibleValues, int[,] values, int score)
	{
		PossibleValues = possibleValues;
		Values = values;
		Score = score;
	}

	public string[,] PossibleValues { get; private set; }
	public int[,] Values { get; private set; }
	public int Score { get; private set; }
}