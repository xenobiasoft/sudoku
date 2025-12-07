using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions.V2;

namespace Sudoku.Web.Server.Components
{
    public partial class GameBoard
    {
        private CellModel _selectedCell = new();

        [Parameter] public GameModel? Game { get; set; }
        [Parameter] public INotificationService? NotificationService { get; set; }
        [Parameter] public EventCallback<CellModel> OnCellFocus { set; get; }
        [Parameter] public EventCallback<CellChangedEventArgs> OnCellChanged { get; set; }
        [Parameter] public EventCallback<CellPossibleValueChangedEventArgs> OnPossibleValueChanged { get; set; }
        [Parameter] public bool IsPencilMode { get; set; }

        private async Task HandleCellFocus(CellModel cell)
        {
            _selectedCell = cell;
            await OnCellFocus.InvokeAsync(cell);
        }

        private void KeyUp(KeyboardEventArgs e)
        {
            switch (e.Code)
            {
                case KeyCodes.DownKey:
                    FocusDown();
                    break;
                case KeyCodes.LeftKey:
                    FocusLeft();
                    break;
                case KeyCodes.RightKey:
                    FocusRight();
                    break;
                case KeyCodes.UpKey:
                    FocusUp();
                    break;
            }
        }

        private void FocusUp()
        {
            var cell = Game!
                .GetColumnCells(_selectedCell.Column)
                .Where(x => x.Row < _selectedCell.Row)
                .MaxBy(x => x.Row);

            NotificationService!.NotifyCellFocused(cell!);
        }

        private void FocusRight()
        {
            var cell = Game!
                .GetRowCells(_selectedCell.Row)
                .Where(x => x.Column > _selectedCell.Column)
                .MinBy(x => x.Column);

            NotificationService!.NotifyCellFocused(cell!);
        }

        private void FocusLeft()
        {
            var cell = Game!
                .GetRowCells(_selectedCell.Row)
                .Where(x => x.Column < _selectedCell.Column)
                .MaxBy(x => x.Column);

            NotificationService!.NotifyCellFocused(cell!);
        }

        private void FocusDown()
        {
            var cell = Game!
                .GetColumnCells(_selectedCell.Column)
                .Where(x => x.Row > _selectedCell.Row)
                .MinBy(x => x.Row);

            NotificationService!.NotifyCellFocused(cell!);
        }
    }
}