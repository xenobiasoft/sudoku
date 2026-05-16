using Microsoft.AspNetCore.Components;
using Sudoku.Blazor.Services.Abstractions;

namespace Sudoku.Blazor.Components.Pages;

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
        var profile = await PlayerManager.GetCurrentProfileAsync();
        var profileId = profile!.ProfileId;

        Logger.LogInformation("Generating new game with difficulty {Difficulty} for profile {ProfileId}", Difficulty, profileId);

        var gameState = await GameManager.CreateGameAsync(profileId, Difficulty);

        Logger.LogInformation("Successfully created game {GameId} for profile {ProfileId}", gameState.Id, profileId);

        Navigation.NavigateTo($"/game/{gameState.Id}");
    }
}