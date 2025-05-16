using System.Diagnostics;
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

    public async Task<GameStateMemory> ResetGameAsync(string gameId)
    {
        var gameState = await gameStateStorage.ResetAsync(gameId);
        await localStorageService.SaveGameStateAsync(gameState!);

        return gameState!;
    }

    public async Task SaveGameAsync(GameStateMemory gameState)
    {
        await gameStateStorage.SaveAsync(gameState);
        await localStorageService.SaveGameStateAsync(gameState);
    }

    public async Task<GameStateMemory> UndoGameAsync(string gameId)
    {
        var currentGameState = await LoadGameAsync(gameId);

        if (currentGameState!.TotalMoves <= 1) return currentGameState;

        var gameState = await gameStateStorage.UndoAsync(gameId);

        return gameState!;
    }
}