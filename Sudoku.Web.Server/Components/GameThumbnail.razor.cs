using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Components;

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