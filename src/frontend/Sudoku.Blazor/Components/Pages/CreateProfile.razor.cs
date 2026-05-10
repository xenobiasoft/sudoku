using Microsoft.AspNetCore.Components;
using Sudoku.Blazor.Services.Abstractions;

namespace Sudoku.Blazor.Components.Pages;

public partial class CreateProfile
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IPlayerManager PlayerManager { get; set; }
    [Inject] public required ILogger<CreateProfile> Logger { get; set; }

    private Models.CreateProfileModel _model = new();
    private string? _errorMessage;
    private bool _hasError;
    private bool _isSubmitting;

    private async Task HandleSubmitAsync()
    {
        _errorMessage = null;
        _hasError = false;
        _isSubmitting = true;

        try
        {
            var result = await PlayerManager.CreateProfileAsync(_model.Alias.Trim());

            if (result.IsSuccess)
            {
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
}
