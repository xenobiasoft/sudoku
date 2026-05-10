using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;
using Sudoku.Blazor.Services.HttpClients;

namespace Sudoku.Blazor.Services;

public class PlayerManager(IPlayerApiClient playerApiClient, ILocalStorageService localStorageService) : IPlayerManager
{
    public async Task<ApiResult<ProfileInfo>> CreateProfileAsync(string displayName)
    {
        var result = await playerApiClient.CreateProfileAsync(displayName);
        if (result is { IsSuccess: true, Value: not null })
        {
            var profile = new ProfileInfo { ProfileId = result.Value.ProfileId, Alias = result.Value.Alias };
            await localStorageService.SetProfileAsync(profile);
            return ApiResult<ProfileInfo>.Success(profile);
        }

        return ApiResult<ProfileInfo>.Failure(result.Error ?? "Failed to create profile", result.StatusCode);
    }

    public async Task<bool> EnsureProfileInitializedAsync()
    {
        var profile = await localStorageService.GetProfileAsync();

        if (profile != null)
        {
            var getResult = await playerApiClient.GetProfileAsync(profile.Alias);

            if (!getResult.IsSuccess)
            {
                throw new InvalidOperationException($"Backend unavailable while verifying profile: {getResult.Error}");
            }

            if (getResult.Value != null) return true;

            var recreateResult = await playerApiClient.CreateProfileAsync(profile.Alias);
            if (recreateResult is { IsSuccess: true, Value: not null })
            {
                await localStorageService.SetProfileAsync(new ProfileInfo
                {
                    ProfileId = recreateResult.Value.ProfileId,
                    Alias = recreateResult.Value.Alias
                });
                return true;
            }
        }

        return false;
    }

    public async Task<ProfileInfo?> GetCurrentProfileAsync()
    {
        return await localStorageService.GetProfileAsync();
    }

    public async Task<ApiResult<ProfileInfo>> UpdateAliasAsync(string newAlias)
    {
        var profile = await localStorageService.GetProfileAsync();
        if (profile == null) return ApiResult<ProfileInfo>.Failure("Profile not found", 404);

        var result = await playerApiClient.UpdateProfileAliasAsync(profile.Alias, newAlias);
        if (result.IsSuccess && result.Value != null)
        {
            profile.Alias = result.Value.Alias;
            await localStorageService.SetProfileAsync(profile);
            return ApiResult<ProfileInfo>.Success(profile);
        }
        
        return ApiResult<ProfileInfo>.Failure(result.Error ?? "Failed to update alias", result.StatusCode);
    }
}
