using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Pages;

public partial class New
{
    private readonly string[] Digits = ["1", "2", "3", "4", "5", "6", "7", "8", "9"];

    [Parameter] public string Difficulty { get; set; } = "Easy";
    [Inject] public required NavigationManager Navigation { get; set; }
    [Inject] public required Services.Abstractions.V2.IGameStateManager GameStateManager { get; set; }
    [Inject] public required IAliasService AliasService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await StartNewGameAsync();
    }

    private async Task StartNewGameAsync()
    {
        var alias = await AliasService.GetAliasAsync();
        
        var result = await GameStateManager.CreateGameAsync(alias, Difficulty);
        
        if (result.IsSuccess && result.Value != null)
        {
            Navigation.NavigateTo($"/game/{result.Value.Id}");
        }
        else
        {
            // Handle error - could navigate to an error page or show a notification
            // For now, navigate to home page
            Navigation.NavigateTo("/");
        }
    }
}