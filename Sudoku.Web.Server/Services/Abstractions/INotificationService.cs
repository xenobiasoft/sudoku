namespace Sudoku.Web.Server.Services.Abstractions;

/// <summary>
/// Unified notification service that combines all game-related notifications
/// </summary>
public interface INotificationService
{
    // Game notifications
    event Action GameStarted;
    event Action GameEnded;
    void NotifyGameStarted();
    void NotifyGameEnded();

    // Cell focus notifications
    event EventHandler<Cell> SetCellFocus;
    void NotifyCellFocused(Cell cell);

    // Invalid cell notifications
    event EventHandler<IEnumerable<Cell>> InvalidCellsNotified;
    void NotifyInvalidCells(IEnumerable<Cell> invalidCells);
}