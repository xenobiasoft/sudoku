using Sudoku.Blazor.Models;

namespace Sudoku.Blazor.Services.Abstractions;

public interface IPlayerManager
{
    Task<ApiResult<ProfileInfo>> CreateProfileAsync(string displayName);
    Task<bool> EnsureProfileInitializedAsync();
    Task<ProfileInfo?> GetCurrentProfileAsync();
    Task<ApiResult<ProfileInfo>> UpdateAliasAsync(string newAlias);
}
