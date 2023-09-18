namespace Sudoku.Web.Server.Services;

public interface ICellFocusedNotificationService
{
    void Notify(Cell cell);
    event EventHandler<Cell> SetCellFocus;
}