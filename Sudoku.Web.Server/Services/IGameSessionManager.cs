namespace Sudoku.Web.Server.Services;

public interface IGameSessionManager
{
    IGameSession CurrentSession { get; }
    Task StartNewSession(string puzzleId, Cell[] board);
    Task PauseSession();
    Task ResumeSession();
    Task EndSession();
} 