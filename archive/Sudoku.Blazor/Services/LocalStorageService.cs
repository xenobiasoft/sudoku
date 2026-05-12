using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;
using System.Text.Json;

namespace Sudoku.Blazor.Services;

public class LocalStorageService(IJsRuntimeWrapper jsRuntime) : ILocalStorageService
{
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    private const string SavedGamesKey = "savedGames";
    private const string ProfileKey = "sudoku-profile";

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

    public async Task<ProfileInfo?> GetProfileAsync()
    {
        var json = await jsRuntime.GetAsync(ProfileKey);
        if (string.IsNullOrWhiteSpace(json)) return null;

        try
        {
            var profile = JsonSerializer.Deserialize<ProfileInfo>(json, _serializerOptions);
            if (profile == null || string.IsNullOrWhiteSpace(profile.ProfileId) || string.IsNullOrWhiteSpace(profile.Alias))
            {
                return null;
            }
            return profile;
        }
        catch (JsonException)
        {
            return null;
        }
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

        try
        {
            var games = JsonSerializer.Deserialize<List<GameModel>>(json, _serializerOptions) ?? [];
            return games.Where(x => !string.IsNullOrWhiteSpace(x.Id)).ToList();
        }
        catch (JsonException)
        {
            return [];
        }
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

    private async Task SaveGameStateAsync(IEnumerable<GameModel> gameState)
    {
        var json = JsonSerializer.Serialize(gameState);

        await jsRuntime.SetAsync(SavedGamesKey, json);
    }

    public async Task SetProfileAsync(ProfileInfo profile)
    {
        var json = JsonSerializer.Serialize(profile);
        await jsRuntime.SetAsync(ProfileKey, json);
    }
}
