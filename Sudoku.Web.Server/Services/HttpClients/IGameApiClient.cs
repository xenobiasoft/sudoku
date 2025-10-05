using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.HttpClients;

/// <summary>
/// Interface for Game API client operations
/// </summary>
public interface IGameApiClient
{
    /// <summary>
    /// Adds a possible value to a cell
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <param name="row">The row of the cell</param>
    /// <param name="column">The column of the cell</param>
    /// <param name="value">The possible value to add</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> AddPossibleValueAsync(string alias, string gameId, int row, int column, int value);

    /// <summary>
    /// Clears all possible values from a cell
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <param name="row">The row of the cell</param>
    /// <param name="column">The column of the cell</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> ClearPossibleValuesAsync(string alias, string gameId, int row, int column);

    /// <summary>
    /// Creates a new game for the specified player with the given difficulty
    /// </summary>
    /// <param name="alias">The alias of the player</param>
    /// <param name="difficulty">The difficulty level of the game</param>
    /// <returns>The created game</returns>
    Task<ApiResult<GameModel>> CreateGameAsync(string alias, string difficulty);

    /// <summary>
    /// Deletes all games for a player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> DeleteAllGamesAsync(string alias);

    /// <summary>
    /// Deletes a specific game for a player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id to delete</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> DeleteGameAsync(string alias, string gameId);

    /// <summary>
    /// Gets all games for a specific player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <returns>A list of games for the player</returns>
    Task<ApiResult<List<GameModel>>> GetAllGamesAsync(string alias);

    /// <summary>
    /// Gets a specific game by id for a player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <returns>The game if found</returns>
    Task<ApiResult<GameModel>> GetGameAsync(string alias, string gameId);

    /// <summary>
    /// Removes a possible value from a cell
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <param name="row">The row of the cell</param>
    /// <param name="column">The column of the cell</param>
    /// <param name="value">The possible value to remove</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> RemovePossibleValueAsync(string alias, string gameId, int row, int column, int value);

    /// <summary>
    /// Resets a game to its initial state
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> ResetGameAsync(string alias, string gameId);

    /// <summary>
    /// Saves the specified game asynchronously.
    /// </summary>
    /// <param name="game">The game model to be saved. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains an  <see cref="ApiResult{T}"/>
    /// indicating whether the save operation was successful.</returns>
    Task<ApiResult<bool>> SaveGameAsync(GameModel game);

    /// <summary>
    /// Undoes the last move in a game
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> UndoMoveAsync(string alias, string gameId);

    /// <summary>
    /// Validates a game to check if it's completed correctly
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <returns>Result of the validation</returns>
    Task<ApiResult<ValidationResultModel>> ValidateGameAsync(string alias, string gameId);
}