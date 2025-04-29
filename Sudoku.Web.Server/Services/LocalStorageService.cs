using System.Text.Json;
using Microsoft.JSInterop;
using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services;

public class LocalStorageService(IJSRuntime jsRuntime)
{
    private const string SavedGamesKey = "savedGames";

    public async Task<List<SavedGame>> GetSavedGamesAsync()
    {
        var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", SavedGamesKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<SavedGame>>(json) ?? [];
    }

    public async Task SaveGameAsync(SavedGame game)
    {
        var games = await GetSavedGamesAsync();
        games.Add(game);

        var json = JsonSerializer.Serialize(games);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", SavedGamesKey, json);
    }

    public async Task ClearSavedGamesAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", SavedGamesKey);
    }
}
