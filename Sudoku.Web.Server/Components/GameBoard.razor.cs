﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Components
{
    public partial class GameBoard
    {
        private Cell _selectedCell = new(0, 0);

        [Parameter] public ISudokuPuzzle? Puzzle { get; set; }
        [Parameter] public ICellFocusedNotificationService? NotificationService { get; set; }
        [Parameter] public EventCallback<Cell> OnCellFocus { set; get; }
        [Parameter] public EventCallback<CellChangedEventArgs> OnCellChanged { get; set; }
        [Parameter] public EventCallback<CellPossibleValueChangedEventArgs> OnPossibleValueChanged { get; set; }
        [Parameter] public bool IsPencilMode { get; set; }

        private async Task HandleCellFocus(Cell cell)
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
            var cell = Puzzle!
                .GetColumnCells(_selectedCell.Column)
                .Where(x => x.Row < _selectedCell.Row)
                .MaxBy(x => x.Row);

            NotificationService!.Notify(cell!);
        }

        private void FocusRight()
        {
            var cell = Puzzle!
                .GetRowCells(_selectedCell.Row)
                .Where(x => x.Column > _selectedCell.Column)
                .MinBy(x => x.Column);

            NotificationService!.Notify(cell!);
        }

        private void FocusLeft()
        {
            var cell = Puzzle!
                .GetRowCells(_selectedCell.Row)
                .Where(x => x.Column < _selectedCell.Column)
                .MaxBy(x => x.Column);

            NotificationService!.Notify(cell!);
        }

        private void FocusDown()
        {
            var cell = Puzzle!
                .GetColumnCells(_selectedCell.Column)
                .Where(x => x.Row > _selectedCell.Row)
                .MinBy(x => x.Row);

            NotificationService!.Notify(cell!);
        }
    }
}