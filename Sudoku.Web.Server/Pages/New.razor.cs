using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Pages;

public partial class New
{
    private readonly string[] Digits = ["1", "2", "3", "4", "5", "6", "7", "8", "9"];

    [Parameter] public string Difficulty { get; set; } = "Easy";
    [Inject] private NavigationManager? Navigation { get; set; }
    [Inject] private ISudokuGame? SudokuGame { get; set; }
    [Inject] private IGameStateManager? GameStorageManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var gameState = await SudokuGame!.NewGameAsync(Difficulty.ParseLevel());

        await GameStorageManager!.SaveGameAsync(gameState);

        Navigation!.NavigateTo($"/game/{gameState.PuzzleId}");
    }
}