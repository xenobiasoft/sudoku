using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class GameStorageManager(IGameStateManager gameStateManager, ILocalStorageService localStorageService) : IGameStorageManager
{
    public async Task DeleteGameAsync(string gameId)
    {
        await localStorageService.DeleteGameAsync(gameId);
        await gameStateManager.DeleteAsync(gameId);
    }

    public async Task<GameStateMemory?> LoadGameAsync(string gameId)
    {
        var gameState = await localStorageService.LoadGameAsync(gameId);

        if (gameState == null)
        {
            await gameStateManager.LoadAsync(gameId);
        }

        return gameState;
    }

    public async Task SaveGameAsync(GameStateMemory gameState)
    {
        await gameStateManager.SaveAsync(gameState);
        await localStorageService.SaveGameStateAsync(gameState);
    }

    public async Task<GameStateMemory> UndoAsync(string gameId)
    {
        var gameState = await gameStateManager.UndoAsync(gameId);

        if (gameState != null)
        {
            await localStorageService.SaveGameStateAsync(gameState);
        }

        return gameState;
    }
}