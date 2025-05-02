using Microsoft.JSInterop;
using System.Text.Json;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class LocalStorageService(IJSRuntime jsRuntime) : ILocalStorageService
{
    private const string SavedGamesKey = "savedGames";

    public async Task AddGameStateAsync(GameStateMemory gameState)
    {
        var games = await LoadGameStatesAsync();
        games.Add(gameState);

        await SaveGameStateAsync(games);
    }

    public async Task ClearGameStateAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", SavedGamesKey);
    }

    public async Task<List<GameStateMemory>> LoadGameStatesAsync()
    {
        var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", SavedGamesKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<GameStateMemory>>(json) ?? [];
    }

    public async Task RemoveGameAsync(string gameId)
    {
        var games = await LoadGameStatesAsync();
        var gameToRemove = games.FirstOrDefault(g => g.PuzzleId == gameId);
        if (gameToRemove != null)
        {
            games.Remove(gameToRemove);
        }

        await SaveGameStateAsync(games);
    }

    private async Task SaveGameStateAsync(IEnumerable<GameStateMemory> gameState)
    {
        var json = JsonSerializer.Serialize(gameState);

        await jsRuntime.InvokeVoidAsync("localStorage.setItem", SavedGamesKey, json);
    }
}
