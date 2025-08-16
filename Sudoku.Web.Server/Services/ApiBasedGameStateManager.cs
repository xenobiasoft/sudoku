using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.HttpClients;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// API-based game state manager that uses the Game API
/// </summary>
public class ApiBasedGameStateManager(
    IGameApiClient gameApiClient,
    ILocalStorageService localStorageService,
    ILogger<ApiBasedGameStateManager> logger)
    : IApiBasedGameStateManager
{
    private readonly IGameApiClient _gameApiClient = gameApiClient ?? throw new ArgumentNullException(nameof(gameApiClient));
    private readonly ILocalStorageService _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
    private readonly ILogger<ApiBasedGameStateManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<ApiResult<object>> DeleteGameAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Deleting game {GameId} for player {Alias}", gameId, alias);
            
            // Delete from local storage first
            await _localStorageService.DeleteGameAsync(gameId);
            
            // Then delete from server
            var result = await _gameApiClient.DeleteGameAsync(alias, gameId);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully deleted game {GameId} for player {Alias}", gameId, alias);
            }
            else
            {
                _logger.LogWarning("Failed to delete game {GameId} from server for player {Alias}: {Error}", gameId, alias, result.Error);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting game {GameId} for player {Alias}", gameId, alias);
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<GameModel>> LoadGameAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Loading game {GameId} for player {Alias}", gameId, alias);
            
            var result = await _gameApiClient.GetGameAsync(alias, gameId);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully loaded game {GameId} for player {Alias}", gameId, alias);
                // TODO: Consider caching in local storage if needed
            }
            else
            {
                _logger.LogWarning("Failed to load game {GameId} for player {Alias}: {Error}", gameId, alias, result.Error);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading game {GameId} for player {Alias}", gameId, alias);
            return ApiResult<GameModel>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<List<GameModel>>> LoadGamesAsync(string alias)
    {
        try
        {
            _logger.LogInformation("Loading all games for player {Alias}", alias);
            
            var result = await _gameApiClient.GetAllGamesAsync(alias);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully loaded {Count} games for player {Alias}", result.Value?.Count ?? 0, alias);
            }
            else
            {
                _logger.LogWarning("Failed to load games for player {Alias}: {Error}", alias, result.Error);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading games for player {Alias}", alias);
            return ApiResult<List<GameModel>>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> ResetGameAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Resetting game {GameId} for player {Alias}", gameId, alias);
            
            var result = await _gameApiClient.ResetGameAsync(alias, gameId);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully reset game {GameId} for player {Alias}", gameId, alias);
            }
            else
            {
                _logger.LogWarning("Failed to reset game {GameId} for player {Alias}: {Error}", gameId, alias, result.Error);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while resetting game {GameId} for player {Alias}", gameId, alias);
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<GameModel>> CreateGameAsync(string alias, string difficulty)
    {
        try
        {
            _logger.LogInformation("Creating game for player {Alias} with difficulty {Difficulty}", alias, difficulty);
            
            var result = await _gameApiClient.CreateGameAsync(alias, difficulty);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully created game {GameId} for player {Alias}", result.Value?.Id, alias);
            }
            else
            {
                _logger.LogWarning("Failed to create game for player {Alias}: {Error}", alias, result.Error);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating game for player {Alias}", alias);
            return ApiResult<GameModel>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> UndoGameAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Undoing move for game {GameId} for player {Alias}", gameId, alias);
            
            var result = await _gameApiClient.UndoMoveAsync(alias, gameId);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully undid move for game {GameId} for player {Alias}", gameId, alias);
            }
            else
            {
                _logger.LogWarning("Failed to undo move for game {GameId} for player {Alias}: {Error}", gameId, alias, result.Error);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while undoing move for game {GameId} for player {Alias}", gameId, alias);
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> MakeMoveAsync(string alias, string gameId, int row, int column, int? value)
    {
        try
        {
            _logger.LogInformation("Making move for game {GameId} for player {Alias} at ({Row}, {Column}) with value {Value}", 
                gameId, alias, row, column, value);
            
            var result = await _gameApiClient.MakeMoveAsync(alias, gameId, row, column, value);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully made move for game {GameId} for player {Alias}", gameId, alias);
            }
            else
            {
                _logger.LogWarning("Failed to make move for game {GameId} for player {Alias}: {Error}", gameId, alias, result.Error);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while making move for game {GameId} for player {Alias}", gameId, alias);
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> AddPossibleValueAsync(string alias, string gameId, int row, int column, int value)
    {
        try
        {
            var result = await _gameApiClient.AddPossibleValueAsync(alias, gameId, row, column, value);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding possible value for game {GameId} for player {Alias}", gameId, alias);
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> RemovePossibleValueAsync(string alias, string gameId, int row, int column, int value)
    {
        try
        {
            var result = await _gameApiClient.RemovePossibleValueAsync(alias, gameId, row, column, value);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing possible value for game {GameId} for player {Alias}", gameId, alias);
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> ClearPossibleValuesAsync(string alias, string gameId, int row, int column)
    {
        try
        {
            var result = await _gameApiClient.ClearPossibleValuesAsync(alias, gameId, row, column);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while clearing possible values for game {GameId} for player {Alias}", gameId, alias);
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<ValidationResultModel>> ValidateGameAsync(string alias, string gameId)
    {
        try
        {
            var result = await _gameApiClient.ValidateGameAsync(alias, gameId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while validating game {GameId} for player {Alias}", gameId, alias);
            return ApiResult<ValidationResultModel>.Failure($"Exception occurred: {ex.Message}");
        }
    }
}