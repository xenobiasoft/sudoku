using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.HttpClients;

/// <summary>
/// Interface for Player API client operations
/// </summary>
public interface IPlayerApiClient
{
    /// <summary>
    /// Creates a new player with an optional alias
    /// </summary>
    /// <param name="alias">Optional custom alias</param>
    /// <returns>The created player's alias</returns>
    Task<ApiResult<string>> CreatePlayerAsync(string? alias = null);
    
    /// <summary>
    /// Checks if a player with the given alias exists
    /// </summary>
    /// <param name="alias">The player's alias to check</param>
    /// <returns>True if the player exists, false otherwise</returns>
    Task<ApiResult<bool>> PlayerExistsAsync(string alias);
}