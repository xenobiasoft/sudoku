using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Specifications;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using XenobiaSoft.Sudoku.Infrastructure.Services;

namespace XenobiaSoft.Sudoku.Infrastructure.Repositories;

public class AzureBlobGameRepository(IAzureStorageService storageService, ILogger<AzureBlobGameRepository> logger)
    : IGameRepository
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private const string ContainerName = "sudoku-games";

    public async Task<SudokuGame?> GetByIdAsync(GameId id)
    {
        try
        {
            var blobName = GetBlobName(id);
            var game = await storageService.LoadAsync<SudokuGame>(ContainerName, blobName);

            if (game == null)
            {
                logger.LogDebug("Game with ID {GameId} not found", id.Value);
                return null;
            }

            logger.LogDebug("Retrieved game {GameId} from Azure Blob Storage", id.Value);
            return game;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving game {GameId} from Azure Blob Storage", id.Value);
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetByPlayerAsync(PlayerAlias playerAlias)
    {
        try
        {
            var games = new List<SudokuGame>();
            var prefix = $"{playerAlias.Value}/";

            await foreach (var blobName in storageService.GetBlobNamesAsync(ContainerName, prefix))
            {
                var game = await storageService.LoadAsync<SudokuGame>(ContainerName, blobName);
                if (game != null)
                {
                    games.Add(game);
                }
            }

            logger.LogDebug("Retrieved {Count} games for player {PlayerAlias}", games.Count, playerAlias.Value);
            return games.OrderByDescending(g => g.CreatedAt);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving games for player {PlayerAlias}", playerAlias.Value);
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetByPlayerAndStatusAsync(PlayerAlias playerAlias, GameStatus status)
    {
        var specification = new GameByPlayerAndStatusSpecification(playerAlias, status);
        return await GetBySpecificationAsync(specification);
    }

    public async Task SaveAsync(SudokuGame game)
    {
        try
        {
            await _semaphore.WaitAsync();

            try
            {
                var blobName = GetBlobName(game.Id);
                await storageService.SaveAsync(ContainerName, blobName, game);

                logger.LogDebug("Saved game {GameId} to Azure Blob Storage", game.Id.Value);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving game {GameId} to Azure Blob Storage", game.Id.Value);
            throw;
        }
    }

    public async Task DeleteAsync(GameId id)
    {
        try
        {
            var blobName = GetBlobName(id);
            await storageService.DeleteAsync(ContainerName, blobName);
            logger.LogDebug("Deleted game {GameId} from Azure Blob Storage", id.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting game {GameId} from Azure Blob Storage", id.Value);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(GameId id)
    {
        try
        {
            var blobName = GetBlobName(id);
            return await storageService.ExistsAsync(ContainerName, blobName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking existence of game {GameId}", id.Value);
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetBySpecificationAsync(ISpecification<SudokuGame> specification)
    {
        try
        {
            var allGames = await GetAllGamesAsync();
            var query = allGames.AsQueryable();

            // Apply criteria
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Apply ordering
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Apply paging
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }

            var result = query.ToList();
            logger.LogDebug("Retrieved {Count} games using specification", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving games using specification");
            throw;
        }
    }

    public async Task<SudokuGame?> GetSingleBySpecificationAsync(ISpecification<SudokuGame> specification)
    {
        var games = await GetBySpecificationAsync(specification);
        return games.FirstOrDefault();
    }

    public async Task<int> CountBySpecificationAsync(ISpecification<SudokuGame> specification)
    {
        try
        {
            var allGames = await GetAllGamesAsync();
            var query = allGames.AsQueryable();

            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            return query.Count();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting games using specification");
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetRecentGamesAsync(int count = 10)
    {
        var specification = new RecentGamesSpecification(count);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<SudokuGame>> GetCompletedGamesAsync(PlayerAlias? playerAlias = null)
    {
        var specification = new CompletedGamesSpecification(playerAlias);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<SudokuGame>> GetGamesByDifficultyAsync(GameDifficulty difficulty)
    {
        var specification = new GameByDifficultySpecification(difficulty);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<SudokuGame>> GetGamesByStatusAsync(GameStatus status)
    {
        var specification = new GameByStatusSpecification(status);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<int> GetTotalGamesCountAsync(PlayerAlias? playerAlias = null)
    {
        try
        {
            var allGames = await GetAllGamesAsync();
            var query = allGames.AsQueryable();

            if (playerAlias != null)
            {
                query = query.Where(g => g.PlayerAlias == playerAlias);
            }

            return query.Count();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting total games count for player {PlayerAlias}", playerAlias?.Value);
            throw;
        }
    }

    public async Task<int> GetCompletedGamesCountAsync(PlayerAlias? playerAlias = null)
    {
        var specification = new CompletedGamesSpecification(playerAlias);
        return await CountBySpecificationAsync(specification);
    }

    public async Task<TimeSpan> GetAverageCompletionTimeAsync(PlayerAlias? playerAlias = null)
    {
        try
        {
            var completedGames = await GetCompletedGamesAsync(playerAlias);
            var gamesWithCompletionTime = completedGames
                .Where(g => g.CompletedAt.HasValue && g.StartedAt.HasValue)
                .ToList();

            if (!gamesWithCompletionTime.Any())
            {
                return TimeSpan.Zero;
            }

            var totalTime = gamesWithCompletionTime.Sum(g =>
                (g.CompletedAt!.Value - g.StartedAt!.Value).TotalMilliseconds);
            var averageMilliseconds = totalTime / gamesWithCompletionTime.Count;
            return TimeSpan.FromMilliseconds(averageMilliseconds);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating average completion time for player {PlayerAlias}", playerAlias?.Value);
            throw;
        }
    }

    private async Task<List<SudokuGame>> GetAllGamesAsync()
    {
        var games = new List<SudokuGame>();

        await foreach (var blobName in storageService.GetBlobNamesAsync(ContainerName))
        {
            var game = await storageService.LoadAsync<SudokuGame>(ContainerName, blobName);
            if (game != null)
            {
                games.Add(game);
            }
        }

        return games;
    }

    private static string GetBlobName(GameId gameId)
    {
        return $"games/{gameId.Value}.json";
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}