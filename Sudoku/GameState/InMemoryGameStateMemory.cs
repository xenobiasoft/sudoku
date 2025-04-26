namespace XenobiaSoft.Sudoku.GameState;

public class InMemoryGameStateMemory : IGameStateMemory
{
	private readonly CircularStack<GameStateMemento> _gameState = new(15);

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

            if (AreBoardsEqual(gameState.Board, previousGameState.Board)) return;
        }

		_gameState.Push(gameState);
	}

	public GameStateMemento Undo()
	{
		return _gameState.Pop();
	}

	private bool AreBoardsEqual(Cell[] board1, Cell[] board2)
    {
        if (board1.Length != board2.Length) return false;

        for (var i = 0; i < board1.Length; i++)
        {
            if (board1[i].Row != board2[i].Row ||
                board1[i].Column != board2[i].Column ||
                board1[i].Value != board2[i].Value ||
                board1[i].PossibleValues != board2[i].PossibleValues ||
                board1[i].Locked != board2[i].Locked)
            {
                return false;
            }
        }

        return true;
    }
}