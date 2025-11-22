using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.HttpClients;
using Sudoku.Web.Server.Services.States;
using IGameManager = Sudoku.Web.Server.Services.Abstractions.V2.IGameManager;
using ILocalStorageService = Sudoku.Web.Server.Services.Abstractions.V2.ILocalStorageService;

namespace Sudoku.Web.Server.Services.V2;

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