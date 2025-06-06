using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Services.States;

/// <summary>
/// Represents a new game session state
/// </summary>
public class NewGameSessionState(IGameSession session) : IGameSessionState
{
    public void End()
    {
        session.Timer.Pause();
        session.SessionState = new CompletedGameSessionState(session);
    }

    public void Pause()
    {
        // New game state doesn't pause
    }

    public void RecordMove(bool isValid)
    {
        // New game state doesn't record moves
    }

    public void Resume()
    {
        // New game state doesn't resume
    }

    public void Start()
    {
        session.Timer.Start();
        session.SessionState = new ActiveGameSessionState(session);
    }
}