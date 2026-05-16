using Microsoft.AspNetCore.Components;
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

        var profile = await LocalStorageService.GetProfileAsync();
        _isReturningPlayer = profile != null;

        Logger.LogInformation("Home page initialized, returning player: {IsReturningPlayer}", _isReturningPlayer);

        StateHasChanged();
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
