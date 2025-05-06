using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class GameStateManager(ILocalStorageService localStorageService, Func<string, IGameStateStorage> gameStateStorageFactory) : IGameStateManager
{
    private readonly IGameStateStorage _gameStateStorage = gameStateStorageFactory(GameStateTypes.AzurePersistent);

    public async Task DeleteGameAsync(string gameId)
    {
        await localStorageService.DeleteGameAsync(gameId);
        await _gameStateStorage.DeleteAsync(gameId);
    }

    public async Task<GameStateMemory?> LoadGameAsync(string gameId)
    {
        var gameState = await localStorageService.LoadGameAsync(gameId);

        if (gameState == null)
        {
            await _gameStateStorage.LoadAsync(gameId);
        }

        return gameState;
    }

    public Task<List<GameStateMemory>> LoadGamesAsync()
    {
        return localStorageService.LoadGameStatesAsync();
    }

    public async Task SaveGameAsync(GameStateMemory gameState)
    {
        await _gameStateStorage.SaveAsync(gameState);
        await localStorageService.SaveGameStateAsync(gameState);
    }

    public async Task<GameStateMemory> UndoAsync(string gameId)
    {
        var gameState = await _gameStateStorage.UndoAsync(gameId);

        if (gameState != null)
        {
            await localStorageService.SaveGameStateAsync(gameState);
        }

        return gameState;
    }
}