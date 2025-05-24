using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Abstractions;

public interface IGameSessionManager
{
    IGameSession CurrentSession { get; }
    Task StartNewSession(GameStateMemory gameState);
    Task PauseSession();
    void ResumeSession(GameStateMemory gameState);
    Task EndSession();
    Task RecordMove(bool isValid);
} 