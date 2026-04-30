using Sudoku.Blazor.Models;

namespace Sudoku.Blazor.Services.Abstractions;

public interface IPlayerManager
{
    Task<string> CreatePlayerAsync(string? alias = null);
    Task<bool> PlayerExistsAsync(string alias);
    Task<string?> GetCurrentPlayerAsync();
    Task SetCurrentPlayerAsync(string alias);
    Task<string> TryGetPlayerAlias();
    Task<ProfileInfo?> GetCurrentProfileAsync();
    Task<bool> EnsureProfileInitializedAsync();
}
