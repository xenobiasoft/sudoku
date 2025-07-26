using Sudoku.Application.Common;

namespace Sudoku.Application.Interfaces;

public interface IPlayerApplicationService
{
    Task<Result<string>> CreatePlayerAsync(string? alias = null);
    Task<Result<bool>> DeletePlayerAsync(string alias);
    Task<Result<bool>> PlayerExistsAsync(string alias);
}