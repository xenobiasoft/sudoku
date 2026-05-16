using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;

namespace Sudoku.Blazor.Services;

/// <summary>
/// Manages the lifecycle and state of a game, including creation, loading, saving, deletion, and other game-related
/// operations.
/// </summary>
public partial class GameManager : IGameStateManager
{
    public async Task<GameModel> CreateGameAsync(string profileId, string difficulty)
    {
        if (string.IsNullOrEmpty(profileId))
        {
            throw new ArgumentException("Profile ID not set.");
        }
        if (string.IsNullOrEmpty(difficulty))
        {
            throw new ArgumentException("Difficulty not set.");
        }
        var response = await gameApiClient.CreateGameAsync(profileId, difficulty);
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

        await DeleteGameAsync(Game.ProfileId, Game.Id);
        Game = null;
    }

    public async Task DeleteGameAsync(string profileId, string gameId)
    {
        if (string.IsNullOrEmpty(profileId))
        {
            throw new ArgumentException("Profile ID not set.");
        }
        if (string.IsNullOrEmpty(gameId))
        {
            throw new ArgumentException("Game ID not set.");
        }
        var result = await gameApiClient.DeleteGameAsync(profileId, gameId);
        if (!result.IsSuccess && result.StatusCode != 404)
        {
            throw new Exception("Failed to delete game from server.");
        }
        await localStorageService.DeleteGameAsync(gameId);
    }

    public async Task<GameModel> LoadGameAsync(string profileId, string gameId)
    {
        if (string.IsNullOrEmpty(profileId))
        {
            throw new ArgumentException("Profile ID not set.");
        }
        if (string.IsNullOrEmpty(gameId))
        {
            throw new ArgumentException("Game ID not set.");
        }

        var response = await gameApiClient.GetGameAsync(profileId, gameId);
        if (!response.IsSuccess || response.Value == null)
        {
            throw new Exception("Failed to load game.");
        }

        Game = response.Value;
        await localStorageService.SaveGameStateAsync(Game);

        return Game;
    }

    public async Task<List<GameModel>> LoadGamesAsync(string profileId)
    {
        if (string.IsNullOrEmpty(profileId))
        {
            throw new ArgumentException("Profile ID not set.");
        }

        var games = await localStorageService.LoadGameStatesAsync();
        if (games.Any())
        {
            return games;
        }
        var response = await gameApiClient.GetAllGamesAsync(profileId);
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
        var resetResponse = await gameApiClient.ResetGameAsync(Game.ProfileId, Game.Id);
        if (!resetResponse.IsSuccess)
        {
            throw new Exception("Failed to reset game.");
        }

        Game = await LoadGameAsync(Game.ProfileId, Game.Id);

        return Game;
    }

    public async Task SaveGameAsync()
    {
        await gameApiClient.SaveGameAsync(Game);
        await localStorageService.SaveGameStateAsync(Game);
    }

    public async Task SaveGameAsync(int row, int column, int? value)
    {
        var response = await gameApiClient.MakeMoveAsync(Game.ProfileId, Game.Id, row, column, value, CurrentStatistics.PlayDuration);
        if (!response.IsSuccess)
        {
            throw new Exception("Failed to save move.");
        }
        Game = await LoadGameAsync(Game.ProfileId, Game.Id);
    }

    public Task SaveGameStatusAsync()
    {
        if (Game.Status == GameStatus.InProgress) return gameApiClient.ResumeGameAsync(Game.ProfileId, Game.Id);
        if (Game.Status == GameStatus.Paused)     return gameApiClient.PauseGameAsync(Game.ProfileId, Game.Id);
        if (Game.Status == GameStatus.Completed)  return gameApiClient.CompleteGameAsync(Game.ProfileId, Game.Id);
        if (Game.Status == GameStatus.Abandoned)  return gameApiClient.AbandonGameAsync(Game.ProfileId, Game.Id);
        return Task.CompletedTask;
    }

    public async Task<GameModel> UndoGameAsync()
    {
        if (Game!.Statistics.TotalMoves < 1)
        {
            return Game;
        }

        var response = await gameApiClient.UndoMoveAsync(Game.ProfileId, Game.Id);
        if (!response.IsSuccess)
        {
            throw new Exception("Failed to undo move.");
        }

        Game = await LoadGameAsync(Game.ProfileId, Game.Id);

        return Game;
    }

    public async Task AddPossibleValueAsync(int row, int column, int value)
    {
        var response = await gameApiClient.AddPossibleValueAsync(Game.ProfileId, Game.Id, row, column, value);
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
        var response = await gameApiClient.RemovePossibleValueAsync(Game.ProfileId, Game.Id, row, column, value);
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
        var response = await gameApiClient.ClearPossibleValuesAsync(Game.ProfileId, Game.Id, row, column);
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
