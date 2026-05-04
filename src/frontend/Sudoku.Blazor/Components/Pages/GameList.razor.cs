using Microsoft.AspNetCore.Components;
using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;

namespace Sudoku.Blazor.Components.Pages;

public partial class GameList
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required ILocalStorageService LocalStorageService { get; set; }
    [Inject] public required IGameManager GameManager { get; set; }
    [Inject] public required ILogger<GameList> Logger { get; set; }

    private string _alias = string.Empty;
    private List<GameModel>? _games;
    private bool _isLoading;
    private bool _hasError;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        var profile = await LocalStorageService.GetProfileAsync();
        if (profile == null)
        {
            Logger.LogInformation("No profile found on GameList page, redirecting to home");
            NavigationManager.NavigateTo("/");
            return;
        }

        _alias = profile.Alias;
        await LoadGamesAsync();
        StateHasChanged();
    }

    private async Task LoadGamesAsync()
    {
        _isLoading = true;
        _hasError = false;

        try
        {
            _games = await GameManager.LoadGamesAsync(_alias) ?? [];
            Logger.LogInformation("Loaded {Count} saved games for player {Alias}", _games.Count, _alias);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading games for player {Alias}", _alias);
            _hasError = true;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void LoadGame(string gameId)
    {
        Logger.LogInformation("Loading game {GameId}", gameId);
        NavigationManager.NavigateTo($"/game/{gameId}");
    }

    private async Task DeleteGameAsync(string gameId)
    {
        Logger.LogInformation("Deleting game {GameId} for player {Alias}", gameId, _alias);
        await GameManager.DeleteGameAsync(_alias, gameId);
        _games = _games?.Where(g => g.Id != gameId).ToList();
        Logger.LogInformation("Successfully deleted game {GameId}", gameId);
        StateHasChanged();
    }

    private void GoBack()
    {
        NavigationManager.NavigateTo("/");
    }
}
