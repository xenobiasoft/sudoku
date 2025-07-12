using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.Extensions;
using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Services;

public class GameService(IPersistentGameStateStorage storage, IPuzzleGenerator puzzleGenerator) : IGameService
{
    public async Task<string> CreateGameAsync(string alias, GameDifficulty difficulty)
    {
        var puzzle = await puzzleGenerator.GenerateAsync(difficulty);

        await storage.SaveAsync(puzzle.ToGameState());

        return puzzle.PuzzleId;
    }

    public Task DeleteGameAsync(string alias, string gameId)
    {
        return storage.DeleteAsync(alias, gameId);
    }

    public Task<GameStateMemory> LoadGameAsync(string alias, string gameId)
    {
        return storage.LoadAsync(alias, gameId);
    }

    public Task<IEnumerable<GameStateMemory>> LoadGamesAsync(string alias)
    {
        return storage.LoadAllAsync(alias);
    }

    public Task<GameStateMemory> ResetGameAsync(string alias, string gameId)
    {
        return storage.ResetAsync(alias, gameId);
    }

    public Task SaveGameAsync(GameStateMemory gameState)
    {
        return storage.SaveAsync(gameState);
    }

    public Task<GameStateMemory> UndoGameAsync(string alias, string gameId)
    {
        return storage.UndoAsync(alias, gameId);
    }
}