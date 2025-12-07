using Sudoku.Web.Server.Models;

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
    event EventHandler<CellModel> SetCellFocus;
    void NotifyCellFocused(CellModel cell);

    // Invalid cell notifications
    event EventHandler<IEnumerable<CellModel>> InvalidCellsNotified;
    void NotifyInvalidCells(IEnumerable<CellModel> invalidCells);
}