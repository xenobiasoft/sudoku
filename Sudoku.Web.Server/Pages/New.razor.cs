using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Pages;

public partial class New
{
    private readonly string[] Digits = ["1", "2", "3", "4", "5", "6", "7", "8", "9"];

    [Parameter] public string Difficulty { get; set; } = "Easy";
    [Inject] private NavigationManager? Navigation { get; set; }
    [Inject] private ISudokuGame? SudokuGame { get; set; }
    [Inject] private IGameStateManager? GameStateManager { get; set; }
    [Inject] private ILocalStorageService? LocalStorageService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var gameState = await SudokuGame!.NewGameAsync(Difficulty.ParseLevel());

        await GameStateManager!.SaveAsync(gameState);
        await LocalStorageService!.SaveGameStateAsync(gameState);

        Navigation!.NavigateTo($"/game/{gameState.PuzzleId}");
    }
}