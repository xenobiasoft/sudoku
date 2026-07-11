using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Specifications;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.EventHandling;
using Sudoku.Infrastructure.Mappers;
using Sudoku.Infrastructure.Services;

namespace Sudoku.Infrastructure.Repositories;

public class CosmosDbGameRepository(
    ICosmosDbService cosmosDbService,
    IDomainEventDispatcher eventDispatcher,
    ILogger<CosmosDbGameRepository> logger) : IGameRepository
{
    public async Task<SudokuGame?> GetByIdAsync(GameId id)
    {
        try
        {
            var document = await cosmosDbService.GetItemAsync<Models.SudokuGameDocument>(id.Value.ToString(), id);

            if (document == null)
            {
                logger.LogDebug("Game with ID {GameId} not found in CosmosDB", id.Value);
                return null;
            }

            var game = SudokuGameMapper.ToDomain(document);
            logger.LogDebug("Retrieved game {GameId} from CosmosDB", id.Value);
            return game;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving game {GameId} from CosmosDB", id.Value);
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetByProfileIdAsync(ProfileId profileId)
    {
        try
        {
            var sqlQuery = "SELECT * FROM c WHERE c.profileId = @profileId ORDER BY c.createdAt DESC";
            var queryParams = new Dictionary<string, string>
            {
                { "@profileId", profileId.Value.ToString() }
            };

            var documents = await cosmosDbService.QueryItemsAsync<Models.SudokuGameDocument>(sqlQuery, queryParams);
            var games = documents.Select(SudokuGameMapper.ToDomain).ToList();

            logger.LogDebug("Retrieved {Count} games for profile {ProfileId} from CosmosDB", games.Count, profileId.Value);
            return games;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving games for profile {ProfileId} from CosmosDB", profileId.Value);
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetByProfileIdAndStatusAsync(ProfileId profileId, GameStatusEnum statusEnum)
    {
        try
        {
            var sqlQuery = "SELECT * FROM c WHERE c.profileId = @profileId AND c.status = @status ORDER BY c.createdAt DESC";
            var queryParams = new Dictionary<string, string>
            {
                { "@profileId", profileId.Value.ToString() },
                { "@status", statusEnum.ToString() }
            };

            var documents = await cosmosDbService.QueryItemsAsync<Models.SudokuGameDocument>(sqlQuery, queryParams);
            var games = documents.Select(SudokuGameMapper.ToDomain).ToList();

            logger.LogDebug("Retrieved {Count} games for profile {ProfileId} with status {Status} from CosmosDB",
                games.Count, profileId.Value, statusEnum);
            return games;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving games for profile {ProfileId} with status {Status} from CosmosDB",
                profileId.Value, statusEnum);
            throw;
        }
    }

    public async Task SaveAsync(SudokuGame game)
    {
        try
        {
            var document = SudokuGameMapper.ToDocument(game);

            await cosmosDbService.UpsertItemAsync(document, game.Id);

            logger.LogDebug("Saved game {GameId} to CosmosDB", game.Id.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving game {GameId} to CosmosDB", game.Id.Value);
            throw;
        }

        await DispatchDomainEventsAsync(game);
    }

    private async Task DispatchDomainEventsAsync(SudokuGame game)
    {
        var domainEvents = game.DomainEvents.ToList();

        // Clear before dispatching: a handler fault must not leave events queued
        // for re-dispatch on the game's next save.
        game.ClearDomainEvents();

        try
        {
            await eventDispatcher.DispatchAsync(domainEvents);
        }
        catch (Exception ex)
        {
            // The game is already persisted, so a handler fault must not fail the caller.
            logger.LogError(ex, "Error dispatching domain events for game {GameId}", game.Id.Value);
        }
    }

    public async Task DeleteAsync(GameId id)
    {
        try
        {
            await cosmosDbService.DeleteItemAsync<Models.SudokuGameDocument>(id.Value.ToString(), id);

            logger.LogDebug("Deleted game {GameId} from CosmosDB", id.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting game {GameId} from CosmosDB", id.Value);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(GameId id)
    {
        try
        {
            var exists = await cosmosDbService.ExistsAsync<Models.SudokuGameDocument>(id.Value.ToString(), id);

            logger.LogDebug("Game {GameId} exists in CosmosDB: {Exists}", id.Value, exists);
            return exists;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking existence of game {GameId} in CosmosDB", id.Value);
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetBySpecificationAsync(ISpecification<SudokuGame> specification)
    {
        try
        {
            // For now, we'll get all games and apply the specification in memory
            // In a production scenario, you'd want to convert the specification to SQL
            var allDocuments = await cosmosDbService.QueryItemsAsync<Models.SudokuGameDocument>("SELECT * FROM c");
            var allGames = allDocuments.Select(SudokuGameMapper.ToDomain);

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
            logger.LogDebug("Retrieved {Count} games using specification from CosmosDB", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving games using specification from CosmosDB");
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
            var games = await GetBySpecificationAsync(specification);
            return games.Count();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting games using specification in CosmosDB");
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetRecentGamesAsync(int count = 10)
    {
        try
        {
            var sqlQuery = "SELECT * FROM c ORDER BY c.createdAt DESC OFFSET 0 LIMIT @count";
            var queryParams = new Dictionary<string, string>
            {
                { "@count", count.ToString() }
            };

            var documents = await cosmosDbService.QueryItemsAsync<Models.SudokuGameDocument>(sqlQuery, queryParams);
            var games = documents.Select(SudokuGameMapper.ToDomain).ToList();

            logger.LogDebug("Retrieved {Count} recent games from CosmosDB", games.Count);
            return games;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving recent games from CosmosDB");
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetCompletedGamesAsync(ProfileId? profileId = null)
    {
        try
        {
            string sqlQuery;
            Dictionary<string, string> queryParams;

            if (profileId != null)
            {
                sqlQuery = "SELECT * FROM c WHERE c.status = @status AND c.profileId = @profileId ORDER BY c.completedAt DESC";
                queryParams = new Dictionary<string, string>
                {
                    { "@status", GameStatusEnum.Completed.ToString() },
                    { "@profileId", profileId.Value.ToString() }
                };
            }
            else
            {
                sqlQuery = "SELECT * FROM c WHERE c.status = @status ORDER BY c.completedAt DESC";
                queryParams = new Dictionary<string, string>
                {
                    { "@status", GameStatusEnum.Completed.ToString() }
                };
            }

            var documents = await cosmosDbService.QueryItemsAsync<Models.SudokuGameDocument>(sqlQuery, queryParams);
            var games = documents.Select(SudokuGameMapper.ToDomain).ToList();

            logger.LogDebug("Retrieved {Count} completed games from CosmosDB", games.Count);
            return games;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving completed games from CosmosDB");
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetGamesByDifficultyAsync(GameDifficulty difficulty)
    {
        try
        {
            var sqlQuery = "SELECT * FROM c WHERE c.difficulty = @difficulty ORDER BY c.createdAt DESC";
            var queryParams = new Dictionary<string, string>
            {
                { "@difficulty", difficulty.ToString() }
            };

            var documents = await cosmosDbService.QueryItemsAsync<Models.SudokuGameDocument>(sqlQuery, queryParams);
            var games = documents.Select(SudokuGameMapper.ToDomain).ToList();

            logger.LogDebug("Retrieved {Count} games with difficulty {Difficulty} from CosmosDB", games.Count, difficulty);
            return games;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving games by difficulty {Difficulty} from CosmosDB", difficulty);
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetGamesByStatusAsync(GameStatusEnum statusEnum)
    {
        try
        {
            var sqlQuery = "SELECT * FROM c WHERE c.status = @status ORDER BY c.createdAt DESC";
            var queryParams = new Dictionary<string, string>
            {
                { "@status", statusEnum.ToString() }
            };

            var documents = await cosmosDbService.QueryItemsAsync<Models.SudokuGameDocument>(sqlQuery, queryParams);
            var games = documents.Select(SudokuGameMapper.ToDomain).ToList();

            logger.LogDebug("Retrieved {Count} games with status {Status} from CosmosDB", games.Count, statusEnum);
            return games;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving games by status {Status} from CosmosDB", statusEnum);
            throw;
        }
    }

    public async Task<int> GetTotalGamesCountAsync(ProfileId? profileId = null)
    {
        try
        {
            string sqlQuery;
            Dictionary<string, string> queryParams = new();

            if (profileId != null)
            {
                sqlQuery = "SELECT VALUE COUNT(1) FROM c WHERE c.profileId = @profileId";
                queryParams = new Dictionary<string, string>
                {
                    { "@profileId", profileId.Value.ToString() }
                };
            }
            else
            {
                sqlQuery = "SELECT VALUE COUNT(1) FROM c";
            }

            var results = await cosmosDbService.QueryItemsAsync<int>(sqlQuery, queryParams);
            var count = results.FirstOrDefault();

            logger.LogDebug("Total games count: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting total games count from CosmosDB");
            throw;
        }
    }

    public async Task<int> GetCompletedGamesCountAsync(ProfileId? profileId = null)
    {
        try
        {
            string sqlQuery;
            Dictionary<string, string> queryParams;

            if (profileId != null)
            {
                sqlQuery = "SELECT VALUE COUNT(1) FROM c WHERE c.status = @status AND c.profileId = @profileId";
                queryParams = new Dictionary<string, string>
                {
                    { "@status", GameStatusEnum.Completed.ToString() },
                    { "@profileId", profileId.Value.ToString() }
                };
            }
            else
            {
                sqlQuery = "SELECT VALUE COUNT(1) FROM c WHERE c.status = @status";
                queryParams = new Dictionary<string, string>()
                {
                    { "@status", GameStatusEnum.Completed.ToString() }
                };
            }

            var results = await cosmosDbService.QueryItemsAsync<int>(sqlQuery, queryParams);
            var count = results.FirstOrDefault();

            logger.LogDebug("Completed games count: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting completed games count from CosmosDB");
            throw;
        }
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

            var averageTime = TimeSpan.FromMilliseconds(averageMilliseconds);
            logger.LogDebug("Average completion time: {AverageTime}", averageTime);
            return averageTime;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating average completion time from CosmosDB");
            throw;
        }
    }
}
