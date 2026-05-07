using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Specifications;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services;

namespace Sudoku.Infrastructure.Repositories;

public class AzureBlobGameRepository(IAzureStorageService storageService, ILogger<AzureBlobGameRepository> logger) : IGameRepository, IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private const string ContainerName = "sudoku-games";

    public async Task<SudokuGame?> GetByIdAsync(GameId id)
    {
        try
        {
            // We need to find the game by ID, but we don't know the profile ID yet
            // so we need to search through all blobs to find it
            var gamePrefix = $"*/{id.Value}/";
            SudokuGame? game = null;

            await foreach (var blobName in storageService.GetBlobNamesAsync(ContainerName, gamePrefix))
            {
                // The blobName will be in the format "{profileId}/{gameId}/{revisionId}.json"
                // We need to get the latest revision
                var parts = blobName.Split('/');
                if (parts.Length < 3) continue;

                var profileId = parts[0];
                var gameId = parts[1];

                if (gameId == id.Value.ToString())
                {
                    var latestBlobName = await GetLatestRevisionAsync(profileId, id);
                    if (latestBlobName != null)
                    {
                        game = await storageService.LoadAsync<SudokuGame>(ContainerName, latestBlobName);
                        break;
                    }
                }
            }

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

    public async Task<IEnumerable<SudokuGame>> GetByProfileIdAsync(ProfileId profileId)
    {
        try
        {
            var games = new List<SudokuGame>();
            var prefix = $"{profileId}/";
            var gameIdSet = new HashSet<string>();

            await foreach (var blobName in storageService.GetBlobNamesAsync(ContainerName, prefix))
            {
                // Extract the game ID from the blob path
                var parts = blobName.Split('/');
                if (parts.Length < 3) continue;

                var gameId = parts[1];

                // Only process each game ID once
                if (gameIdSet.Add(gameId))
                {
                    var latestBlobName = await GetLatestRevisionAsync(profileId.ToString(), GameId.Create(gameId));
                    if (latestBlobName != null)
                    {
                        var game = await storageService.LoadAsync<SudokuGame>(ContainerName, latestBlobName);
                        if (game != null)
                        {
                            games.Add(game);
                        }
                    }
                }
            }

            logger.LogDebug("Retrieved {Count} games for profile {ProfileId}", games.Count, profileId);
            return games.OrderByDescending(g => g.CreatedAt);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving games for profile {ProfileId}", profileId);
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetByProfileIdAndStatusAsync(ProfileId profileId, GameStatusEnum statusEnum)
    {
        var specification = new GameByProfileIdAndStatusSpecification(profileId, statusEnum);
        return await GetBySpecificationAsync(specification);
    }

    public async Task SaveAsync(SudokuGame game)
    {
        try
        {
            await _semaphore.WaitAsync();

            try
            {
                var nextBlobName = await GetNextRevisionBlobNameAsync(game.ProfileId.ToString(), game.Id);
                await storageService.SaveAsync(ContainerName, nextBlobName, game);

                logger.LogDebug("Saved game {GameId} to Azure Blob Storage at {BlobName}", game.Id.Value, nextBlobName);
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
            // Find all revisions for this game
            var gamePrefix = $"*/{id.Value}/";

            await foreach (var blobName in storageService.GetBlobNamesAsync(ContainerName, gamePrefix))
            {
                await storageService.DeleteAsync(ContainerName, blobName);
                logger.LogDebug("Deleted game revision {BlobName} from Azure Blob Storage", blobName);
            }

            logger.LogDebug("Deleted all revisions of game {GameId} from Azure Blob Storage", id.Value);
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
            // Check if any blob exists with this game ID
            var gamePrefix = $"*/{id.Value}/";

            await foreach (var blobName in storageService.GetBlobNamesAsync(ContainerName, gamePrefix))
            {
                return true;
            }

            return false;
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

    public async Task<IEnumerable<SudokuGame>> GetCompletedGamesAsync(ProfileId? profileId = null)
    {
        var specification = new CompletedGamesSpecification(profileId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<SudokuGame>> GetGamesByDifficultyAsync(GameDifficulty difficulty)
    {
        var specification = new GameByDifficultySpecification(difficulty);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<SudokuGame>> GetGamesByStatusAsync(GameStatusEnum statusEnum)
    {
        var specification = new GameByStatusSpecification(statusEnum);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<int> GetTotalGamesCountAsync(ProfileId? profileId = null)
    {
        try
        {
            var allGames = await GetAllGamesAsync();
            var query = allGames.AsQueryable();

            if (profileId != null)
            {
                query = query.Where(g => g.ProfileId == profileId);
            }

            return query.Count();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting total games count for profile {ProfileId}", profileId);
            throw;
        }
    }

    public async Task<int> GetCompletedGamesCountAsync(ProfileId? profileId = null)
    {
        var specification = new CompletedGamesSpecification(profileId);
        return await CountBySpecificationAsync(specification);
    }

    public async Task<TimeSpan> GetAverageCompletionTimeAsync(ProfileId? profileId = null)
    {
        try
        {
            var completedGames = await GetCompletedGamesAsync(profileId);
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
            logger.LogError(ex, "Error calculating average completion time for profile {ProfileId}", profileId);
            throw;
        }
    }

    private async Task<List<SudokuGame>> GetAllGamesAsync()
    {
        var games = new List<SudokuGame>();
        var processedGameIds = new HashSet<string>();

        await foreach (var blobName in storageService.GetBlobNamesAsync(ContainerName))
        {
            var parts = blobName.Split('/');
            if (parts.Length < 3) continue;

            var profileId = parts[0];
            var gameId = parts[1];

            // Only process each game ID once to get the latest revision
            var gameIdString = gameId.ToString();
            if (processedGameIds.Add(gameIdString))
            {
                var latestBlobName = await GetLatestRevisionAsync(profileId, GameId.Create(gameId));
                if (latestBlobName != null)
                {
                    var game = await storageService.LoadAsync<SudokuGame>(ContainerName, latestBlobName);
                    if (game != null)
                    {
                        games.Add(game);
                    }
                }
            }
        }

        return games;
    }

    private async Task<string?> GetLatestRevisionAsync(string profileId, GameId gameId)
    {
        var prefix = $"{profileId}/{gameId.Value}/";
        var revisions = new List<string>();

        await foreach (var blobName in storageService.GetBlobNamesAsync(ContainerName, prefix))
        {
            revisions.Add(blobName);
        }

        if (revisions.Count == 0)
        {
            return null;
        }

        // Sort by revision number (last part of the path, excluding .json)
        return revisions.OrderByDescending(r =>
        {
            var parts = r.Split('/');
            var fileName = parts[^1];
            var revNumber = System.IO.Path.GetFileNameWithoutExtension(fileName);
            return int.Parse(revNumber);
        }).First();
    }

    private async Task<string> GetNextRevisionBlobNameAsync(string profileId, GameId gameId)
    {
        var prefix = $"{profileId}/{gameId.Value}/";
        var revisions = new List<int>();

        await foreach (var blobName in storageService.GetBlobNamesAsync(ContainerName, prefix))
        {
            var parts = blobName.Split('/');
            var fileName = parts[^1];
            var revNumberStr = System.IO.Path.GetFileNameWithoutExtension(fileName);
            if (int.TryParse(revNumberStr, out var revNumber))
            {
                revisions.Add(revNumber);
            }
        }

        var nextRevision = revisions.Count > 0 ? revisions.Max() + 1 : 1;
        return $"{profileId}/{gameId.Value}/{nextRevision:D5}.json";
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}
