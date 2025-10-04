using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// Unified notification service that combines all game-related notifications
/// </summary>
public class NotificationService : INotificationService
{
    // Game notifications
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

    // Cell focus notifications
    public event EventHandler<Cell>? SetCellFocus;

    public void NotifyCellFocused(Cell cell)
    {
        SetCellFocus?.Invoke(this, cell);
    }

    // Invalid cell notifications
    public event EventHandler<IEnumerable<Cell>>? InvalidCellsNotified;

    public void NotifyInvalidCells(IEnumerable<Cell> invalidCells)
    {
        InvalidCellsNotified?.Invoke(this, invalidCells);
    }
}