using Microsoft.AspNetCore.Components;
using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;

namespace Sudoku.Blazor.Components.Pages;

public partial class Index
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required ILocalStorageService LocalStorageService { get; set; }
    [Inject] public required ILogger<Index> Logger { get; set; }

    private bool _isReturningPlayer;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        Logger.LogInformation("Home page initializing");

        await MigrateProfileIfNeededAsync();

        var profile = await LocalStorageService.GetProfileAsync();
        _isReturningPlayer = profile != null;

        Logger.LogInformation("Home page initialized, returning player: {IsReturningPlayer}", _isReturningPlayer);

        StateHasChanged();
    }

    private async Task MigrateProfileIfNeededAsync()
    {
        try
        {
            var profile = await LocalStorageService.GetProfileAsync();
            if (profile != null) return;

            var legacyAlias = await LocalStorageService.GetAliasAsync();
            if (string.IsNullOrEmpty(legacyAlias)) return;

            await LocalStorageService.SetProfileAsync(new ProfileInfo
            {
                ProfileId = Guid.NewGuid().ToString(),
                Alias = legacyAlias.Trim()
            });
            await LocalStorageService.RemoveAliasAsync();

            Logger.LogInformation("Migrated legacy alias to profile");
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Profile migration failed silently, treating as new player");
        }
    }

    private void NavigateToProfile()
    {
        NavigationManager.NavigateTo(_isReturningPlayer ? "/profile" : "/create-profile");
    }

    private void NavigateToSelectDifficulty()
    {
        NavigationManager.NavigateTo("/select-difficulty");
    }

    private void NavigateToGameList()
    {
        NavigationManager.NavigateTo("/games");
    }
}
