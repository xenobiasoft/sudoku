using Sudoku.Web.Server.Services.HttpClients;
using ILocalStorageService = Sudoku.Web.Server.Services.Abstractions.ILocalStorageService;
using IPlayerManager = Sudoku.Web.Server.Services.Abstractions.IPlayerManager;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// Manages player operations including creation, loading, and verification of player aliases.
/// This service coordinates between the Player API client and local storage to provide a unified
/// interface for player management operations.
/// </summary>
/// <remarks>This class is responsible for managing player lifecycle operations such as creating new players,
/// verifying existing players, and managing the current player state in local storage. It serves as the single
/// point of access for all player-related operations and handles API communication and local storage coordination.</remarks>
/// <param name="playerApiClient">The client for communicating with the Player API</param>
/// <param name="localStorageService">The service for managing local storage operations</param>
public class PlayerManager(IPlayerApiClient playerApiClient, ILocalStorageService localStorageService) : IPlayerManager
{
    /// <summary>
    /// Creates a new player with an optional custom alias.
    /// If no alias is provided, the server will auto-generate one.
    /// </summary>
    /// <param name="alias">Optional custom alias for the player</param>
    /// <returns>The created player's alias</returns>
    /// <exception cref="Exception">Thrown when the API fails to create the player</exception>
    public async Task<string> CreatePlayerAsync(string? alias = null)
    {
        var response = await playerApiClient.CreatePlayerAsync(alias);
        if (!response.IsSuccess || response.Value == null)
        {
            throw new Exception("Failed to create player.");
        }
        
        // Store the created player alias as the current player
        await localStorageService.SetAliasAsync(response.Value);
        
        return response.Value;
    }
    
    /// <summary>
    /// Checks if a player with the given alias exists on the server.
    /// </summary>
    /// <param name="alias">The player's alias to verify</param>
    /// <returns>True if the player exists, false otherwise</returns>
    /// <exception cref="ArgumentException">Thrown when the alias is null or empty</exception>
    /// <exception cref="Exception">Thrown when the API fails to check player existence</exception>
    public async Task<bool> PlayerExistsAsync(string alias)
    {
        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentException("Alias not set.");
        }
        
        var response = await playerApiClient.PlayerExistsAsync(alias);
        if (!response.IsSuccess)
        {
            throw new Exception("Failed to check if player exists.");
        }
        
        return response.Value;
    }
    
    /// <summary>
    /// Gets the current player alias from local storage.
    /// </summary>
    /// <returns>The current player's alias, or null if none is set</returns>
    public async Task<string?> GetCurrentPlayerAsync()
    {
        return await localStorageService.GetAliasAsync();
    }
    
    /// <summary>
    /// Sets the current player alias in local storage.
    /// </summary>
    /// <param name="alias">The player's alias to store</param>
    /// <exception cref="ArgumentException">Thrown when the alias is null or empty</exception>
    public async Task SetCurrentPlayerAsync(string alias)
    {
        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentException("Alias not set.");
        }
        
        await localStorageService.SetAliasAsync(alias);
    }
}