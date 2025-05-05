using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Pages;

public partial class Index
{
	[Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IGameStateManager? GameStorageManager { get; set; } = null!;

    private bool _showSavedGames;
    private bool _showDifficulty;
    private IEnumerable<GameStateMemory>? _savedGames;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_savedGames == null)
        {
            await LoadGamesAsync();
            StateHasChanged();
        }
    }

    private void LoadGame(string gameId)
    {
        NavigationManager.NavigateTo($"/game/{gameId}");
    }

    private async Task LoadGamesAsync()
    {
        _savedGames = await GameStorageManager!.LoadGamesAsync() ?? [];
    }

    private async Task DeleteGameAsync(string gameId)
    {
        await GameStorageManager!.DeleteGameAsync(gameId);
        _savedGames = _savedGames.Where(x => x.PuzzleId != gameId).ToList();
        StateHasChanged();
    }

    private void StartNewGame(string difficulty)
    {
        NavigationManager.NavigateTo($"/new/{difficulty}");
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