using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class GameSessionManager(IGameTimer timer, IGameStateManager gameStateManager) : IGameSessionManager
{
    private IGameSession _currentSession = NullGameSession.Instance;

    public IGameSession CurrentSession => _currentSession;

    public async Task StartNewSession(GameStateMemory gameState)
    {
        _currentSession = new GameSession(gameState, timer);
        _currentSession.Timer.Start();
        await SaveSessionAsync();
    }

    public async Task PauseSession()
    {
        if (_currentSession is NullGameSession) return;

        _currentSession.Timer.Pause();
        await SaveSessionAsync();
    }

    public void ResumeSession()
    {
        if (_currentSession is NullGameSession) return;
        
        _currentSession.Timer.Resume();
    }

    public async Task EndSession()
    {
        if (_currentSession is NullGameSession) return;
        
        _currentSession.Timer.Pause();
        await SaveSessionAsync();
        _currentSession = NullGameSession.Instance;
    }

    private async Task SaveSessionAsync()
    {
        if (_currentSession is NullGameSession) return;

        var gameState = new GameStateMemory(_currentSession.PuzzleId, _currentSession.Board)
        {
            InvalidMoves = _currentSession.InvalidMoves,
            TotalMoves = _currentSession.TotalMoves,
            PlayDuration = _currentSession.PlayDuration
        };

        await gameStateManager.SaveGameAsync(gameState);
    }
} 