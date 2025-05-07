using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Generator;

namespace XenobiaSoft.Sudoku;

public class SudokuGame(IPuzzleGenerator puzzleGenerator, Func<string, IGameStateStorage> gameStateMemoryFactory)
    : ISudokuGame
{
    private readonly IGameStateStorage _gameState = gameStateMemoryFactory(GameStateTypes.AzurePersistent);

    public async Task<GameStateMemory> NewGameAsync(Level level)
	{
		var puzzle = await puzzleGenerator.Generate(level).ConfigureAwait(false);

        var gameState = puzzle.ToGameState();

        return gameState;
    }

    public Task SaveAsync(GameStateMemory memory)
    {
        return _gameState.SaveAsync(memory);
    }

    public Task DeleteAsync(string puzzleId)
    {
        return _gameState.DeleteAsync(puzzleId);
    }

    public Task<GameStateMemory> LoadAsync(string puzzleId)
	{
        return _gameState.LoadAsync(puzzleId);
    }
}