using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class GameStateManager(ILocalStorageService localStorageService, IGameStateStorage<GameStateMemory> gameStateStorage) : IGameStateManager
{
    public async Task DeleteGameAsync(string gameId)
    {
        await localStorageService.DeleteGameAsync(gameId);
        await gameStateStorage.DeleteAsync(gameId);
    }

    public async Task<GameStateMemory?> LoadGameAsync(string gameId)
    {
        var gameState = await localStorageService.LoadGameAsync(gameId);

        if (gameState == null)
        {
            await gameStateStorage.LoadAsync(gameId);
        }

        return gameState;
    }

    public Task<List<GameStateMemory>> LoadGamesAsync()
    {
        return localStorageService.LoadGameStatesAsync();
    }

    public Task<GameStateMemory> ResetAsync(string gameId)
    {
        throw new NotImplementedException();
    }

    public async Task SaveGameAsync(GameStateMemory gameState)
    {
        await gameStateStorage.SaveAsync(gameState);
        await localStorageService.SaveGameStateAsync(gameState);
    }

    public async Task<GameStateMemory> UndoAsync(string gameId)
    {
        var currentGameState = await LoadGameAsync(gameId);

        if (currentGameState!.TotalMoves <= 1) return currentGameState;

        var gameState = await gameStateStorage.UndoAsync(gameId);

        if (gameState != null)
        {
            await localStorageService.SaveGameStateAsync(gameState);
        }

        return gameState!;

    }
}