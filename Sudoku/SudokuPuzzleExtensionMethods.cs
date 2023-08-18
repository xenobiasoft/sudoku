using XenobiaSoft.Sudoku.Helpers;

namespace XenobiaSoft.Sudoku;

public static class SudokuPuzzleExtensionMethods
{
	public static void PopulatePossibleValues(this SudokuPuzzle puzzle)
	{
		for (var row = 0; row < SudokuPuzzle.Rows; row++)
		{
			for (var col = 0; col < SudokuPuzzle.Columns; col++)
			{
				puzzle.PossibleValues[col, row] = puzzle.Values[col, row] == 0 ?
					CalculatePossibleValues(puzzle, col, row) :
					string.Empty;
			}
		}
	}

	private static string CalculatePossibleValues(SudokuPuzzle puzzle, int col, int row)
	{
		var possible = puzzle.PossibleValues;
		var values = puzzle.Values;
		var possibleValues = string.IsNullOrWhiteSpace(possible[col, row]) ? "123456789" : possible[col, row];

		for (var r = 0; r < SudokuPuzzle.Rows; r++)
		{
			if (values[col, r] != 0)
			{
				possibleValues = possibleValues.Replace(values[col, r].ToString(), string.Empty);
			}
		}

		for (var c = 0; c < SudokuPuzzle.Columns; c++)
		{
			if (values[c, row] != 0)
			{
				possibleValues = possibleValues.Replace(values[c, row].ToString(), string.Empty);
			}
		}

		var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);
		var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartCol(row);

		for (var miniGridRow = miniGridStartRow; miniGridRow <= miniGridStartRow + 2; miniGridRow++)
		{
			for (var miniGridCol = miniGridStartCol; miniGridCol <= miniGridStartCol + 2; miniGridCol++)
			{
				if (values[miniGridCol, miniGridRow] != 0)
				{
					possibleValues = possibleValues.Replace(values[miniGridCol, miniGridRow].ToString(), string.Empty);
				}
			}
		}

		return possibleValues;
	}
}