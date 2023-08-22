namespace XenobiaSoft.Sudoku.GameState;

public class GameStateMemory : IGameStateMemory
{
	private readonly Stack<GameStateMemento> _gameState = new();

	public void Save(GameStateMemento gameState)
	{
		_gameState.Push(gameState);
	}

	public Stack<GameStateMemento> GameState => _gameState;

	public GameStateMemento Undo()
	{
		return _gameState.Pop();
	}
}