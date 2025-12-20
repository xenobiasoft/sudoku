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
    [Inject] public required ILogger<New> Logger { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GenerateNewGameAsync();
    }

    private async Task GenerateNewGameAsync()
    {
        var alias = await PlayerManager.GetCurrentPlayerAsync();

        Logger.LogInformation("Generating new game with difficulty {Difficulty} for player {PlayerAlias}", Difficulty, alias);

        var gameState = await GameManager.CreateGameAsync(alias, Difficulty);

        Logger.LogInformation("Successfully created game {GameId} for player {PlayerAlias}", gameState.Id, alias);

        Navigation.NavigateTo($"/game/{gameState.Id}");
    }
}