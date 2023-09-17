using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
    private Cell _selectedCell = new(0, 0);

	public Cell[] Puzzle { get; set; } = new Cell[81];

	[Inject]
	public ISudokuGame? SudokuGame { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await NewGame(Level.Easy);
    }

    protected override void OnInitialized()
    {
        var values = new int?[9, 9];
        var cells = new Cell[81];
        var index = 0;

        for (var col = 0; col < 9; col++)
        {
            for (var row = 0; row < 9; row++)
            {
                var cell = new Cell(row, col) { Value = values[row, col] };
                cells[index++] = cell;
            }
        }
        
        Puzzle = cells;
    }

    private async Task NewGame(Level level)
    {
        try
        {
            await SudokuGame.New(level).ConfigureAwait(false);
            Puzzle = SudokuGame.Puzzle;
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
            await NewGame(level);
        }
    }

    private void SetCellValue(int value)
    {
        _selectedCell.Value = value;
    }

    private void HandleCellFocus(Cell cell)
    {
        _selectedCell = cell;
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
        var cell = Puzzle
            .GetColumnCells(_selectedCell.Column)
            .Where(x => !x.Locked && x.Row < _selectedCell.Row)
            .MaxBy(x => x.Row);

        Console.WriteLine($"Shift focus to cell {cell?.Row}:{cell?.Column}");
    }

    private void FocusRight()
    {
        var cell = Puzzle
            .GetRowCells(_selectedCell.Row)
            .Where(x => !x.Locked && x.Column > _selectedCell.Column)
            .MinBy(x => x.Column);

        Console.WriteLine($"Shift focus to cell {cell?.Row}:{cell?.Column}");
    }

    private void FocusLeft()
    {
        var cell = Puzzle
            .GetRowCells(_selectedCell.Row)
            .Where(x => !x.Locked && x.Column < _selectedCell.Column)
            .MaxBy(x => x.Column);

        Console.WriteLine($"Shift focus to cell {cell?.Row}:{cell?.Column}");
    }

    private void FocusDown()
    {
        var cell = Puzzle
            .GetColumnCells(_selectedCell.Column)
            .Where(x => !x.Locked && x.Row > _selectedCell.Row)
            .MinBy(x => x.Row);

        Console.WriteLine($"Shift focus to cell {cell?.Row}:{cell?.Column}");
    }
}