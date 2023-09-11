using System.Diagnostics;

namespace XenobiaSoft.Sudoku.GameState;

public class GameStateMemory : IGameStateMemory
{
	private readonly Stack<GameStateMemento> _gameState = new();

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
		_gameState.Push(gameState);
	}

	public GameStateMemento Undo()
	{
		return _gameState.Pop();
	}
}