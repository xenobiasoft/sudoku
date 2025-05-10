using Microsoft.AspNetCore.Components;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Components;

public partial class GameThumbnail
{
    [Parameter] public GameStateMemory? Game { get; set; }
    [Parameter] public EventCallback<string> OnDeleteGame { get; set; }
    [Parameter] public EventCallback<string> OnLoadGame { get; set; }

    private async Task DeleteGame()
    {
        if (Game is null)
        {
            return;
        }
        await OnDeleteGame.InvokeAsync(Game.PuzzleId);
    }

    private async Task LoadGame()
    {
        if (Game is null)
        {
            return;
        }
        await OnLoadGame.InvokeAsync(Game.PuzzleId);
    }
}