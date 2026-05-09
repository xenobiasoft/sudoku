using Sudoku.Blazor.Models;

namespace Sudoku.Blazor.Services.Abstractions;

public interface IPlayerManager
{
    Task<string?> GetCurrentPlayerAsync();
    Task<string?> GetCurrentProfileIdAsync();
    Task SetCurrentPlayerAsync(string alias);
    Task<string> TryGetPlayerAlias();
    Task<ProfileInfo?> GetCurrentProfileAsync();
    Task<bool> EnsureProfileInitializedAsync();
}
