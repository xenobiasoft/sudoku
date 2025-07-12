using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using Sudoku.Application.Specifications;

namespace Sudoku.Application.Interfaces;

public interface IGameRepository
{
    // Basic CRUD operations
    Task<SudokuGame?> GetByIdAsync(GameId id);
    Task<IEnumerable<SudokuGame>> GetByPlayerAsync(PlayerAlias playerAlias);
    Task<IEnumerable<SudokuGame>> GetByPlayerAndStatusAsync(PlayerAlias playerAlias, GameStatus status);
    Task SaveAsync(SudokuGame game);
    Task DeleteAsync(GameId id);
    Task<bool> ExistsAsync(GameId id);

    // Specification-based queries
    Task<IEnumerable<SudokuGame>> GetBySpecificationAsync(ISpecification<SudokuGame> specification);
    Task<SudokuGame?> GetSingleBySpecificationAsync(ISpecification<SudokuGame> specification);
    Task<int> CountBySpecificationAsync(ISpecification<SudokuGame> specification);

    // Additional query methods
    Task<IEnumerable<SudokuGame>> GetRecentGamesAsync(int count = 10);
    Task<IEnumerable<SudokuGame>> GetCompletedGamesAsync(PlayerAlias? playerAlias = null);
    Task<IEnumerable<SudokuGame>> GetGamesByDifficultyAsync(GameDifficulty difficulty);
    Task<IEnumerable<SudokuGame>> GetGamesByStatusAsync(GameStatus status);

    // Statistics and analytics
    Task<int> GetTotalGamesCountAsync(PlayerAlias? playerAlias = null);
    Task<int> GetCompletedGamesCountAsync(PlayerAlias? playerAlias = null);
    Task<TimeSpan> GetAverageCompletionTimeAsync(PlayerAlias? playerAlias = null);
}