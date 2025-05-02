using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Pages;

public partial class Index
{
	[Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] private IGameStateManager GameStateManager { get; set; } = null!;

    private bool _showSavedGames;
    private bool _showDifficulty;
    private List<GameStateMemory> _savedGames = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_savedGames.Any())
        {
            _savedGames = await LocalStorage.LoadGameStatesAsync() ?? [];

            if (_savedGames.Any())
            {
                StateHasChanged();
            }
        }
    }

    private void LoadGame(string gameId)
    {
        NavigationManager.NavigateTo($"/game/{gameId}");
    }

    private async Task DeleteGameAsync(string gameId)
    {
        await LocalStorage.RemoveGameAsync(gameId);
        await GameStateManager.DeleteAsync(gameId);
        StateHasChanged();
    }

    private void StartNewGame(string difficulty)
    {
        NavigationManager.NavigateTo($"/new?difficulty={difficulty}");
    }

    private void ToggleDisplaySavedGames()
    {
        _showSavedGames = !_showSavedGames;
    }

    private void ToggleDifficultyOptions()
    {
        _showDifficulty = !_showDifficulty;
    }
}