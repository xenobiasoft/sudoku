using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public interface IGameSessionManager
{
    IGameSession CurrentSession { get; }
    Task StartNewSession(GameStateMemory gameState);
    Task PauseSession();
    void ResumeSession();
    Task EndSession();
} 