using System.Text.Json;
using Sudoku.Blazor.Models;
using Sudoku.Blazor.Models.Requests;

namespace Sudoku.Blazor.Services.HttpClients;

/// <summary>
/// Provides methods for interacting with a game API, including retrieving, creating, updating, and deleting games, as
/// well as performing game-specific actions such as making moves, validating, and resetting games.
/// </summary>
public class GameApiClient(HttpClient httpClient, ILogger<GameApiClient> logger) : IGameApiClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<GameApiClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<ApiResult<bool>> AddPossibleValueAsync(string profileId, string gameId, int row, int column, int value)
    {
        try
        {
            _logger.LogInformation("Adding possible value {Value} for game {GameId}, position: ({Row}, {Column})", value, gameId, row, column);

            var request = new PossibleValueRequest(row, column, value);
            var response = await _httpClient.PostAsJsonAsync($"api/players/{Uri.EscapeDataString(profileId)}/games/{Uri.EscapeDataString(gameId)}/possible-values", request, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Possible value added successfully for game {GameId}", gameId);
                return ApiResult<bool>.Success(true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to add possible value. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<bool>.Failure($"Failed to add possible value: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while adding possible value");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<bool>> ClearPossibleValuesAsync(string profileId, string gameId, int row, int column)
    {
        try
        {
            _logger.LogInformation("Clearing possible values for game {GameId}, position: ({Row}, {Column})", gameId, row, column);

            var request = new CellRequest(row, column);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"api/players/{Uri.EscapeDataString(profileId)}/games/{Uri.EscapeDataString(gameId)}/possible-values/clear")
            {
                Content = JsonContent.Create(request, options: _jsonOptions)
            };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Possible values cleared successfully for game {GameId}", gameId);
                return ApiResult<bool>.Success(true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to clear possible values. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<bool>.Failure($"Failed to clear possible values: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while clearing possible values");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<GameModel>> CreateGameAsync(string profileId, string difficulty)
    {
        try
        {
            _logger.LogInformation("Creating game for profile: {ProfileId}, difficulty: {Difficulty}", profileId, difficulty);

            var response = await _httpClient.PostAsync($"api/players/{Uri.EscapeDataString(profileId)}/games/{Uri.EscapeDataString(difficulty)}", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to create game. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<GameModel>.Failure($"Failed to create game: {error}");
            }

            var location = response.Headers.Location?.ToString();
            var gameId = location?.Split('/').LastOrDefault();
            if (string.IsNullOrEmpty(gameId))
            {
                _logger.LogWarning("Created game but Location header was missing or unparseable for profile: {ProfileId}", profileId);
                return ApiResult<GameModel>.Failure("Game created but could not determine game ID from Location header");
            }

            _logger.LogInformation("Created game {GameId} for profile: {ProfileId}, fetching full game", gameId, profileId);
            return await GetGameAsync(profileId, gameId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating game");
            return ApiResult<GameModel>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<bool>> DeleteAllGamesAsync(string profileId)
    {
        try
        {
            _logger.LogInformation("Deleting all games for profile: {ProfileId}", profileId);

            var response = await _httpClient.DeleteAsync($"api/players/{Uri.EscapeDataString(profileId)}/games");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("All games deleted successfully for profile: {ProfileId}", profileId);
                return ApiResult<bool>.Success(true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to delete all games. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<bool>.Failure($"Failed to delete all games: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting all games");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<bool>> DeleteGameAsync(string profileId, string gameId)
    {
        try
        {
            _logger.LogInformation("Deleting game {GameId} for profile: {ProfileId}", gameId, profileId);

            var response = await _httpClient.DeleteAsync($"api/players/{Uri.EscapeDataString(profileId)}/games/{Uri.EscapeDataString(gameId)}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Game {GameId} deleted successfully for profile: {ProfileId}", gameId, profileId);
                return ApiResult<bool>.Success(true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to delete game. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<bool>.Failure($"Failed to delete game: {error}", (int)response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting game");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<List<GameModel>>> GetAllGamesAsync(string profileId)
    {
        try
        {
            _logger.LogInformation("Getting all games for profile: {ProfileId}", profileId);

            var response = await _httpClient.GetAsync($"api/players/{Uri.EscapeDataString(profileId)}/games");

            if (response.IsSuccessStatusCode)
            {
                var games = await response.Content.ReadFromJsonAsync<List<GameModel>>(_jsonOptions) ?? new List<GameModel>();
                _logger.LogInformation("Retrieved {Count} games for profile: {ProfileId}", games.Count, profileId);
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

    public async Task<ApiResult<GameModel>> GetGameAsync(string profileId, string gameId)
    {
        try
        {
            _logger.LogInformation("Getting game {GameId} for profile: {ProfileId}", gameId, profileId);

            var response = await _httpClient.GetAsync($"api/players/{Uri.EscapeDataString(profileId)}/games/{Uri.EscapeDataString(gameId)}");

            if (response.IsSuccessStatusCode)
            {
                var game = await response.Content.ReadFromJsonAsync<GameModel>(_jsonOptions);
                if (game != null)
                {
                    _logger.LogInformation("Retrieved game {GameId} for profile: {ProfileId}", gameId, profileId);
                    return ApiResult<GameModel>.Success(game);
                }
                else
                {
                    _logger.LogWarning("Game {GameId} not found for profile: {ProfileId}", gameId, profileId);
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

    public async Task<ApiResult<bool>> MakeMoveAsync(string profileId, string gameId, int row, int column, int? value, TimeSpan playDuration)
    {
        try
        {
            _logger.LogInformation("Making move for game {GameId}, profile: {ProfileId}, position: ({Row}, {Column}), value: {Value}",
                gameId, profileId, row, column, value);

            var request = new MoveRequest(row, column, value, playDuration);
            var response = await _httpClient.PutAsJsonAsync($"api/players/{Uri.EscapeDataString(profileId)}/games/{Uri.EscapeDataString(gameId)}/actions", request, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Move made successfully for game {GameId}", gameId);
                return ApiResult<bool>.Success(true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to make move. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<bool>.Failure($"Failed to make move: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while making move");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<bool>> RemovePossibleValueAsync(string profileId, string gameId, int row, int column, int value)
    {
        try
        {
            _logger.LogInformation("Removing possible value {Value} for game {GameId}, position: ({Row}, {Column})", value, gameId, row, column);

            var request = new PossibleValueRequest(row, column, value);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"api/players/{Uri.EscapeDataString(profileId)}/games/{Uri.EscapeDataString(gameId)}/possible-values")
            {
                Content = JsonContent.Create(request, options: _jsonOptions)
            };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Possible value removed successfully for game {GameId}", gameId);
                return ApiResult<bool>.Success(true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to remove possible value. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<bool>.Failure($"Failed to remove possible value: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while removing possible value");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<bool>> ResetGameAsync(string profileId, string gameId)
    {
        try
        {
            _logger.LogInformation("Resetting game {GameId} for profile: {ProfileId}", gameId, profileId);

            var response = await _httpClient.PostAsync($"api/players/{Uri.EscapeDataString(profileId)}/games/{Uri.EscapeDataString(gameId)}/actions/reset", null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Game {GameId} reset successfully for profile: {ProfileId}", gameId, profileId);
                return ApiResult<bool>.Success(true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to reset game. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<bool>.Failure($"Failed to reset game: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while resetting game");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<bool>> SaveGameAsync(GameModel game)
    {
        try
        {
            if (game == null) throw new ArgumentNullException(nameof(game));

            _logger.LogInformation("Saving game {GameId} for profile: {ProfileId}", game.Id, game.ProfileId);

            var response = await _httpClient.PutAsJsonAsync($"api/players/{Uri.EscapeDataString(game.ProfileId)}/games/{Uri.EscapeDataString(game.Id)}", game, _jsonOptions);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Game {GameId} saved successfully for profile: {ProfileId}", game.Id, game.ProfileId);
                return ApiResult<bool>.Success(true);
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to save game. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
            return ApiResult<bool>.Failure($"Failed to save game: {error}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while saving game");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<bool>> AbandonGameAsync(string profileId, string gameId)
        => await PostStatusAsync(profileId, gameId, "abandon");

    public async Task<ApiResult<bool>> CompleteGameAsync(string profileId, string gameId)
        => await PostStatusAsync(profileId, gameId, "complete");

    public async Task<ApiResult<bool>> PauseGameAsync(string profileId, string gameId)
        => await PostStatusAsync(profileId, gameId, "pause");

    public async Task<ApiResult<bool>> ResumeGameAsync(string profileId, string gameId)
        => await PostStatusAsync(profileId, gameId, "resume");

    private async Task<ApiResult<bool>> PostStatusAsync(string profileId, string gameId, string action)
    {
        try
        {
            _logger.LogInformation("Posting game status '{Action}' for game {GameId}, profile: {ProfileId}", action, gameId, profileId);
            var response = await _httpClient.PostAsync($"api/players/{Uri.EscapeDataString(profileId)}/games/{Uri.EscapeDataString(gameId)}/status/{action}", null);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Game status '{Action}' applied successfully for game {GameId}", action, gameId);
                return ApiResult<bool>.Success(true);
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to apply game status '{Action}'. Status: {StatusCode}, Error: {Error}", action, response.StatusCode, error);
            return ApiResult<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while applying game status '{Action}'", action);
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<bool>> UndoMoveAsync(string profileId, string gameId)
    {
        try
        {
            _logger.LogInformation("Undoing move for game {GameId}, profile: {ProfileId}", gameId, profileId);

            var response = await _httpClient.PostAsync($"api/players/{Uri.EscapeDataString(profileId)}/games/{Uri.EscapeDataString(gameId)}/actions/undo", null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Move undone successfully for game {GameId}", gameId);
                return ApiResult<bool>.Success(true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to undo move. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<bool>.Failure($"Failed to undo move: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while undoing move");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<ValidationResultModel>> ValidateGameAsync(string profileId, string gameId)
    {
        try
        {
            _logger.LogInformation("Validating game {GameId} for profile: {ProfileId}", gameId, profileId);

            var response = await _httpClient.PostAsync($"api/players/{Uri.EscapeDataString(profileId)}/games/{Uri.EscapeDataString(gameId)}/status/validate", null);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ValidationResultModel>(_jsonOptions);
                if (result != null)
                {
                    _logger.LogInformation("Game {GameId} validated successfully for profile: {ProfileId}", gameId, profileId);
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
