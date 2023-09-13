using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
	[Parameter]
	public Cell[] Puzzle { get; set; }

	[Inject]
	public ISudokuGame SudokuGame { get; set; }

    protected override void OnInitialized()
    {
        Puzzle = GetPuzzle();
    }

    private async Task New()
    {
        try
        {
            await SudokuGame.New(Level.Easy).ConfigureAwait(false);
            Puzzle = SudokuGame.Puzzle;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static Cell[] GetPuzzle()
	{
		var cells = PopulateCells(new int?[9, 9]);

		return cells;
	}

    public static Cell[] PopulateCells(int?[,] values)
	{
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

		return cells;
	}
}