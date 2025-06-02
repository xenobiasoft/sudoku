using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Pages;

public partial class Index
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IGameStateManager? GameStateManager { get; set; }
    [Inject] private IAliasService AliasService { get; set; } = null!;
    private string Alias { set; get; } = string.Empty;

    private bool _showSavedGames;
    private bool _showDifficulty;
    private IEnumerable<GameStateMemory>? _savedGames;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Alias = await AliasService.GetAliasAsync();

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
        _savedGames = await GameStateManager!.LoadGamesAsync() ?? [];
    }

    private async Task DeleteGameAsync(string gameId)
    {
        await GameStateManager!.DeleteGameAsync(Alias, gameId);
        _savedGames = _savedGames!.Where(x => x.PuzzleId != gameId).ToList();
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