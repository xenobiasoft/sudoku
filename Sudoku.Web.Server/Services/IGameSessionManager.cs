using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public interface IGameSessionManager
{
    IGameSession CurrentSession { get; }
    Task StartNewSession(GameStateMemory gameState);
    Task PauseSession();
    Task ResumeSession();
    Task EndSession();
} 