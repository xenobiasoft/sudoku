using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.States;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class GameSession : IGameSession
{
    public GameSession(GameStateMemory gameState, IGameTimer timer)
    {
        Timer = timer;
        SessionState = new NewGameSessionState(this);
        GameState = gameState;
    }

    public bool IsNull => false;
    public IGameTimer Timer { get; }
    public GameStateMemory GameState { get; private set; }
    public IGameSessionState SessionState { get; set; }

    public void End()
    {
        SessionState.End();
    }

    public void Pause()
    {
        SessionState.Pause();
    }

    public void RecordMove(bool isValid)
    {
        SessionState.RecordMove(isValid);
    }

    public void ReloadBoard(GameStateMemory gameState)
    {
        GameState = gameState;
    }

    public void Resume()
    {
        SessionState.Resume();
    }

    public void Start()
    {
        SessionState.Start();
    }
}