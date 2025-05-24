namespace Sudoku.Web.Server.Services.Abstractions;

public interface IGameNotificationService
{
    event Action GameStarted;
    event Action GameEnded;

    void NotifyGameStarted();
    void NotifyGameEnded();
}