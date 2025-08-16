using System.Net.Http.Json;
using System.Text.Json;
using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.HttpClients;

/// <summary>
/// HTTP client for Game API operations
/// </summary>
public class GameApiClient : IGameApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GameApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public GameApiClient(HttpClient httpClient, ILogger<GameApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<ApiResult<List<GameModel>>> GetAllGamesAsync(string alias)
    {
        try
        {
            _logger.LogInformation("Getting all games for player: {Alias}", alias);
            
            var response = await _httpClient.GetAsync($"api/players/{Uri.EscapeDataString(alias)}/games");

            if (response.IsSuccessStatusCode)
            {
                var games = await response.Content.ReadFromJsonAsync<List<GameModel>>(_jsonOptions) ?? new List<GameModel>();
                _logger.LogInformation("Retrieved {Count} games for player: {Alias}", games.Count, alias);
                return ApiResult<List<GameModel>>.Success(games);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get games. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<List<GameModel>>.Failure($"Failed to get games: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting games");
            return ApiResult<List<GameModel>>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<GameModel>> GetGameAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Getting game {GameId} for player: {Alias}", gameId, alias);
            
            var response = await _httpClient.GetAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}");

            if (response.IsSuccessStatusCode)
            {
                var game = await response.Content.ReadFromJsonAsync<GameModel>(_jsonOptions);
                if (game != null)
                {
                    _logger.LogInformation("Retrieved game {GameId} for player: {Alias}", gameId, alias);
                    return ApiResult<GameModel>.Success(game);
                }
                else
                {
                    _logger.LogWarning("Game {GameId} not found for player: {Alias}", gameId, alias);
                    return ApiResult<GameModel>.Failure("Game not found");
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get game. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<GameModel>.Failure($"Failed to get game: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting game");
            return ApiResult<GameModel>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<GameModel>> CreateGameAsync(string alias, string difficulty)
    {
        try
        {
            _logger.LogInformation("Creating game for player: {Alias}, difficulty: {Difficulty}", alias, difficulty);
            
            var response = await _httpClient.PostAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(difficulty)}", null);

            if (response.IsSuccessStatusCode)
            {
                var game = await response.Content.ReadFromJsonAsync<GameModel>(_jsonOptions);
                if (game != null)
                {
                    _logger.LogInformation("Created game {GameId} for player: {Alias}", game.Id, alias);
                    return ApiResult<GameModel>.Success(game);
                }
                else
                {
                    _logger.LogWarning("Failed to deserialize created game for player: {Alias}", alias);
                    return ApiResult<GameModel>.Failure("Failed to deserialize created game");
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to create game. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<GameModel>.Failure($"Failed to create game: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating game");
            return ApiResult<GameModel>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> MakeMoveAsync(string alias, string gameId, int row, int column, int? value)
    {
        try
        {
            _logger.LogInformation("Making move for game {GameId}, player: {Alias}, position: ({Row}, {Column}), value: {Value}", 
                gameId, alias, row, column, value);
            
            var request = new MoveRequest(row, column, value);
            var response = await _httpClient.PutAsJsonAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}", request, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Move made successfully for game {GameId}", gameId);
                return ApiResult<object>.Success(new object());
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to make move. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<object>.Failure($"Failed to make move: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while making move");
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> AddPossibleValueAsync(string alias, string gameId, int row, int column, int value)
    {
        try
        {
            _logger.LogInformation("Adding possible value {Value} for game {GameId}, position: ({Row}, {Column})", value, gameId, row, column);
            
            var request = new PossibleValueRequest(row, column, value);
            var response = await _httpClient.PostAsJsonAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/possiblevalues", request, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Possible value added successfully for game {GameId}", gameId);
                return ApiResult<object>.Success(new object());
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to add possible value. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<object>.Failure($"Failed to add possible value: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while adding possible value");
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> RemovePossibleValueAsync(string alias, string gameId, int row, int column, int value)
    {
        try
        {
            _logger.LogInformation("Removing possible value {Value} for game {GameId}, position: ({Row}, {Column})", value, gameId, row, column);
            
            var request = new PossibleValueRequest(row, column, value);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/possiblevalues")
            {
                Content = JsonContent.Create(request, options: _jsonOptions)
            };
            
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Possible value removed successfully for game {GameId}", gameId);
                return ApiResult<object>.Success(new object());
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to remove possible value. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<object>.Failure($"Failed to remove possible value: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while removing possible value");
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> ClearPossibleValuesAsync(string alias, string gameId, int row, int column)
    {
        try
        {
            _logger.LogInformation("Clearing possible values for game {GameId}, position: ({Row}, {Column})", gameId, row, column);
            
            var request = new CellRequest(row, column);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/possiblevalues/clear")
            {
                Content = JsonContent.Create(request, options: _jsonOptions)
            };
            
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Possible values cleared successfully for game {GameId}", gameId);
                return ApiResult<object>.Success(new object());
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to clear possible values. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<object>.Failure($"Failed to clear possible values: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while clearing possible values");
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> DeleteGameAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Deleting game {GameId} for player: {Alias}", gameId, alias);
            
            var response = await _httpClient.DeleteAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Game {GameId} deleted successfully for player: {Alias}", gameId, alias);
                return ApiResult<object>.Success(new object());
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to delete game. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<object>.Failure($"Failed to delete game: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting game");
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> DeleteAllGamesAsync(string alias)
    {
        try
        {
            _logger.LogInformation("Deleting all games for player: {Alias}", alias);
            
            var response = await _httpClient.DeleteAsync($"api/players/{Uri.EscapeDataString(alias)}/games");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("All games deleted successfully for player: {Alias}", alias);
                return ApiResult<object>.Success(new object());
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to delete all games. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<object>.Failure($"Failed to delete all games: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting all games");
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> UndoMoveAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Undoing move for game {GameId}, player: {Alias}", gameId, alias);
            
            var response = await _httpClient.PostAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/undo", null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Move undone successfully for game {GameId}", gameId);
                return ApiResult<object>.Success(new object());
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to undo move. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<object>.Failure($"Failed to undo move: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while undoing move");
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<object>> ResetGameAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Resetting game {GameId} for player: {Alias}", gameId, alias);
            
            var response = await _httpClient.PostAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/reset", null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Game {GameId} reset successfully for player: {Alias}", gameId, alias);
                return ApiResult<object>.Success(new object());
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to reset game. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<object>.Failure($"Failed to reset game: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while resetting game");
            return ApiResult<object>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<ValidationResultModel>> ValidateGameAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Validating game {GameId} for player: {Alias}", gameId, alias);
            
            var response = await _httpClient.PostAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/validate", null);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ValidationResultModel>(_jsonOptions);
                if (result != null)
                {
                    _logger.LogInformation("Game {GameId} validated successfully for player: {Alias}", gameId, alias);
                    return ApiResult<ValidationResultModel>.Success(result);
                }
                else
                {
                    _logger.LogWarning("Failed to deserialize validation result for game {GameId}", gameId);
                    return ApiResult<ValidationResultModel>.Failure("Failed to deserialize validation result");
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to validate game. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<ValidationResultModel>.Failure($"Failed to validate game: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while validating game");
            return ApiResult<ValidationResultModel>.Failure($"Exception occurred: {ex.Message}");
        }
    }
}