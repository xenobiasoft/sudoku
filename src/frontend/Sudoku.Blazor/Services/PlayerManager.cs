using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.HttpClients;
using ILocalStorageService = Sudoku.Blazor.Services.Abstractions.ILocalStorageService;

namespace Sudoku.Blazor.Services;

using IPlayerManager = Abstractions.IPlayerManager;

public class PlayerManager(IPlayerApiClient playerApiClient, ILocalStorageService localStorageService) : IPlayerManager
{
    public async Task<string> CreatePlayerAsync(string? alias = null)
    {
        var response = await playerApiClient.CreatePlayerAsync(alias);
        if (!response.IsSuccess || response.Value == null)
        {
            throw new Exception("Failed to create player.");
        }

        await localStorageService.SetAliasAsync(response.Value);

        return response.Value;
    }

    public async Task<bool> PlayerExistsAsync(string alias)
    {
        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentException("Alias not set.");
        }

        var response = await playerApiClient.PlayerExistsAsync(alias);
        if (!response.IsSuccess)
        {
            throw new Exception("Failed to check if player exists.");
        }

        return response.Value;
    }

    public async Task<string?> GetCurrentPlayerAsync()
    {
        var profile = await localStorageService.GetProfileAsync();
        if (profile != null) return profile.Alias;
        return await localStorageService.GetAliasAsync();
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

        if (string.IsNullOrEmpty(alias))
        {
            alias = await CreatePlayerAsync();
            await SetCurrentPlayerAsync(alias);
        }

        return alias;
    }

    public async Task<ProfileInfo?> GetCurrentProfileAsync()
    {
        return await localStorageService.GetProfileAsync();
    }

    public async Task<bool> EnsureProfileInitializedAsync()
    {
        var profile = await localStorageService.GetProfileAsync();
        if (profile != null)
        {
            var getResult = await playerApiClient.GetProfileAsync(profile.Alias);
            if (getResult.IsSuccess && getResult.Value != null) return true;

            if (getResult.IsSuccess && getResult.Value == null)
            {
                // Orphaned profile — attempt re-create
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
            }

            return false;
        }

        // Check for legacy alias and attempt silent migration
        var legacyAlias = await localStorageService.GetAliasAsync();
        if (!string.IsNullOrEmpty(legacyAlias))
        {
            var normalizedAlias = legacyAlias.Trim().ToLowerInvariant();
            var canRetry = normalizedAlias.Length < 50;

            var createResult = await playerApiClient.CreateProfileAsync(normalizedAlias);
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
                var suffix = new Random().Next(10, 100).ToString();
                var aliasWithSuffix = normalizedAlias[..Math.Min(normalizedAlias.Length, 48)] + suffix;
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
