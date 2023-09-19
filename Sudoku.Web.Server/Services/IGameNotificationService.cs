namespace Sudoku.Web.Server.Services;

public interface IGameNotificationService
{
    event Action GameStarted;
    event Action GameEnded;

    void NotifyGameStarted();
    void NotifyGameEnded();
}