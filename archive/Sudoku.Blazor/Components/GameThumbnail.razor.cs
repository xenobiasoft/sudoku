using Microsoft.AspNetCore.Components;
using Sudoku.Blazor.Models;

namespace Sudoku.Blazor.Components;

public partial class GameThumbnail
{
    [Parameter] public GameModel? Game { get; set; }
    [Parameter] public EventCallback<string> OnDeleteGame { get; set; }
    [Parameter] public EventCallback<string> OnLoadGame { get; set; }

    private async Task DeleteGame()
    {
        if (Game is null)
        {
            return;
        }
        await OnDeleteGame.InvokeAsync(Game.Id);
    }

    private async Task LoadGame()
    {
        if (Game is null)
        {
            return;
        }
        await OnLoadGame.InvokeAsync(Game.Id);
    }
}