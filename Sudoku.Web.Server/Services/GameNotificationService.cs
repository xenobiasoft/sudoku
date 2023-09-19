namespace Sudoku.Web.Server.Services
{
    public class GameNotificationService : IGameNotificationService
    {
        public event Action? GameStarted;
        public event Action? GameEnded;

        public void NotifyGameStarted()
        {
            GameStarted?.Invoke();
        }

        public void NotifyGameEnded()
        {
            GameEnded?.Invoke();
        }
    }
}
