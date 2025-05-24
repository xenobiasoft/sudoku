namespace Sudoku.Web.Server.Services.Abstractions;

public interface IInvalidCellNotificationService
{
    void Notify(IEnumerable<Cell> invalidCells);
    event EventHandler<IEnumerable<Cell>> NotifyInvalidCells;
}