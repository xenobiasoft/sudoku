namespace Sudoku.Web.Server.Services
{
    public class InvalidCellNotificationService : IInvalidCellNotificationService
    {
        public void Notify(IEnumerable<Cell> invalidCells)
        {
            NotifyInvalidCells?.Invoke(this, invalidCells);
        }

        public event EventHandler<IEnumerable<Cell>>? NotifyInvalidCells;
    }
}
