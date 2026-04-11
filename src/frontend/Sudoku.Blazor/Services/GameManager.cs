using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;
using Sudoku.Blazor.Services.HttpClients;
using ILocalStorageService = Sudoku.Blazor.Services.Abstractions.ILocalStorageService;

namespace Sudoku.Blazor.Services;

using IGameManager = Abstractions.IGameManager;

public partial class GameManager(ILocalStorageService localStorageService, IGameApiClient gameApiClient, IGameTimer gameTimer) : IGameManager
{
    public GameModel? Game { get; private set; }

    public async Task StartGameAsync()
    {
        switch (Game.Status)
        {
            case GameStatus.NotStarted:
                await StartNewSession();
                break;
            case GameStatus.InProgress:
            case GameStatus.Paused:
                await ResumeSession();
                break;
            case GameStatus.Completed:
            case GameStatus.Abandoned:
                break;
        }
    }
}