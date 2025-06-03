using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Pages;

public partial class New
{
    private readonly string[] Digits = ["1", "2", "3", "4", "5", "6", "7", "8", "9"];

    [Parameter] public string Difficulty { get; set; } = "Easy";
    [Inject] public required NavigationManager Navigation { get; set; }
    [Inject] public required ISudokuGame SudokuGame { get; set; }
    [Inject] public required IGameStateManager GameStorageManager { get; set; }
    [Inject] public required IAliasService AliasService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await StartNewGameAsync();
    }

    private async Task StartNewGameAsync()
    {
        var alias = await AliasService.GetAliasAsync();
        var gameState = await SudokuGame.NewGameAsync(alias, Difficulty.ParseLevel());
        await GameStorageManager.SaveGameAsync(gameState);
        Navigation.NavigateTo($"/game/{gameState.PuzzleId}");
    }
}