using Microsoft.AspNetCore.Components;

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

    private void HandleCellFocus(Cell cell)
    {
        _selectedCell = cell;
    }

    private void SetSelectedCell(int value)
    {
        _selectedCell.Value = value;
    }
}