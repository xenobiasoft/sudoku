using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Pages;

public partial class Index
{
	[Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;

    private bool _showSavedGames;
    private bool _showDifficulty;
    private List<SavedGame> _savedGames = [];

    private void ToggleDifficultyOptions()
    {
        _showDifficulty = !_showDifficulty;
    }

    private void StartNewGame(string difficulty)
    {
        NavigationManager.NavigateTo($"/new?difficulty={difficulty}");
    }

    private async Task LoadSavedGames()
    {
        _savedGames = await LocalStorage.GetSavedGamesAsync();
        _showSavedGames = true;
    }

    private void LoadGame(string gameId)
    {
        NavigationManager.NavigateTo($"/game/{gameId}");
    }
}