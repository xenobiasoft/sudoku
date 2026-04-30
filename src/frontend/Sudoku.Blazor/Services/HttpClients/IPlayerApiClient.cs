using Sudoku.Blazor.Models;

namespace Sudoku.Blazor.Services.HttpClients;

public interface IPlayerApiClient
{
    Task<ApiResult<string>> CreatePlayerAsync(string? alias = null);
    Task<ApiResult<bool>> PlayerExistsAsync(string alias);
    Task<ApiResult<ProfileDto>> CreateProfileAsync(string alias);
    Task<ApiResult<ProfileDto?>> GetProfileAsync(string alias);
    Task<ApiResult<ProfileDto>> UpdateProfileAliasAsync(string alias, string newAlias);
}
