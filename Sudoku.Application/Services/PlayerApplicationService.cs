using Microsoft.Extensions.Logging;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Services;

public class PlayerApplicationService(IGameRepository gameRepository, ILogger<PlayerApplicationService> logger) : IPlayerApplicationService
{
    public async Task<Result<string>> CreatePlayerAsync(string? alias = null)
    {
        try
        {
            // If no alias is provided, generate a random one
            if (string.IsNullOrWhiteSpace(alias))
            {
                alias = GenerateRandomAlias();
            }

            var playerAlias = PlayerAlias.Create(alias);
            
            // Check if the player already exists by getting games for this alias
            var existingGames = await gameRepository.GetByPlayerAsync(playerAlias);
            if (existingGames.Any())
            {
                return Result<string>.Success(playerAlias.Value);
            }

            logger.LogInformation("Created new player with alias: {Alias}", playerAlias.Value);
            return Result<string>.Success(playerAlias.Value);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to create player: {Error}", ex.Message);
            return Result<string>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred creating player");
            return Result<string>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeletePlayerAsync(string alias)
    {
        try
        {
            var playerAlias = PlayerAlias.Create(alias);
            
            // Delete all games for this player
            var games = await gameRepository.GetByPlayerAsync(playerAlias);
            
            foreach (var game in games)
            {
                await gameRepository.DeleteAsync(game.Id);
            }
            
            logger.LogInformation("Deleted player {Alias} with {Count} games", playerAlias.Value, games.Count());
            return Result<bool>.Success(true);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to delete player: {Error}", ex.Message);
            return Result<bool>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred deleting player");
            return Result<bool>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<bool>> PlayerExistsAsync(string alias)
    {
        try
        {
            var playerAlias = PlayerAlias.Create(alias);
            
            // Check if the player exists by getting games for this alias
            var existingGames = await gameRepository.GetByPlayerAsync(playerAlias);
            var exists = existingGames.Any();
            
            return Result<bool>.Success(exists);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to check if player exists: {Error}", ex.Message);
            return Result<bool>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred checking if player exists");
            return Result<bool>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
    
    private string GenerateRandomAlias()
    {
        // Generate a random player alias with a prefix and a random number
        var random = new Random();
        var prefix = "Player";
        var randomNumber = random.Next(1000, 10000);
        return $"{prefix}{randomNumber}";
    }
}