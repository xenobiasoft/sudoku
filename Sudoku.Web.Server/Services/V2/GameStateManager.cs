using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.HttpClients;
using IGameStateManager = Sudoku.Web.Server.Services.Abstractions.V2.IGameStateManager;
using ILocalStorageService = Sudoku.Web.Server.Services.Abstractions.V2.ILocalStorageService;

namespace Sudoku.Web.Server.Services.V2;

public partial class GameManager(ILocalStorageService localStorageService, IGameApiClient gameApiClient, IGameTimer gameTimer) : IGameStateManager
{
    public GameModel? Game { get; private set; }

    public async Task DeleteGameAsync(string alias, string gameId)
    {
        var result = await gameApiClient.DeleteGameAsync(alias, gameId);
        if (!result.IsSuccess)
        {
            throw new Exception("Failed to delete game from server.");
        }
        await localStorageService.DeleteGameAsync(gameId);
    }

    public async Task<GameModel> LoadGameAsync(string alias, string gameId)
    {
        var game = await localStorageService.LoadGameAsync(gameId);
        if (game != null)
        {
            Game = game;
            return Game;
        }

        var response = await gameApiClient.GetGameAsync(alias, gameId);
        if (!response.IsSuccess || response.Value == null)
        {
            throw new Exception("Failed to load game.");
        }

        Game = response.Value;
        return Game;
    }

    public async Task<List<GameModel>> LoadGamesAsync()
    {
        var games = await localStorageService.LoadGameStatesAsync();
        if (games.Any())
        {
            return games;
        }
        var alias = await localStorageService.GetAliasAsync();
        if (string.IsNullOrEmpty(alias))
        {
            throw new Exception("Alias not set.");
        }
        var response = await gameApiClient.GetAllGamesAsync(alias);
        if (!response.IsSuccess || response.Value == null)
        {
            throw new Exception("Failed to load games.");
        }
        foreach (var game in response.Value)
        {
            await localStorageService.SaveGameStateAsync(game);
        }
        return response.Value;
    }

    public async Task<GameModel> ResetGameAsync(string alias, string gameId)
    {
        var resetResponse = await gameApiClient.ResetGameAsync(alias, gameId);
        if (!resetResponse.IsSuccess || resetResponse.Value == null)
        {
            throw new Exception("Failed to reset game.");
        }

        Game = await LoadGameAsync(alias, gameId);

        await localStorageService.SaveGameStateAsync(Game);

        return Game;
    }

    public async Task SaveGameAsync()
    {
        await gameApiClient.SaveGameAsync(Game);
        await localStorageService.SaveGameStateAsync(Game);
    }

    public async Task<GameModel> UndoGameAsync(string alias, string gameId)
    {
        if (Game!.Statistics.TotalMoves < 1)
        {
            return Game;
        }

        var response = await gameApiClient.UndoMoveAsync(alias, gameId);
        if (!response.IsSuccess || response.Value == null)
        {
            throw new Exception("Failed to undo move.");
        }
        Game = await LoadGameAsync(alias, gameId);
        await localStorageService.SaveGameStateAsync(Game);

        return Game;
    }
}