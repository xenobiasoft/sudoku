using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;
using System.Text.Json;
using ILocalStorageService = Sudoku.Web.Server.Services.Abstractions.V2.ILocalStorageService;

namespace Sudoku.Web.Server.Services.V2;

public class LocalStorageService(IJsRuntimeWrapper jsRuntime) : ILocalStorageService
{
    private const string SavedGamesKey = "savedGames";
    private const string AliasKey = "sudoku-alias";

    public async Task DeleteGameAsync(string gameId)
    {
        var games = await LoadGameStatesAsync();
        var gameToRemove = games.FirstOrDefault(g => g.Id == gameId);
        if (gameToRemove != null)
        {
            games.Remove(gameToRemove);
        }

        await SaveGameStateAsync(games);
    }

    public async Task<string?> GetAliasAsync()
    {
        var alias = await jsRuntime.GetAsync(AliasKey);

        return alias;
    }

    public async Task<GameModel?> LoadGameAsync(string gameId)
    {
        var games = await LoadGameStatesAsync();
        var foundGame = games.ToList().Find(x => x.Id == gameId);

        return foundGame;
    }

    public async Task<List<GameModel>> LoadGameStatesAsync()
    {
        var json = await jsRuntime.GetAsync(SavedGamesKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<GameModel>>(json) ?? [];
    }

    public async Task SaveGameStateAsync(GameModel gameState)
    {
        var games = await LoadGameStatesAsync();
        var existingGame = games.FirstOrDefault(g => g.Id == gameState.Id);

        if (existingGame != null)
        {
            games.Remove(existingGame);
        }

        games.Add(gameState);

        await SaveGameStateAsync(games);
    }

    public async Task SetAliasAsync(string alias)
    {
        await jsRuntime.SetAsync(AliasKey, alias);
    }

    private async Task SaveGameStateAsync(IEnumerable<GameModel> gameState)
    {
        var json = JsonSerializer.Serialize(gameState);

        await jsRuntime.SetAsync(SavedGamesKey, json);
    }
}