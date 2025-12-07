using Sudoku.Web.Server.Models;
using IGameStateManager = Sudoku.Web.Server.Services.Abstractions.V2.IGameStateManager;

namespace Sudoku.Web.Server.Services.V2;

/// <summary>
/// Manages the lifecycle and state of a game, including creation, loading, saving, deletion, and other game-related
/// operations.
/// </summary>
/// <remarks>This class provides methods to interact with both local storage and a remote game API to manage game
/// data.  It ensures that game state is synchronized between the client and server, and offers functionality for
/// creating,  loading, saving, deleting, resetting, and undoing game actions. The class maintains the current game
/// state  through the <see cref="Game"/> property.</remarks>
/// <param name="localStorageService"></param>
/// <param name="gameApiClient"></param>
/// <param name="gameTimer"></param>
public partial class GameManager : IGameStateManager
{
    public async Task<GameModel> CreateGameAsync(string alias, string difficulty)
    {
        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentException("Alias not set.");
        }
        if (string.IsNullOrEmpty(difficulty))
        {
            throw new ArgumentException("Difficulty not set.");
        }
        var response = await gameApiClient.CreateGameAsync(alias, difficulty);
        if (!response.IsSuccess || response.Value == null)
        {
            throw new Exception("Failed to create game.");
        }
        Game = response.Value;
        await localStorageService.SaveGameStateAsync(Game);
        return Game;
    }

    public async Task DeleteGameAsync()
    {
        if (Game == null)
        {
            throw new Exception("No game loaded.");
        }

        await DeleteGameAsync(Game.PlayerAlias, Game.Id);
        Game = null;
    }

    public async Task DeleteGameAsync(string alias, string gameId)
    {
        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentException("Alias not set.");
        }
        if (string.IsNullOrEmpty(gameId))
        {
            throw new ArgumentException("Game ID not set.");
        }
        var result = await gameApiClient.DeleteGameAsync(alias, gameId);
        if (!result.IsSuccess)
        {
            throw new Exception("Failed to delete game from server.");
        }
        await localStorageService.DeleteGameAsync(gameId);
    }

    public Task<GameModel> LoadGameAsync(string alias, string gameId)
    {
        return LoadGameAsync(alias, gameId, forceRefresh: false);
    }

    private async Task<GameModel> LoadGameAsync(string alias, string gameId, bool forceRefresh)
    {
        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentException("Alias not set.");
        }
        if (string.IsNullOrEmpty(gameId))
        {
            throw new ArgumentException("Game ID not set.");
        }

        if (!forceRefresh)
        {
            var game = await localStorageService.LoadGameAsync(gameId);
            if (game != null)
            {
                Game = game;
                return Game;
            }
        }

        var response = await gameApiClient.GetGameAsync(alias, gameId);
        if (!response.IsSuccess || response.Value == null)
        {
            throw new Exception("Failed to load game.");
        }

        Game = response.Value;
        await localStorageService.SaveGameStateAsync(Game);

        return Game;
    }

    public async Task<List<GameModel>> LoadGamesAsync(string alias)
    {
        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentException("Alias not set.");
        }

        var games = await localStorageService.LoadGameStatesAsync();
        if (games.Any())
        {
            return games;
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

    public async Task<GameModel> ResetGameAsync()
    {
        var resetResponse = await gameApiClient.ResetGameAsync(Game.PlayerAlias, Game.Id);
        if (!resetResponse.IsSuccess)
        {
            throw new Exception("Failed to reset game.");
        }

        Game = await LoadGameAsync(Game.PlayerAlias, Game.Id, forceRefresh: true);

        return Game;
    }

    public async Task SaveGameAsync()
    {
        await gameApiClient.SaveGameAsync(Game);
        await localStorageService.SaveGameStateAsync(Game);
    }

    public async Task SaveGameAsync(int row, int column, int? value)
    {
        var response = await gameApiClient.MakeMoveAsync(Game.PlayerAlias, Game.Id, row, column, value, CurrentStatistics.PlayDuration);
        if (!response.IsSuccess)
        {
            throw new Exception("Failed to save move.");
        }
        Game = await LoadGameAsync(Game.PlayerAlias, Game.Id, forceRefresh: true);
    }

    public async Task SaveGameStatusAsync()
    {
        await gameApiClient.SaveGameStatusAsync(Game.PlayerAlias, Game.Id, Game.Status);
    }

    public async Task<GameModel> UndoGameAsync()
    {
        if (Game!.Statistics.TotalMoves < 1)
        {
            return Game;
        }

        var response = await gameApiClient.UndoMoveAsync(Game.PlayerAlias, Game.Id);
        if (!response.IsSuccess)
        {
            throw new Exception("Failed to undo move.");
        }

        Game = await LoadGameAsync(Game.PlayerAlias, Game.Id, forceRefresh: true);

        return Game;
    }

    public async Task AddPossibleValueAsync(int row, int column, int value)
    {
        var response = await gameApiClient.AddPossibleValueAsync(Game.PlayerAlias, Game.Id, row, column, value);
        if (!response.IsSuccess)
        {
            throw new Exception("Failed to add possible value.");
        }

        var cell = Game.Cells.FirstOrDefault(c => c.Row == row && c.Column == column);
        if (cell != null && !cell.PossibleValues.Contains(value))
        {
            cell.PossibleValues.Add(value);
        }

        await localStorageService.SaveGameStateAsync(Game);
    }

    public async Task RemovePossibleValueAsync(int row, int column, int value)
    {
        var response = await gameApiClient.RemovePossibleValueAsync(Game.PlayerAlias, Game.Id, row, column, value);
        if (!response.IsSuccess)
        {
            throw new Exception("Failed to remove possible value.");
        }

        var cell = Game.Cells.FirstOrDefault(c => c.Row == row && c.Column == column);
        if (cell != null && cell.PossibleValues.Contains(value))
        {
            cell.PossibleValues.Remove(value);
        }

        await localStorageService.SaveGameStateAsync(Game);
    }

    public async Task ClearPossibleValuesAsync(int row, int column)
    {
        var response = await gameApiClient.ClearPossibleValuesAsync(Game.PlayerAlias, Game.Id, row, column);
        if (!response.IsSuccess)
        {
            throw new Exception("Failed to clear possible values.");
        }

        var cell = Game.Cells.FirstOrDefault(c => c.Row == row && c.Column == column);
        if (cell != null)
        {
            cell.PossibleValues.Clear();
        }

        await localStorageService.SaveGameStateAsync(Game);
    }
}