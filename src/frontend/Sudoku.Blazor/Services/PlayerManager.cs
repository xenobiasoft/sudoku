using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.HttpClients;
using ILocalStorageService = Sudoku.Blazor.Services.Abstractions.ILocalStorageService;

namespace Sudoku.Blazor.Services;

using IPlayerManager = Abstractions.IPlayerManager;

public class PlayerManager(IPlayerApiClient playerApiClient, ILocalStorageService localStorageService) : IPlayerManager
{
    public async Task<string?> GetCurrentPlayerAsync()
    {
        var profile = await localStorageService.GetProfileAsync();
        if (profile != null) return profile.Alias;
        return await localStorageService.GetAliasAsync();
    }

    public async Task<string?> GetCurrentProfileIdAsync()
    {
        var profile = await localStorageService.GetProfileAsync();
        return profile?.ProfileId;
    }

    public async Task SetCurrentPlayerAsync(string alias)
    {
        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentException("Alias not set.");
        }

        await localStorageService.SetAliasAsync(alias);
    }

    public async Task<string> TryGetPlayerAlias()
    {
        var profile = await localStorageService.GetProfileAsync();
        if (profile != null) return profile.Alias;

        var alias = await localStorageService.GetAliasAsync();

        return alias ?? string.Empty;
    }

    public async Task<ProfileInfo?> GetCurrentProfileAsync()
    {
        return await localStorageService.GetProfileAsync();
    }

    public async Task<ApiResult<ProfileInfo>> CreateProfileAsync(string alias)
    {
        var result = await playerApiClient.CreateProfileAsync(alias);
        if (result.IsSuccess && result.Value != null)
        {
            var profile = new ProfileInfo { ProfileId = result.Value.ProfileId, Alias = result.Value.Alias };
            await localStorageService.SetProfileAsync(profile);
            return ApiResult<ProfileInfo>.Success(profile);
        }
        return ApiResult<ProfileInfo>.Failure(result.Error ?? "Failed to create profile", result.StatusCode);
    }

    public async Task<DateTime?> GetProfileCreatedAtAsync()
    {
        var profile = await localStorageService.GetProfileAsync();
        if (profile == null) return null;
        var result = await playerApiClient.GetProfileAsync(profile.Alias);
        return result.IsSuccess ? result.Value?.CreatedAt : null;
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

    public async Task<bool> EnsureProfileInitializedAsync()
    {
        var profile = await localStorageService.GetProfileAsync();
        if (profile != null)
        {
            var getResult = await playerApiClient.GetProfileAsync(profile.Alias);

            if (!getResult.IsSuccess)
            {
                // Transient backend failure — throw so caller can route to an error page
                throw new InvalidOperationException($"Backend unavailable while verifying profile: {getResult.Error}");
            }

            if (getResult.Value != null) return true;

            // Profile exists in localStorage but 404 in backend (orphaned) — attempt re-create
            var recreateResult = await playerApiClient.CreateProfileAsync(profile.Alias);
            if (recreateResult.IsSuccess && recreateResult.Value != null)
            {
                await localStorageService.SetProfileAsync(new ProfileInfo
                {
                    ProfileId = recreateResult.Value.ProfileId,
                    Alias = recreateResult.Value.Alias
                });
                return true;
            }

            return false;
        }

        // Check for legacy alias and attempt silent migration
        var legacyAlias = await localStorageService.GetAliasAsync();
        if (!string.IsNullOrEmpty(legacyAlias))
        {
            var aliasName = legacyAlias.Trim();
            var canRetry = aliasName.Length < 50;

            var createResult = await playerApiClient.CreateProfileAsync(aliasName);
            if (createResult.IsSuccess && createResult.Value != null)
            {
                await localStorageService.SetProfileAsync(new ProfileInfo
                {
                    ProfileId = createResult.Value.ProfileId,
                    Alias = createResult.Value.Alias
                });
                await localStorageService.RemoveAliasAsync();
                return true;
            }

            if (createResult.StatusCode == 409 && canRetry)
            {
                var suffix = Random.Shared.Next(10, 100).ToString();
                var aliasWithSuffix = aliasName[..Math.Min(aliasName.Length, 48)] + suffix;
                var retryResult = await playerApiClient.CreateProfileAsync(aliasWithSuffix);
                if (retryResult.IsSuccess && retryResult.Value != null)
                {
                    await localStorageService.SetProfileAsync(new ProfileInfo
                    {
                        ProfileId = retryResult.Value.ProfileId,
                        Alias = retryResult.Value.Alias
                    });
                    await localStorageService.RemoveAliasAsync();
                    return true;
                }
            }

            return false;
        }

        return false;
    }
}
