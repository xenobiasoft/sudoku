using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Pages;

public partial class New
{
    private readonly string[] Digits = ["1", "2", "3", "4", "5", "6", "7", "8", "9"];

    [Parameter] public string Difficulty { get; set; } = "Easy";
    [Inject] public required NavigationManager Navigation { get; set; }
    [Inject] public required IGameManager GameManager { get; set; }
    [Inject] public required IPlayerManager PlayerManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await StartNewGameAsync();
    }

    private async Task StartNewGameAsync()
    {
        var alias = await PlayerManager.GetCurrentPlayerAsync();
        var gameState = await GameManager.CreateGameAsync(alias, Difficulty);
        Navigation.NavigateTo($"/game/{gameState.Id}");
    }
}