namespace XenobiaSoft.Sudoku.GameState;

public class InMemoryGameStateStorage : IInMemoryGameStateStorage
{
	private readonly CircularStack<GameStateMemory> _gameState = new(50);

    public GameStateMemoryType MemoryType => GameStateMemoryType.InMemory;

    public Task DeleteAsync(string alias, string puzzleId)
    {
        _gameState.Clear();

        return Task.CompletedTask;
    }

    public Task<GameStateMemory> LoadAsync(string alias, string puzzleId)
    {
        return Task.FromResult(_gameState.Count > 0 ? _gameState.Peek() : null);
    }

    public async Task<GameStateMemory> ResetAsync(string alias, string puzzleId)
    {
        while (_gameState.Count > 1)
        {
            await UndoAsync(alias, puzzleId);
        }

        return _gameState.Peek();
    }

    public Task SaveAsync(GameStateMemory gameState)
    {
        if (_gameState.Count > 0)
        {
            var previousGameState = _gameState.Peek();

            if (AreGameStatesEqual(gameState, previousGameState)) return Task.CompletedTask;
        }

        _gameState.Push(gameState);

        return Task.CompletedTask;
    }

    public Task<GameStateMemory> UndoAsync(string alias, string puzzleId)
    {
        return _gameState.Count == 0 ? Task.FromResult<GameStateMemory>(null) : Task.FromResult(_gameState.Pop());
    }

    private bool AreGameStatesEqual(GameStateMemory gameState1, GameStateMemory gameState2)
    {
        if (gameState1.PuzzleId != gameState2.PuzzleId)
        {
            return false;
        }

        return AreBoardsEqual(gameState1.Board, gameState2.Board);
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