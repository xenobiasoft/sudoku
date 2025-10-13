using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.Abstractions.V2;

/// <summary>
/// Manages player operations including creation, loading, and verification of player aliases.
/// </summary>
public interface IPlayerManager
{
    /// <summary>
    /// Creates a new player with an optional custom alias.
    /// If no alias is provided, the server will auto-generate one.
    /// </summary>
    /// <param name="alias">Optional custom alias for the player</param>
    /// <returns>The created player's alias</returns>
    Task<string> CreatePlayerAsync(string? alias = null);
    
    /// <summary>
    /// Checks if a player with the given alias exists on the server.
    /// </summary>
    /// <param name="alias">The player's alias to verify</param>
    /// <returns>True if the player exists, false otherwise</returns>
    Task<bool> PlayerExistsAsync(string alias);
    
    /// <summary>
    /// Gets the current player alias from local storage.
    /// </summary>
    /// <returns>The current player's alias, or null if none is set</returns>
    Task<string?> GetCurrentPlayerAsync();
    
    /// <summary>
    /// Sets the current player alias in local storage.
    /// </summary>
    /// <param name="alias">The player's alias to store</param>
    Task SetCurrentPlayerAsync(string alias);
}