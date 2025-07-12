using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Interfaces;

public interface IGameRepository
{
    Task<SudokuGame?> GetByIdAsync(GameId id);
    Task<IEnumerable<SudokuGame>> GetByPlayerAsync(PlayerAlias playerAlias);
    Task<IEnumerable<SudokuGame>> GetByPlayerAndStatusAsync(PlayerAlias playerAlias, GameStatus status);
    Task SaveAsync(SudokuGame game);
    Task DeleteAsync(GameId id);
    Task<bool> ExistsAsync(GameId id);
}