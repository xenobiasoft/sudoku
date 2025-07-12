using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Specifications;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;

namespace XenobiaSoft.Sudoku.Infrastructure.Repositories;

public class AzureBlobGameRepository : IGameRepository
{
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<AzureBlobGameRepository> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AzureBlobGameRepository(BlobContainerClient containerClient, ILogger<AzureBlobGameRepository> logger)
    {
        _containerClient = containerClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<SudokuGame?> GetByIdAsync(GameId id)
    {
        try
        {
            var blobName = GetBlobName(id);
            var blobClient = _containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                _logger.LogDebug("Game with ID {GameId} not found", id.Value);
                return null;
            }

            var response = await blobClient.DownloadAsync();
            using var streamReader = new StreamReader(response.Value.Content);
            var json = await streamReader.ReadToEndAsync();
            var game = JsonSerializer.Deserialize<SudokuGame>(json, _jsonOptions);

            _logger.LogDebug("Retrieved game {GameId} from Azure Blob Storage", id.Value);
            return game;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving game {GameId} from Azure Blob Storage", id.Value);
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetByPlayerAsync(PlayerAlias playerAlias)
    {
        try
        {
            var games = new List<SudokuGame>();
            var prefix = $"{playerAlias.Value}/";

            await foreach (var blobItem in _containerClient.GetBlobsAsync(prefix: prefix))
            {
                var game = await LoadGameFromBlobAsync(blobItem.Name);
                if (game != null)
                {
                    games.Add(game);
                }
            }

            _logger.LogDebug("Retrieved {Count} games for player {PlayerAlias}", games.Count, playerAlias.Value);
            return games.OrderByDescending(g => g.CreatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving games for player {PlayerAlias}", playerAlias.Value);
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
                var blobClient = _containerClient.GetBlobClient(blobName);

                var json = JsonSerializer.Serialize(game, _jsonOptions);
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

                await blobClient.UploadAsync(stream, overwrite: true);

                _logger.LogDebug("Saved game {GameId} to Azure Blob Storage", game.Id.Value);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving game {GameId} to Azure Blob Storage", game.Id.Value);
            throw;
        }
    }

    public async Task DeleteAsync(GameId id)
    {
        try
        {
            var blobName = GetBlobName(id);
            var blobClient = _containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteAsync();
                _logger.LogDebug("Deleted game {GameId} from Azure Blob Storage", id.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting game {GameId} from Azure Blob Storage", id.Value);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(GameId id)
    {
        try
        {
            var blobName = GetBlobName(id);
            var blobClient = _containerClient.GetBlobClient(blobName);
            return await blobClient.ExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of game {GameId}", id.Value);
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
            _logger.LogDebug("Retrieved {Count} games using specification", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving games using specification");
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
            _logger.LogError(ex, "Error counting games using specification");
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
            _logger.LogError(ex, "Error getting total games count for player {PlayerAlias}", playerAlias?.Value);
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
                .Where(g => g.CompletedAt.HasValue)
                .ToList();

            if (!gamesWithCompletionTime.Any())
            {
                return TimeSpan.Zero;
            }

            var totalTime = gamesWithCompletionTime
                .Sum(g => (g.CompletedAt!.Value - g.CreatedAt).TotalMilliseconds);

            var averageMilliseconds = totalTime / gamesWithCompletionTime.Count;
            return TimeSpan.FromMilliseconds(averageMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating average completion time for player {PlayerAlias}", playerAlias?.Value);
            throw;
        }
    }

    private async Task<List<SudokuGame>> GetAllGamesAsync()
    {
        var games = new List<SudokuGame>();

        await foreach (var blobItem in _containerClient.GetBlobsAsync())
        {
            var game = await LoadGameFromBlobAsync(blobItem.Name);
            if (game != null)
            {
                games.Add(game);
            }
        }

        return games;
    }

    private async Task<SudokuGame?> LoadGameFromBlobAsync(string blobName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var response = await blobClient.DownloadAsync();
            using var streamReader = new StreamReader(response.Value.Content);
            var json = await streamReader.ReadToEndAsync();

            return JsonSerializer.Deserialize<SudokuGame>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error loading game from blob {BlobName}", blobName);
            return null;
        }
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