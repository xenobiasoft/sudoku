using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Generator;

namespace XenobiaSoft.Sudoku;

public class SudokuGame(IPuzzleGenerator puzzleGenerator, Func<string, IGameStateMemory> gameStateMemoryFactory)
    : ISudokuGame
{
    private readonly IGameStateMemory _gameState = gameStateMemoryFactory(GameStateTypes.AzurePersistent);

    public async Task<string> NewGameAsync(Level level)
	{
		var puzzle = await puzzleGenerator.Generate(level).ConfigureAwait(false);

        var gameState = puzzle.ToGameState(0);
        await _gameState.SaveAsync(gameState);

        return puzzle.PuzzleId;
    }

    public Task SaveAsync(GameStateMemento memento)
    {
        return _gameState.SaveAsync(memento);
    }

    public Task DeleteAsync(string puzzleId)
    {
        return _gameState.DeleteAsync(puzzleId);
    }

    public Task<GameStateMemento> LoadAsync(string puzzleId)
	{
        return _gameState.LoadAsync(puzzleId);
    }
}