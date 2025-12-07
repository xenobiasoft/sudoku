using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Pages;

public partial class Index
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IGameManager GameManager { get; set; }
    [Inject] public required IPlayerManager PlayerManager { get; set; }

    private string Alias { get; set; } = string.Empty;
    private bool _showSavedGames;
    private bool _showDifficulty;
    private IEnumerable<GameModel>? _savedGames;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        await InitializePageAsync();
    }

    private async Task InitializePageAsync()
    {
        Alias = await PlayerManager.GetCurrentPlayerAsync();
        await LoadGamesAsync();
        StateHasChanged();
    }

    private void LoadGame(string gameId)
    {
        NavigationManager.NavigateTo($"/game/{gameId}");
    }

    private async Task LoadGamesAsync()
    {
        _savedGames = await GameManager.LoadGamesAsync(Alias) ?? [];
    }

    private async Task DeleteGameAsync(string gameId)
    {
        await GameManager.DeleteGameAsync(Alias, gameId);
        _savedGames = _savedGames?.Where(x => x.Id != gameId).ToList();
        StateHasChanged();
    }

    private void StartNewGame(string difficulty)
    {
        NavigationManager.NavigateTo($"/new/{difficulty}");
    }

    private void ToggleDisplaySavedGames()
    {
        _showSavedGames = !_showSavedGames;
        StateHasChanged();
    }

    private void ToggleDifficultyOptions()
    {
        _showDifficulty = !_showDifficulty;
        StateHasChanged();
    }
}