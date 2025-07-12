using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Specifications;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;

namespace XenobiaSoft.Sudoku.Infrastructure.Repositories;

public class InMemoryGameRepository : IGameRepository
{
    private readonly Dictionary<GameId, SudokuGame> _games = new();
    private readonly ILogger<InMemoryGameRepository> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public InMemoryGameRepository(ILogger<InMemoryGameRepository> logger)
    {
        _logger = logger;
    }

    public async Task<SudokuGame?> GetByIdAsync(GameId id)
    {
        try
        {
            await _semaphore.WaitAsync();
            try
            {
                _games.TryGetValue(id, out var game);
                _logger.LogDebug("Retrieved game {GameId} from in-memory storage", id.Value);
                return game;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving game {GameId} from in-memory storage", id.Value);
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetByPlayerAsync(PlayerAlias playerAlias)
    {
        try
        {
            await _semaphore.WaitAsync();
            try
            {
                var games = _games.Values
                    .Where(g => g.PlayerAlias == playerAlias)
                    .OrderByDescending(g => g.CreatedAt)
                    .ToList();

                _logger.LogDebug("Retrieved {Count} games for player {PlayerAlias} from in-memory storage",
                    games.Count, playerAlias.Value);
                return games;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving games for player {PlayerAlias} from in-memory storage", playerAlias.Value);
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
                _games[game.Id] = game;
                _logger.LogDebug("Saved game {GameId} to in-memory storage", game.Id.Value);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving game {GameId} to in-memory storage", game.Id.Value);
            throw;
        }
    }

    public async Task DeleteAsync(GameId id)
    {
        try
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_games.Remove(id))
                {
                    _logger.LogDebug("Deleted game {GameId} from in-memory storage", id.Value);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting game {GameId} from in-memory storage", id.Value);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(GameId id)
    {
        try
        {
            await _semaphore.WaitAsync();
            try
            {
                return _games.ContainsKey(id);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of game {GameId} in in-memory storage", id.Value);
            throw;
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetBySpecificationAsync(ISpecification<SudokuGame> specification)
    {
        try
        {
            await _semaphore.WaitAsync();
            try
            {
                var query = _games.Values.AsQueryable();

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
                _logger.LogDebug("Retrieved {Count} games using specification from in-memory storage", result.Count);
                return result;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving games using specification from in-memory storage");
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
            await _semaphore.WaitAsync();
            try
            {
                var query = _games.Values.AsQueryable();

                if (specification.Criteria != null)
                {
                    query = query.Where(specification.Criteria);
                }

                return query.Count();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting games using specification from in-memory storage");
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
            await _semaphore.WaitAsync();
            try
            {
                var query = _games.Values.AsQueryable();

                if (playerAlias != null)
                {
                    query = query.Where(g => g.PlayerAlias == playerAlias);
                }

                return query.Count();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total games count for player {PlayerAlias} from in-memory storage", playerAlias?.Value);
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
            _logger.LogError(ex, "Error calculating average completion time for player {PlayerAlias} from in-memory storage", playerAlias?.Value);
            throw;
        }
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}