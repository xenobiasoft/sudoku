using System.Text.Json;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class LocalStorageService(IJsRuntimeWrapper jsRuntime) : ILocalStorageService
{
    private const string SavedGamesKey = "savedGames";

    public async Task<GameStateMemory?> LoadGameAsync(string gameId)
    {
        var games = await LoadGameStatesAsync();
        var foundGame = games.Find(x => x.PuzzleId == gameId);

        return foundGame;
    }

    public async Task<List<GameStateMemory>> LoadGameStatesAsync()
    {
        var json = await jsRuntime.GetAsync(SavedGamesKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<GameStateMemory>>(json) ?? [];
    }

    public async Task DeleteGameAsync(string gameId)
    {
        var games = await LoadGameStatesAsync();
        var gameToRemove = games.FirstOrDefault(g => g.PuzzleId == gameId);
        if (gameToRemove != null)
        {
            games.Remove(gameToRemove);
        }

        await SaveGameStateAsync(games);
    }

    public async Task SaveGameStateAsync(GameStateMemory gameState)
    {
        var games = await LoadGameStatesAsync();
        var existingGame = games.FirstOrDefault(g => g.PuzzleId == gameState.PuzzleId);

        if (existingGame != null)
        {
            games.Remove(existingGame);
        }
        
        games.Add(gameState);

        await SaveGameStateAsync(games);
    }

    private async Task SaveGameStateAsync(IEnumerable<GameStateMemory> gameState)
    {
        var json = JsonSerializer.Serialize(gameState);

        await jsRuntime.SetAsync(SavedGamesKey, json);
    }
}
