namespace Sudoku.Web.Server.Services.Abstractions;

public interface ICellFocusedNotificationService
{
    void Notify(Cell cell);
    event EventHandler<Cell> SetCellFocus;
}