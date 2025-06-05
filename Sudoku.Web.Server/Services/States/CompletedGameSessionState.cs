using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.States;

/// <summary>
/// Represents a completed game session state
/// </summary>
public class CompletedGameSessionState(IGameSession session) : IGameSessionState
{
    public void End()
    {
        // Completed game state doesn't end
    }

    public void Pause()
    {
        // Completed game state doesn't pause
    }

    public void RecordMove(bool isValid)
    {
        // Completed game state doesn't record moves
    }

    public void ReloadBoard(GameStateMemory gameState)
    {
        // Completed game state doesn't reload board
    }

    public void Resume()
    {
        // Completed game state doesn't resume
    }

    public void Start()
    {
        // Completed game state doesn't start
    }
}