namespace XenobiaSoft.Sudoku.GameState.Decorators;

public class CachingAzureBlobGameStateStorageDecorator(IGameStateStorage<GameStateMemory> decorated) : AzureBlobGameStateStorageDecorator(decorated)
{
    private readonly Dictionary<string, GameStateMemory> _cache = new();

    public override async Task DeleteAsync(string puzzleId)
    {
        await decorated.DeleteAsync(puzzleId);

        _cache.Remove(puzzleId);
    }

    public override async Task<GameStateMemory> LoadAsync(string puzzleId)
    {
        if (_cache.TryGetValue(puzzleId, out var gameState))
        {
            return gameState;
        }

        gameState = await decorated.LoadAsync(puzzleId);

        if (gameState != null)
        {
            _cache[puzzleId] = gameState;
        }

        return gameState;
    }

    public override Task SaveAsync(GameStateMemory gameState)
    {
        _cache[gameState.PuzzleId] = gameState;
        return decorated.SaveAsync(gameState);
    }

    public override async Task<GameStateMemory> UndoAsync(string puzzleId)
    {
        var gameState = await decorated.UndoAsync(puzzleId);

        _cache[puzzleId] = gameState;

        return gameState;
    }
}