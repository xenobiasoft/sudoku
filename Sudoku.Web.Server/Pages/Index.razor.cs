using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Pages;

public partial class Index
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IGameManager GameManager { get; set; }
    [Inject] public required IPlayerManager PlayerManager { get; set; }
    [Inject] public required ILogger<Index> Logger { get; set; }

    private string Alias { get; set; } = string.Empty;
    private bool _showSavedGames;
    private bool _showDifficulty;
    private IEnumerable<GameModel>? _savedGames;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        Logger.LogInformation("Index page first render completed, initializing page");

        await InitializePageAsync();
    }

    private async Task InitializePageAsync()
    {
        try
        {
            Alias = await PlayerManager.GetCurrentPlayerAsync();
            Logger.LogInformation("Current player retrieved: {Alias}", Alias);
            
            await LoadGamesAsync();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing Index page");
            NavigationManager.NavigateTo("/error/500");
        }
    }

    private void LoadGame(string gameId)
    {
        Logger.LogInformation("Loading game with ID: {GameId}", gameId);
        NavigationManager.NavigateTo($"/game/{gameId}");
    }

    private async Task LoadGamesAsync()
    {
        _savedGames = await GameManager.LoadGamesAsync(Alias) ?? [];
        Logger.LogInformation("Loaded {GameCount} saved games for player {Alias}", _savedGames.Count(), Alias);
    }

    private async Task DeleteGameAsync(string gameId)
    {
        Logger.LogInformation("Deleting game {GameId} for player {Alias}", gameId, Alias);
        await GameManager.DeleteGameAsync(Alias, gameId);
        _savedGames = _savedGames?.Where(x => x.Id != gameId).ToList();
        Logger.LogInformation("Successfully deleted game {GameId}", gameId);
        StateHasChanged();
    }

    private void StartNewGame(string difficulty)
    {
        Logger.LogInformation("Starting new game with difficulty: {Difficulty}", difficulty);
        NavigationManager.NavigateTo($"/new/{difficulty}");
    }

    private void ToggleDisplaySavedGames()
    {
        _showSavedGames = !_showSavedGames;
        Logger.LogDebug("Toggled saved games display to: {ShowSavedGames}", _showSavedGames);
        StateHasChanged();
    }

    private void ToggleDifficultyOptions()
    {
        _showDifficulty = !_showDifficulty;
        Logger.LogDebug("Toggled difficulty options display to: {ShowDifficulty}", _showDifficulty);
        StateHasChanged();
    }
}