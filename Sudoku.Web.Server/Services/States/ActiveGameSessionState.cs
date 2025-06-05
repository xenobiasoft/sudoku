using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.States;

/// <summary>
/// Represents an active game session state
/// </summary>
public class ActiveGameSessionState(IGameSession session) : IGameSessionState
{
    public void End()
    {
        session.Timer.Pause();
        session.ChangeState(new CompletedGameSessionState(session));
    }

    public void Pause()
    {
        session.Timer.Pause();
    }

    public void RecordMove(bool isValid)
    {
        session.IncrementTotalMoves();
        if (!isValid) session.IncrementInvalidMoves();
    }

    public void ReloadBoard(GameStateMemory gameState)
    {
        session.ReloadGameState(gameState);
    }

    public void Resume()
    {
        session.Timer.Resume();
    }

    public void Start()
    {
        // Active game state doesn't start
    }
}