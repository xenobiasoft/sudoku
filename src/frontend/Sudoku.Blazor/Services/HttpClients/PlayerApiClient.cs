using System.Net;
using System.Text.Json;
using Sudoku.Blazor.Models;

namespace Sudoku.Blazor.Services.HttpClients;

public class PlayerApiClient(HttpClient httpClient, ILogger<PlayerApiClient> logger) : IPlayerApiClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<PlayerApiClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task<ApiResult<ProfileDto>> CreateProfileAsync(string alias)
    {
        try
        {
            _logger.LogInformation("Creating profile for alias: {Alias}", alias);

            var response = await _httpClient.PostAsJsonAsync("api/profiles", new { alias }, _jsonOptions);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var dto = await response.Content.ReadFromJsonAsync<ProfileDto>(_jsonOptions);
                _logger.LogInformation("Profile created for alias: {Alias}", alias);
                return ApiResult<ProfileDto>.Success(dto!);
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to create profile. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
            return new ApiResult<ProfileDto>
            {
                IsSuccess = false,
                Error = error,
                StatusCode = (int)response.StatusCode
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating profile");
            return ApiResult<ProfileDto>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<ProfileDto?>> GetProfileAsync(string alias)
    {
        try
        {
            _logger.LogInformation("Getting profile for alias: {Alias}", alias);

            var response = await _httpClient.GetAsync($"api/profiles/{Uri.EscapeDataString(alias)}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return ApiResult<ProfileDto?>.Success(null);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<ProfileDto>(_jsonOptions);
                return ApiResult<ProfileDto?>.Success(dto);
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to get profile. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
            return ApiResult<ProfileDto?>.Failure($"Failed to get profile: {error}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting profile");
            return ApiResult<ProfileDto?>.Failure($"Exception occurred: {ex.Message}");
        }
    }

    public async Task<ApiResult<ProfileDto>> UpdateProfileAliasAsync(string alias, string newAlias)
    {
        try
        {
            _logger.LogInformation("Updating profile alias from {Alias} to {NewAlias}", alias, newAlias);

            var response = await _httpClient.PatchAsJsonAsync(
                $"api/profiles/{Uri.EscapeDataString(alias)}",
                new { newAlias },
                _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<ProfileDto>(_jsonOptions);
                return ApiResult<ProfileDto>.Success(dto!);
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to update profile alias. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
            return new ApiResult<ProfileDto>
            {
                IsSuccess = false,
                Error = error,
                StatusCode = (int)response.StatusCode
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while updating profile alias");
            return ApiResult<ProfileDto>.Failure($"Exception occurred: {ex.Message}");
        }
    }
}
