namespace Sudoku.Web.Server.Services;

public class CellFocusedNotificationService : ICellFocusedNotificationService
{
    public void Notify(Cell cell)
    {
        SetCellFocus?.Invoke(this, cell);
    }

    public event EventHandler<Cell>? SetCellFocus;
}