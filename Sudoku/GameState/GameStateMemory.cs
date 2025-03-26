namespace XenobiaSoft.Sudoku.GameState;

public class GameStateMemory : IGameStateMemory
{
	private readonly CircularStack<GameStateMemento> _gameState = new(10);

	public void Clear()
	{
		_gameState.Clear();
	}

	public bool IsEmpty()
	{
		return _gameState.Count == 0;
	}

	public void Save(GameStateMemento gameState)
	{
		if (_gameState.Count > 0)
		{
			var previousGameState = _gameState.Peek();

			if (gameState.Puzzle.GetAllCells().SequenceEqual(previousGameState.Puzzle.GetAllCells())) return;
		}

		_gameState.Push(gameState);
	}

	public GameStateMemento Undo()
	{
		return _gameState.Pop();
	}
}