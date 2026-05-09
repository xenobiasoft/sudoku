using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;
using Sudoku.Blazor.Services.HttpClients;

namespace Sudoku.Blazor.Components.Pages;

public partial class Profile
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IPlayerManager PlayerManager { get; set; }
    [Inject] public required IPlayerApiClient PlayerApiClient { get; set; }
    [Inject] public required ILocalStorageService LocalStorageService { get; set; }
    [Inject] public required ILogger<Profile> Logger { get; set; }

    private string? _alias;
    private DateTime? _createdAt;
    private bool _isLoading = true;
    private bool _isEditing;
    private bool _isSaving;
    private string? _editError;
    private EditAliasModel _editModel = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        try
        {
            var profile = await PlayerManager.GetCurrentProfileAsync();
            if (profile == null)
            {
                NavigationManager.NavigateTo("/create-profile");
                return;
            }

            _alias = profile.Alias;

            var getResult = await PlayerApiClient.GetProfileAsync(profile.Alias);
            if (getResult.IsSuccess && getResult.Value != null)
            {
                _createdAt = getResult.Value.CreatedAt;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading profile");
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private void GoBack() => NavigationManager.NavigateTo("/");

    private void StartEdit()
    {
        _editModel = new EditAliasModel { NewAlias = _alias ?? string.Empty };
        _editError = null;
        _isEditing = true;
    }

    private void CancelEdit()
    {
        _isEditing = false;
        _editError = null;
    }

    private async Task HandleSaveAliasAsync()
    {
        _editError = null;
        if (_alias == null) return;

        var aliasName = _editModel.NewAlias.Trim();
        _isSaving = true;

        try
        {
            var result = await PlayerApiClient.UpdateProfileAliasAsync(_alias, aliasName);

            if (result.IsSuccess && result.Value != null)
            {
                var storedProfile = await LocalStorageService.GetProfileAsync();
                if (storedProfile != null)
                {
                    storedProfile.Alias = result.Value.Alias;
                    await LocalStorageService.SetProfileAsync(storedProfile);
                }

                _alias = result.Value.Alias;
                _isEditing = false;
                StateHasChanged();
                return;
            }

            if (result.StatusCode == 409)
            {
                _editError = "That alias is already taken. Please choose a different one.";
                return;
            }

            if (result.StatusCode == 404)
            {
                _editError = "Profile not found. Please refresh the page.";
                return;
            }

            _editError = "Something went wrong. Please try again.";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating profile alias");
            _editError = "Failed to update alias. Please check your connection.";
        }
        finally
        {
            _isSaving = false;
        }
    }

    private class EditAliasModel
    {
        [Required(ErrorMessage = "Alias is required.")]
        [MinLength(2, ErrorMessage = "Alias must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Alias cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Alias can only contain letters, numbers, and spaces.")]
        public string NewAlias { get; set; } = string.Empty;
    }
}
