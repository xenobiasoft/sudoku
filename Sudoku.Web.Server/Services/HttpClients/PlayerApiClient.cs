using System.Net.Http.Json;
using System.Text.Json;
using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.HttpClients;

/// <summary>
/// HTTP client for Player API operations
/// </summary>
public class PlayerApiClient : IPlayerApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PlayerApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public PlayerApiClient(HttpClient httpClient, ILogger<PlayerApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<ApiResult<string>> CreatePlayerAsync(string? alias = null)
    {
        try
        {
            _logger.LogInformation("Creating player with alias: {Alias}", alias ?? "auto-generated");
            
            var request = alias != null ? new CreatePlayerRequest(alias) : null;
            var response = await _httpClient.PostAsJsonAsync("api/players", request, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var createdAlias = await response.Content.ReadAsStringAsync();
                // Remove quotes if the response is a JSON string
                createdAlias = JsonSerializer.Deserialize<string>(createdAlias) ?? createdAlias.Trim('"');
                
                _logger.LogInformation("Player created successfully with alias: {Alias}", createdAlias);
                return ApiResult<string>.Success(createdAlias);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to create player. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<string>.Failure($"Failed to create player: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating player");
            return ApiResult<string>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<bool>> PlayerExistsAsync(string alias)
    {
        try
        {
            _logger.LogInformation("Checking if player exists: {Alias}", alias);
            
            var response = await _httpClient.GetAsync($"api/players/{Uri.EscapeDataString(alias)}/exists");

            if (response.IsSuccessStatusCode)
            {
                var exists = await response.Content.ReadFromJsonAsync<bool>(_jsonOptions);
                _logger.LogInformation("Player existence check completed. Alias: {Alias}, Exists: {Exists}", alias, exists);
                return ApiResult<bool>.Success(exists);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to check player existence. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return ApiResult<bool>.Failure($"Failed to check player existence: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while checking player existence");
            return ApiResult<bool>.Failure($"Exception occurred: {ex.Message}");
        }
    }
}