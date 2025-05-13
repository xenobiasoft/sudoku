using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public class GameSessionManager(IGameTimer timer, IGameStateManager gameStateManager) : IGameSessionManager
{
    private GameSession? _currentSession;

    public IGameSession CurrentSession => _currentSession ?? throw new InvalidOperationException("No active session");

    public async Task StartNewSession(string puzzleId, Cell[] board)
    {
        _currentSession = new GameSession(puzzleId, board, timer);
        timer.Start();
        await SaveSessionAsync();
    }

    public async Task PauseSession()
    {
        if (_currentSession == null) return;
        
        timer.Pause();
        await SaveSessionAsync();
    }

    public Task ResumeSession()
    {
        if (_currentSession == null) return Task.CompletedTask;
        
        timer.Resume();
        return Task.CompletedTask;
    }

    public async Task EndSession()
    {
        if (_currentSession == null) return;
        
        timer.Pause();
        await SaveSessionAsync();
        _currentSession = null;
    }

    private async Task SaveSessionAsync()
    {
        if (_currentSession == null) return;

        var gameState = new GameStateMemory(_currentSession.PuzzleId, _currentSession.Board)
        {
            InvalidMoves = _currentSession.InvalidMoves,
            TotalMoves = _currentSession.TotalMoves,
            PlayDuration = _currentSession.PlayDuration,
            StartTime = _currentSession.StartTime,
            LastResumeTime = timer.IsRunning ? DateTime.UtcNow : null
        };

        await gameStateManager.SaveGameAsync(gameState);
    }
} 