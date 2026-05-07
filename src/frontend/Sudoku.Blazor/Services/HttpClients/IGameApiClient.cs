using Sudoku.Blazor.Models;

namespace Sudoku.Blazor.Services.HttpClients;

/// <summary>
/// Interface for Game API client operations
/// </summary>
public interface IGameApiClient
{
    /// <summary>
    /// Adds a possible value to a cell
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <param name="gameId">The game id</param>
    /// <param name="row">The row of the cell</param>
    /// <param name="column">The column of the cell</param>
    /// <param name="value">The possible value to add</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> AddPossibleValueAsync(string profileId, string gameId, int row, int column, int value);

    /// <summary>
    /// Clears all possible values from a cell
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <param name="gameId">The game id</param>
    /// <param name="row">The row of the cell</param>
    /// <param name="column">The column of the cell</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> ClearPossibleValuesAsync(string profileId, string gameId, int row, int column);

    /// <summary>
    /// Creates a new game for the specified player with the given difficulty
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <param name="difficulty">The difficulty level of the game</param>
    /// <returns>The created game</returns>
    Task<ApiResult<GameModel>> CreateGameAsync(string profileId, string difficulty);

    /// <summary>
    /// Deletes all games for a player
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> DeleteAllGamesAsync(string profileId);

    /// <summary>
    /// Deletes a specific game for a player
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <param name="gameId">The game id to delete</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> DeleteGameAsync(string profileId, string gameId);

    /// <summary>
    /// Gets all games for a specific player
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <returns>A list of games for the player</returns>
    Task<ApiResult<List<GameModel>>> GetAllGamesAsync(string profileId);

    /// <summary>
    /// Gets a specific game by id for a player
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <param name="gameId">The game id</param>
    /// <returns>The game if found</returns>
    Task<ApiResult<GameModel>> GetGameAsync(string profileId, string gameId);

    /// <summary>
    /// Attempts to make a move in the specified game for the given player alias asynchronously.
    /// </summary>
    /// <remarks>This method validates the move according to the current game state and rules. If the move is
    /// invalid or the game is not found, the result will indicate failure. Thread safety and concurrent moves are
    /// managed internally.</remarks>
    /// <param name="alias">The unique identifier or username representing the player making the move. Cannot be null or empty.</param>
    /// <param name="gameId">The identifier of the game in which the move is to be made. Cannot be null or empty.</param>
    /// <param name="row">The zero-based row index where the move will be placed. Must be within the valid range for the game board.</param>
    /// <param name="column">The zero-based column index where the move will be placed. Must be within the valid range for the game board.</param>
    /// <param name="value">The value to place at the specified position, if applicable. If null, the default move value will be used.</param>
    /// <param name="playDuration">The duration of the player's turn.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains an ApiResult indicating whether the move
    /// was successful (<see langword="true"/> if the move was made; otherwise, <see langword="false"/>).</returns>
    Task<ApiResult<bool>> MakeMoveAsync(string profileId, string gameId, int row, int column, int? value, TimeSpan playDuration);

    /// <summary>
    /// Removes a possible value from a cell
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <param name="gameId">The game id</param>
    /// <param name="row">The row of the cell</param>
    /// <param name="column">The column of the cell</param>
    /// <param name="value">The possible value to remove</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> RemovePossibleValueAsync(string profileId, string gameId, int row, int column, int value);

    /// <summary>
    /// Resets a game to its initial state
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <param name="gameId">The game id</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> ResetGameAsync(string profileId, string gameId);

    /// <summary>
    /// Saves the specified game asynchronously.
    /// </summary>
    /// <param name="game">The game model to be saved. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains an  <see cref="ApiResult{T}"/>
    /// indicating whether the save operation was successful.</returns>
    Task<ApiResult<bool>> SaveGameAsync(GameModel game);

    /// <summary>Abandons an in-progress or paused game.</summary>
    Task<ApiResult<bool>> AbandonGameAsync(string profileId, string gameId);

    /// <summary>Marks a game as complete.</summary>
    Task<ApiResult<bool>> CompleteGameAsync(string profileId, string gameId);

    /// <summary>Pauses an in-progress game.</summary>
    Task<ApiResult<bool>> PauseGameAsync(string profileId, string gameId);

    /// <summary>Resumes a paused or not-started game.</summary>
    Task<ApiResult<bool>> ResumeGameAsync(string profileId, string gameId);

    /// <summary>
    /// Undoes the last move in a game
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <param name="gameId">The game id</param>
    /// <returns>Success or failure result</returns>
    Task<ApiResult<bool>> UndoMoveAsync(string profileId, string gameId);

    /// <summary>
    /// Validates a game to check if it's completed correctly
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <param name="gameId">The game id</param>
    /// <returns>Result of the validation</returns>
    Task<ApiResult<ValidationResultModel>> ValidateGameAsync(string profileId, string gameId);
}