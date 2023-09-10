using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Pages;

public partial class Game
{
	[Parameter]
	public Cell[] Puzzle { get; set; }

	[Inject]
	public ISudokuGame SudokuGame { get; set; }

	protected override void OnInitialized()
	{
		SudokuGame.LoadPuzzle(GetPuzzle());
		SudokuGame.Puzzle.PopulatePossibleValues();
		Puzzle = SudokuGame.Puzzle;
	}

	public static Cell[] GetPuzzle()
	{
		var cells = PopulateCells(EasyPuzzle);

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

	private static readonly int?[,] EasyPuzzle = {
		{ 5, 3, null, null, 7, null, null, null, null },
		{ 6, null, null, 1, 9, 5, null, null, null },
		{ null, 9, 8, null, null, null, null, 6, null },
		{ 8, null, null, null, 6, null, null, null, 3 },
		{ 4, null, null, 8, null, 3, null, null, 1 },
		{ 7, null, null, null, 2, null, null, null, 6 },
		{ null, 6, null, null, null, null, 2, 8, null },
		{ null, null, null, 4, 1, 9, null, null, 5 },
		{ null, null, null, null, 8, null, null, 7, 9 }
	};
}