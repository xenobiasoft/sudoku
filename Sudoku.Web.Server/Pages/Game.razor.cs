using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Components;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
    private Cell _selectedCell;

	public Cell[] Puzzle { get; set; }

    [Parameter] 
    public Level Level { get; set; }

	[Inject]
	public ISudokuGame SudokuGame { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await NewGame(Level.Easy);
    }

    protected override void OnInitialized()
    {
        var values = new int?[9, 9];
        var cells1 = new Cell[81];
        var index = 0;

        for (var col = 0; col < 9; col++)
        {
            for (var row = 0; row < 9; row++)
            {
                var cell = new Cell(row, col) { Value = values[row, col] };
                cells1[index++] = cell;
            }
        }

        var cells = cells1;

        Puzzle = cells;
    }

    private async Task NewGame(Level level)
    {
        Level = level;

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

    private void HandleCellFocus(CellFocusEventArgs e)
    {
        _selectedCell = Puzzle.GetCell(e.Row, e.Column);
    }

    private void SetSelectedCell(int value)
    {
        _selectedCell.Value = value;
    }
}