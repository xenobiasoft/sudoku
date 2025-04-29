using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Pages;

public partial class Index
{
	[Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;

    private bool showSavedGames;
    private bool showDifficulty;
    private List<SavedGame> savedGames = [];

    private void ToggleDifficultyOptions()
    {
        showDifficulty = !showDifficulty;
    }

    private void StartNewGame(string difficulty)
    {
        NavigationManager.NavigateTo($"/game/new?difficulty={difficulty}");
    }

    private async Task LoadSavedGames()
    {
        savedGames = await LocalStorage.GetSavedGamesAsync();
        showSavedGames = true;
    }

    private void LoadGame(string gameId)
    {
        NavigationManager.NavigateTo($"/game/load/{gameId}");
    }
}