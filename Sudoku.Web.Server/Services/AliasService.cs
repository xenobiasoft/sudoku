using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.HttpClients;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// API-based alias service that uses the Player API
/// </summary>
public class AliasService(
    IPlayerApiClient playerApiClient,
    ILocalStorageService localStorageService,
    ILogger<AliasService> logger)
    : IAliasService
{
    private readonly IPlayerApiClient _playerApiClient = playerApiClient ?? throw new ArgumentNullException(nameof(playerApiClient));
    private readonly ILocalStorageService _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
    private readonly ILogger<AliasService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<string> GetAliasAsync()
    {
        try
        {
            // First try to get alias from local storage
            var localAlias = await _localStorageService.GetAliasAsync();
            
            if (!string.IsNullOrEmpty(localAlias))
            {
                // Verify the alias exists on the server
                var existsResult = await _playerApiClient.PlayerExistsAsync(localAlias);
                if (existsResult is { IsSuccess: true, Value: true })
                {
                    _logger.LogInformation("Using existing alias from local storage: {Alias}", localAlias);
                    return localAlias;
                }
            }

            _logger.LogWarning("Local alias {Alias} does not exist on server, creating new one", localAlias);
            var createResult = await _playerApiClient.CreatePlayerAsync(localAlias);
            if (createResult.IsSuccess && !string.IsNullOrEmpty(createResult.Value))
            {
                var newAlias = createResult.Value;
                await _localStorageService.SetAliasAsync(newAlias);
                _logger.LogInformation("Created and stored new alias: {Alias}", newAlias);
                return newAlias;
            }

            _logger.LogError("Failed to create new alias: {Error}", createResult.Error);
            throw new InvalidOperationException($"Failed to create new alias: {createResult.Error}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting alias");
            throw;
        }
    }
}