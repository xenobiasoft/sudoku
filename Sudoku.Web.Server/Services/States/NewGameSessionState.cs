using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.States;

/// <summary>
/// Represents a new game session state
/// </summary>
public class NewGameSessionState(IGameSession session) : IGameSessionState
{
    public void End()
    {
        session.Timer.Pause();
        session.ChangeState(new CompletedGameSessionState(session));
    }

    public void Pause()
    {
        // New game state doesn't pause
    }

    public void RecordMove(bool isValid)
    {
        // New game state doesn't record moves
    }

    public void ReloadBoard(GameStateMemory gameState)
    {
        session.ReloadGameState(gameState);
    }

    public void Resume()
    {
        // New game state doesn't resume
    }

    public void Start()
    {
        session.Timer.Start();
        session.ChangeState(new ActiveGameSessionState(session));
    }
}