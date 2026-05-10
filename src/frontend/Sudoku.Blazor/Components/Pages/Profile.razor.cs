using Microsoft.AspNetCore.Components;
using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;

namespace Sudoku.Blazor.Components.Pages;

public partial class Profile
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IPlayerManager PlayerManager { get; set; }
    [Inject] public required ILogger<EditProfileModel> Logger { get; set; }

    private string? _alias;
    private DateTime? _createdAt;
    private bool _isLoading = true;
    private bool _isEditing;
    private bool _isSaving;
    private string? _editError;
    private EditProfileModel _editModel = new();

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
            _createdAt = await PlayerManager.GetProfileCreatedAtAsync();
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
        _editModel = new EditProfileModel { DisplayName = _alias ?? string.Empty };
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
        _isSaving = true;

        try
        {
            var result = await PlayerManager.UpdateAliasAsync(_editModel.DisplayName.Trim());

            if (result.IsSuccess)
            {
                _alias = result.Value!.Alias;
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
}
