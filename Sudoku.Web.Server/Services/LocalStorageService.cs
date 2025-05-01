using Microsoft.JSInterop;
using System.Text.Json;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class LocalStorageService(IJSRuntime jsRuntime) : ILocalStorageService
{
    private const string SavedGamesKey = "savedGames";

    public async Task DeleteGameStateAsync()
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

    public async Task SaveGameStateAsync(GameStateMemory gameState)
    {
        var games = await LoadGameStatesAsync();
        games.Add(gameState);

        var json = JsonSerializer.Serialize(games);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", SavedGamesKey, json);
    }
}
