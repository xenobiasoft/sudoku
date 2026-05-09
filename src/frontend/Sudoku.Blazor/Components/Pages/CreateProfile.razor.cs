using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;
using Sudoku.Blazor.Services.HttpClients;

namespace Sudoku.Blazor.Components.Pages;

public partial class CreateProfile
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IPlayerApiClient PlayerApiClient { get; set; }
    [Inject] public required ILocalStorageService LocalStorageService { get; set; }
    [Inject] public required ILogger<CreateProfile> Logger { get; set; }

    private CreateProfileModel _model = new();
    private string? _errorMessage;
    private bool _hasError;
    private bool _isSubmitting;

    private async Task HandleSubmitAsync()
    {
        _errorMessage = null;
        _hasError = false;

        var trimmedAlias = _model.Alias.Trim();
        _isSubmitting = true;

        try
        {
            var result = await PlayerApiClient.CreateProfileAsync(trimmedAlias);

            if (result.IsSuccess && result.Value != null)
            {
                await LocalStorageService.SetProfileAsync(new ProfileInfo
                {
                    ProfileId = result.Value.ProfileId,
                    Alias = result.Value.Alias
                });
                NavigationManager.NavigateTo("/");
                return;
            }

            if (result.StatusCode == 409)
            {
                _errorMessage = "That alias is already taken. Please choose a different one.";
                _hasError = true;
                return;
            }

            _errorMessage = "Something went wrong. Please try again.";
            _hasError = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating profile");
            _errorMessage = "Failed to create profile. Please check your connection.";
            _hasError = true;
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private class CreateProfileModel
    {
        [Required(ErrorMessage = "Alias is required.")]
        [MinLength(2, ErrorMessage = "Alias must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Alias cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Alias can only contain letters, numbers, and spaces.")]
        public string Alias { get; set; } = string.Empty;
    }
}
