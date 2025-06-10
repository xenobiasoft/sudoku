using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Services.States;

/// <summary>
/// Represents an active game session state
/// </summary>
public class ActiveGameSessionState(IGameSession session) : IGameSessionState
{
    public void End()
    {
        session.Timer.Pause();
        session.SessionState = new CompletedGameSessionState(session);
    }

    public void Pause()
    {
        session.Timer.Pause();
    }

    public void RecordMove(bool isValid)
    {
        session.GameState.TotalMoves++;

        if (!isValid) session.GameState.InvalidMoves++;
    }

    public void Resume()
    {
        Resume(TimeSpan.Zero);
    }

    public void Resume(TimeSpan initialDuration)
    {
        session.Timer.Resume(initialDuration);
    }

    public void Start()
    {
        // Active game state doesn't start
    }
}