using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Pages;

public partial class New
{
    [Parameter]
    public string Difficulty { get; set; } = "Easy";

    [Inject] private NavigationManager? Navigation { get; set; }
    [Inject] private ISudokuGame? SudokuGame { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var puzzleId = await SudokuGame!.NewGameAsync(Difficulty.ParseLevel());

        Navigation!.NavigateTo($"/game/{puzzleId}");
    }
}