using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.States;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// Represents a null game session that does nothing
/// </summary>
public class NullGameSession : IGameSession
{
    public NullGameSession()
    {
        SessionState = new CompletedGameSessionState(this);
    }

    public static NullGameSession Instance { get; } = new();

    public bool IsNull => true;
    public IGameTimer Timer => new NullGameTimer();
    public GameStateMemory GameState { get; } = new();
    public IGameSessionState SessionState { get; set; }

    public void End() { }
    public void Pause() { }
    public void RecordMove(bool isValid) { }
    public void ReloadBoard(GameStateMemory gameState) { }
    public void Resume() { }
    public void Start() { }
}