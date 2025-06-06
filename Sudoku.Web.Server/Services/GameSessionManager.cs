using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// Manages game sessions and their persistence
/// </summary>
public class GameSessionManager(IGameTimer timer, IGameStateManager gameStateManager) : IGameSessionManager
{
    private readonly IGameTimer _timer = timer ?? throw new ArgumentNullException(nameof(timer));
    private readonly IGameStateManager _gameStateManager = gameStateManager ?? throw new ArgumentNullException(nameof(gameStateManager));
    private IGameSession _currentSession = NullGameSession.Instance;

    public IGameSession CurrentSession => _currentSession;

    public async Task StartNewSession(GameStateMemory gameState)
    {
        if (gameState == null) throw new ArgumentNullException(nameof(gameState));

        _currentSession = new GameSession(gameState, _timer);
        _currentSession.Start();
        await SaveSessionAsync();
    }

    public async Task PauseSession()
    {
        if (_currentSession.IsNull) return;

        _currentSession.Pause();
        await SaveSessionAsync();
    }

    public async Task ResumeSession(GameStateMemory gameState)
    {
        if (gameState == null) throw new ArgumentNullException(nameof(gameState));
        if (_currentSession.IsNull) return;

        _currentSession.ReloadBoard(gameState);
        _currentSession.Resume();
        await SaveSessionAsync();
    }

    public async Task EndSession()
    {
        if (_currentSession.IsNull) return;

        _currentSession.End();
        await SaveSessionAsync();
        _currentSession = NullGameSession.Instance;
    }

    public async Task RecordMove(bool isValid)
    {
        if (_currentSession.IsNull) return;

        _currentSession.RecordMove(isValid);
        await SaveSessionAsync();
    }

    private async Task SaveSessionAsync()
    {
        if (_currentSession.IsNull) return;

        await _gameStateManager.SaveGameAsync(_currentSession.GameState);
    }
}