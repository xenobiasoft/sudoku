using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Models.Requests;
using System.Text.Json;

namespace Sudoku.Web.Server.Services.HttpClients;

/// <summary>
/// Provides methods for interacting with a game API, including retrieving, creating, updating, and deleting games, as
/// well as performing game-specific actions such as making moves, validating, and resetting games.
/// </summary>
/// <remarks>This client is designed to communicate with a game API using HTTP requests. It supports operations
/// for managing games associated with a specific player, identified by their alias. The client handles serialization
/// and deserialization of JSON payloads and logs relevant information and errors during API interactions.  Typical
/// usage involves creating an instance of <see cref="GameApiClient"/> with an <see cref="HttpClient"/> and <see
/// cref="ILogger{TCategoryName}"/> dependency, and then calling the provided asynchronous methods to interact with the
/// API.  All methods return an <see cref="ApiResult{T}"/> object, which encapsulates the result of the operation,
/// including success status, data (if applicable), and error messages.</remarks>
public class GameApiClient(HttpClient httpClient, ILogger<GameApiClient> logger) : IGameApiClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<GameApiClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Adds a possible value to the specified game at the given position.
    /// </summary>
    /// <remarks>This method sends a request to the server to add a possible value for the specified game and
    /// position. If the operation fails, the returned <see cref="ApiResult{T}"/> will contain an error message
    /// describing the issue.</remarks>
    /// <param name="alias">The alias of the player associated with the game.</param>
    /// <param name="gameId">The unique identifier of the game.</param>
    /// <param name="row">The row index of the position where the value is to be added.</param>
    /// <param name="column">The column index of the position where the value is to be added.</param>
    /// <param name="value">The possible value to add at the specified position.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing an empty object if the operation succeeds,  or an error message if the
    /// operation fails.</returns>
    public async Task<ApiResult<bool>> AddPossibleValueAsync(string alias, string gameId, int row, int column, int value)
    {
        try
        {
            _logger.LogInformation("Adding possible value {Value} for game {GameId}, position: ({Row}, {Column})", value, gameId, row, column);
            
            var request = new PossibleValueRequest(row, column, value);
            var response = await _httpClient.PostAsJsonAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/possible-values", request, _jsonOptions);

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

    /// <summary>
    /// Clears all possible values for a specific cell in a game.
    /// </summary>
    /// <remarks>This method sends a request to the server to clear all possible values for the specified
    /// cell.  If the operation is successful, an empty object is returned. If the operation fails,  the result will
    /// contain an error message describing the failure.</remarks>
    /// <param name="alias">The alias of the player associated with the game.</param>
    /// <param name="gameId">The unique identifier of the game.</param>
    /// <param name="row">The row index of the cell to clear, starting from 0.</param>
    /// <param name="column">The column index of the cell to clear, starting from 0.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing an empty object if the operation succeeds,  or an error message if the
    /// operation fails.</returns>
    public async Task<ApiResult<bool>> ClearPossibleValuesAsync(string alias, string gameId, int row, int column)
    {
        try
        {
            _logger.LogInformation("Clearing possible values for game {GameId}, position: ({Row}, {Column})", gameId, row, column);
            
            var request = new CellRequest(row, column);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/possible-values/clear")
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

    /// <summary>
    /// Asynchronously creates a new game for the specified player with the given difficulty level.
    /// </summary>
    /// <remarks>This method sends a request to the server to create a new game for the specified player. If
    /// the operation is successful, the created game is returned. If the operation fails, an error message is included
    /// in the result. Common failure scenarios include invalid input, server errors, or network issues.</remarks>
    /// <param name="alias">The alias of the player for whom the game is being created. Cannot be null or empty.</param>
    /// <param name="difficulty">The difficulty level of the game to be created. Cannot be null or empty.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing the created <see cref="GameModel"/> if the operation succeeds, or an
    /// error message if the operation fails.</returns>
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

    /// <summary>
    /// Deletes all games associated with the specified player alias.
    /// </summary>
    /// <remarks>This method sends an HTTP DELETE request to the server to remove all games for the specified
    /// player. If the operation is successful, a success result is returned. If the operation fails, the method logs
    /// the error and returns a failure result with the error details.</remarks>
    /// <param name="alias">The alias of the player whose games are to be deleted. Cannot be null or empty.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing the result of the operation.  If successful, the result contains an
    /// empty object.  If the operation fails, the result contains an error message.</returns>
    public async Task<ApiResult<bool>> DeleteAllGamesAsync(string alias)
    {
        try
        {
            _logger.LogInformation("Deleting all games for player: {Alias}", alias);
            
            var response = await _httpClient.DeleteAsync($"api/players/{Uri.EscapeDataString(alias)}/games");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("All games deleted successfully for player: {Alias}", alias);
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

    /// <summary>
    /// Deletes a game associated with a specific player.
    /// </summary>
    /// <remarks>This method sends an HTTP DELETE request to the server to remove the specified game for the
    /// given player.  If the operation is successful, an informational log is recorded.  If the operation fails, a
    /// warning or error log is recorded, and the failure details are included in the result.</remarks>
    /// <param name="alias">The alias of the player whose game is to be deleted. This value cannot be null or empty.</param>
    /// <param name="gameId">The unique identifier of the game to delete. This value cannot be null or empty.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing the result of the operation.  If successful, the result contains an
    /// empty object.  If the operation fails, the result contains an error message.</returns>
    public async Task<ApiResult<bool>> DeleteGameAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Deleting game {GameId} for player: {Alias}", gameId, alias);
            
            var response = await _httpClient.DeleteAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Game {GameId} deleted successfully for player: {Alias}", gameId, alias);
                return ApiResult<bool>.Success(true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to delete game. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<bool>.Failure($"Failed to delete game: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting game");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a list of games associated with the specified player's alias.
    /// </summary>
    /// <remarks>This method sends an HTTP GET request to retrieve the games for the specified player.  If the
    /// request is successful, the returned list contains the games associated with the player.  If the request fails or
    /// an exception occurs, the result will indicate failure with an appropriate error message.</remarks>
    /// <param name="alias">The alias of the player whose games are to be retrieved. Cannot be null or empty.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing a list of <see cref="GameModel"/> objects if the operation is
    /// successful. If the operation fails, the result contains an error message describing the failure.</returns>
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

    /// <summary>
    /// Asynchronously retrieves a game for a specified player based on the provided alias and game ID.
    /// </summary>
    /// <remarks>This method sends an HTTP GET request to retrieve the game data for the specified player. If
    /// the game is successfully retrieved, it is returned as part of the result. If the game is not found or an error
    /// occurs, the result will indicate failure with an appropriate error message.</remarks>
    /// <param name="alias">The alias of the player whose game is being retrieved. Cannot be null or empty.</param>
    /// <param name="gameId">The unique identifier of the game to retrieve. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ApiResult{T}"/> object
    /// that indicates the success or failure of the operation. On success, the result contains the <see
    /// cref="GameModel"/> representing the retrieved game. On failure, the result contains an error message.</returns>
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

    /// <summary>
    /// Sends a move request for a player in a specified game to the server.
    /// </summary>
    /// <remarks>This method sends an HTTP PUT request to the server to register the player's move.  If the
    /// request is successful, the server processes the move and updates the game state.  If the request fails, an error
    /// message is returned in the result.</remarks>
    /// <param name="alias">The alias of the player making the move. Cannot be null or empty.</param>
    /// <param name="gameId">The unique identifier of the game. Cannot be null or empty.</param>
    /// <param name="row">The row index of the move. Must be a valid row within the game board.</param>
    /// <param name="column">The column index of the move. Must be a valid column within the game board.</param>
    /// <param name="value">The optional value associated with the move, if applicable.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing the result of the move operation.  If successful, the result contains
    /// an empty object. If the operation fails, the result contains an error message.</returns>
    public async Task<ApiResult<bool>> MakeMoveAsync(string alias, string gameId, int row, int column, int? value)
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

    /// <summary>
    /// Removes a possible value from a specific cell in a game for a given player.
    /// </summary>
    /// <remarks>This method sends an HTTP DELETE request to the server to remove the specified possible value
    /// from the given cell in the game. If the operation is successful, the method returns a success result. Otherwise,
    /// it returns a failure result with an error message.</remarks>
    /// <param name="alias">The alias of the player for whom the possible value is being removed.</param>
    /// <param name="gameId">The unique identifier of the game.</param>
    /// <param name="row">The row index of the cell from which the possible value is to be removed.</param>
    /// <param name="column">The column index of the cell from which the possible value is to be removed.</param>
    /// <param name="value">The possible value to be removed from the specified cell.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing an empty object if the operation succeeds, or an error message if it
    /// fails.</returns>
    public async Task<ApiResult<bool>> RemovePossibleValueAsync(string alias, string gameId, int row, int column, int value)
    {
        try
        {
            _logger.LogInformation("Removing possible value {Value} for game {GameId}, position: ({Row}, {Column})", value, gameId, row, column);
            
            var request = new PossibleValueRequest(row, column, value);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/possible-values")
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

    /// <summary>
    /// Resets the specified game for the given player.
    /// </summary>
    /// <remarks>This method sends a request to reset the specified game for the player identified by
    /// <paramref name="alias"/>. If the operation is successful, the game is reset, and an empty object is returned. 
    /// If the operation fails, an error message is included in the result.</remarks>
    /// <param name="alias">The alias of the player whose game is being reset. Cannot be null or empty.</param>
    /// <param name="gameId">The unique identifier of the game to reset. Cannot be null or empty.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing the result of the operation.  If successful, the result contains an
    /// empty object.  If the operation fails, the result contains an error message.</returns>
    public async Task<ApiResult<bool>> ResetGameAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Resetting game {GameId} for player: {Alias}", gameId, alias);
            
            var response = await _httpClient.PostAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/reset", null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Game {GameId} reset successfully for player: {Alias}", gameId, alias);
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

    /// <summary>
    /// Saves the specified game asynchronously.
    /// </summary>
    /// <param name="game">The game model to be saved. Cannot be <see langword="null"/>.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains  an <see cref="ApiResult{T}"/>
    /// indicating whether the save operation was successful.</returns>
    public async Task<ApiResult<bool>> SaveGameAsync(GameModel game)
    {
        try
        {
            if (game == null) throw new ArgumentNullException(nameof(game));

            _logger.LogInformation("Saving game {GameId} for player: {Alias}", game.Id, game.PlayerAlias);
            
            var response = await _httpClient.PutAsJsonAsync($"api/players/{Uri.EscapeDataString(game.PlayerAlias)}/games/{Uri.EscapeDataString(game.Id)}", game, _jsonOptions);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Game {GameId} saved successfully for player: {Alias}", game.Id, game.PlayerAlias);
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

    /// <summary>
    /// Asynchronously saves the current status of a game for the specified user alias and game identifier.
    /// </summary>
    /// <param name="alias">The unique alias representing the user whose game status will be saved. Cannot be null or empty.</param>
    /// <param name="gameId">The identifier of the game for which the status is being saved. Cannot be null or empty.</param>
    /// <param name="gameStatus">The serialized status data to be saved for the game. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public async Task<ApiResult<bool>> SaveGameStatusAsync(string alias, string gameId, string gameStatus)
    {
        if (string.IsNullOrEmpty(alias)) throw new ArgumentNullException(nameof(alias));
        if (string.IsNullOrEmpty(gameId)) throw new ArgumentNullException(nameof(gameId));
        if (string.IsNullOrEmpty(gameStatus)) throw new ArgumentNullException(nameof(gameStatus));

        try
        {
            _logger.LogInformation("Saving game status for game {GameId}, player: {Alias}", gameId, alias);
            
            var response = await _httpClient.PostAsJsonAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/status", gameStatus, _jsonOptions);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Game status saved successfully for game {GameId}", gameId);
                return ApiResult<bool>.Success(true);
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to save game status. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
            return ApiResult<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while saving game status");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Attempts to undo the last move for a specified player in a given game.
    /// </summary>
    /// <remarks>This method sends a request to the server to undo the last move for the specified player in
    /// the specified game. The operation may fail if the server rejects the request or if an exception occurs during
    /// the process.</remarks>
    /// <param name="alias">The alias of the player for whom the move should be undone. Cannot be null or empty.</param>
    /// <param name="gameId">The unique identifier of the game in which the move should be undone. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ApiResult{T}"/>
    /// object: <list type="bullet"> <item><description>On success, the result contains an empty
    /// object.</description></item> <item><description>On failure, the result contains an error message describing the
    /// issue.</description></item> </list></returns>
    public async Task<ApiResult<bool>> UndoMoveAsync(string alias, string gameId)
    {
        try
        {
            _logger.LogInformation("Undoing move for game {GameId}, player: {Alias}", gameId, alias);
            
            var response = await _httpClient.PostAsync($"api/players/{Uri.EscapeDataString(alias)}/games/{Uri.EscapeDataString(gameId)}/undo", null);

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

    /// <summary>
    /// Validates a game for a specific player by sending a validation request to the server.
    /// </summary>
    /// <remarks>This method sends an HTTP POST request to the server to validate the specified game for the
    /// given player. If the server responds with a success status code, the validation result is deserialized and
    /// returned. If the response cannot be deserialized or the server returns an error, the method returns a failure
    /// result with an appropriate error message.</remarks>
    /// <param name="alias">The alias of the player for whom the game is being validated. Cannot be null or empty.</param>
    /// <param name="gameId">The unique identifier of the game to validate. Cannot be null or empty.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing a <see cref="ValidationResultModel"/> if the validation is successful.
    /// If the validation fails, the result contains an error message describing the failure.</returns>
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