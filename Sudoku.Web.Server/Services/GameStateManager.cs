using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class GameStateManager(ILocalStorageService localStorageService, IGameStateStorage gameStateStorage) : IGameStateManager
{
    public async Task DeleteGameAsync(string alias, string gameId)
    {
        await localStorageService.DeleteGameAsync(gameId);
        await gameStateStorage.DeleteAsync(alias, gameId);
    }

    public async Task<GameStateMemory?> LoadGameAsync(string alias, string gameId)
    {
        var gameState = await localStorageService.LoadGameAsync(gameId);

        if (gameState == null)
        {
            await gameStateStorage.LoadAsync(alias, gameId);
        }

        return gameState;
    }

    public Task<List<GameStateMemory>> LoadGamesAsync()
    {
        return localStorageService.LoadGameStatesAsync();
    }

    public async Task<GameStateMemory> ResetGameAsync(string alias, string gameId)
    {
        var gameState = await gameStateStorage.ResetAsync(alias, gameId);
        await localStorageService.SaveGameStateAsync(gameState!);

        return gameState!;
    }

    public async Task SaveGameAsync(GameStateMemory gameState)
    {
        await gameStateStorage.SaveAsync(gameState);
        await localStorageService.SaveGameStateAsync(gameState);
    }

    public async Task<GameStateMemory> UndoGameAsync(string alias, string gameId)
    {
        var currentGameState = await LoadGameAsync(alias, gameId);

        if (currentGameState!.TotalMoves <= 1) return currentGameState;

        var previousGameState = await gameStateStorage.UndoAsync(alias, gameId);
        await localStorageService.SaveGameStateAsync(previousGameState!);

        return previousGameState!;
    }
}