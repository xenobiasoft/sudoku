using Microsoft.AspNetCore.Components;
using Sudoku.Blazor.Services.Abstractions;

namespace Sudoku.Blazor.Components.Pages;

public partial class SelectDifficulty
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required ILocalStorageService LocalStorageService { get; set; }
    [Inject] public required ILogger<SelectDifficulty> Logger { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        var profile = await LocalStorageService.GetProfileAsync();
        if (profile == null)
        {
            Logger.LogInformation("No profile found on SelectDifficulty page, redirecting to home");
            NavigationManager.NavigateTo("/");
        }
    }

    private void StartGame(string difficulty)
    {
        Logger.LogInformation("Starting new game with difficulty: {Difficulty}", difficulty);
        NavigationManager.NavigateTo($"/new/{difficulty}");
    }

    private void GoBack()
    {
        NavigationManager.NavigateTo("/");
    }
}
