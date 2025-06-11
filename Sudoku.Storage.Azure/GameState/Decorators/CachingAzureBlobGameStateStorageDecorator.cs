using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Storage.Azure.GameState.Decorators;

public class CachingAzureBlobGameStateStorageDecorator(IPersistentGameStateStorage decorated) : AzureBlobGameStateStorageDecorator(decorated)
{
    private readonly Dictionary<string, GameStateMemory> _cache = new();

    public override async Task DeleteAsync(string alias, string puzzleId)
    {
        await decorated.DeleteAsync(alias, puzzleId);

        _cache.Remove(puzzleId);
    }

    public override async Task<GameStateMemory> LoadAsync(string alias, string puzzleId)
    {
        if (_cache.TryGetValue(puzzleId, out var gameState))
        {
            return gameState;
        }

        gameState = await decorated.LoadAsync(alias, puzzleId);

        if (gameState != null)
        {
            _cache[puzzleId] = gameState;
        }

        return gameState;
    }

    public override async Task<GameStateMemory> ResetAsync(string alias, string puzzleId)
    {
        var gameState = await decorated.ResetAsync(alias, puzzleId);

        _cache[puzzleId] = gameState;

        return gameState;
    }

    public override Task SaveAsync(GameStateMemory gameState)
    {
        _cache[gameState.PuzzleId] = gameState;

        return decorated.SaveAsync(gameState);
    }

    public override async Task<GameStateMemory> UndoAsync(string alias, string puzzleId)
    {
        var gameState = await decorated.UndoAsync(alias, puzzleId);

        _cache[puzzleId] = gameState;

        return gameState;
    }
}