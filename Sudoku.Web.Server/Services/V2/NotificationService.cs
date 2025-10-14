using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions.V2;

namespace Sudoku.Web.Server.Services.V2;

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
    public event EventHandler<CellModel>? SetCellFocus;

    public void NotifyCellFocused(CellModel cell)
    {
        SetCellFocus?.Invoke(this, cell);
    }

    // Invalid cell notifications
    public event EventHandler<IEnumerable<CellModel>>? InvalidCellsNotified;

    public void NotifyInvalidCells(IEnumerable<CellModel> invalidCells)
    {
        InvalidCellsNotified?.Invoke(this, invalidCells);
    }
}