using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Generator;

namespace XenobiaSoft.Sudoku;

public class SudokuGame(IPuzzleGenerator puzzleGenerator, IGameStateStorage<GameStateMemory> gameState)
    : ISudokuGame
{
    public async Task<GameStateMemory> NewGameAsync(Level level)
	{
		var puzzle = await puzzleGenerator.Generate(level).ConfigureAwait(false);

        var gameState = puzzle.ToGameState();

        return gameState;
    }

    public Task SaveAsync(GameStateMemory memory)
    {
        return gameState.SaveAsync(memory);
    }

    public Task DeleteAsync(string puzzleId)
    {
        return gameState.DeleteAsync(puzzleId);
    }

    public Task<GameStateMemory> LoadAsync(string puzzleId)
	{
        return gameState.LoadAsync(puzzleId);
    }
}