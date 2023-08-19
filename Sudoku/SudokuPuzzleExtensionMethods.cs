using System.Text;
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

	public static bool IsValid(this SudokuPuzzle puzzle)
	{
		for (var row = 0; row < SudokuPuzzle.Rows; row++)
		{
			var usedNumbers = new StringBuilder();

			for (var col = 0; col < SudokuPuzzle.Columns; col++)
			{
				if (usedNumbers.ToString().Contains(puzzle.Values[col, row].ToString()))
				{
					return false;
				}

				usedNumbers.Append(puzzle.Values[col, row]);
			}
		}

		for (var col = 0; col < SudokuPuzzle.Columns; col++)
		{
			var usedNumbers = new StringBuilder();

			for (var row = 0; row < SudokuPuzzle.Rows; row++)
			{
				if (usedNumbers.ToString().Contains(puzzle.Values[col, row].ToString()))
				{
					return false;
				}

				usedNumbers.Append(puzzle.Values[col, row]);
			}
		}

		for (var col = 0; col < SudokuPuzzle.Columns; col++)
		{
			for (var row = 0; row < SudokuPuzzle.Rows; row++)
			{
				var usedNumbers = new StringBuilder();
				var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);
				var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartRow(row);

				for (var miniGridCol = miniGridStartCol; miniGridCol < miniGridStartCol + 3; miniGridCol++)
				{
					for (var miniGridRow = miniGridStartRow; miniGridRow < miniGridStartRow + 3; miniGridRow++)
					{
						if (usedNumbers.ToString().Contains(puzzle.Values[miniGridCol, miniGridRow].ToString()))
						{
							return false;
						}

						usedNumbers.Append(puzzle.Values[miniGridCol, miniGridRow]);
					}
				}
			}
		}

		return true;
	}

	public static bool IsSolved(this SudokuPuzzle puzzle)
	{
		for (var row = 0; row < SudokuPuzzle.Rows; row++)
		{
			var pattern = "123456789";

			for (var col = 0; col < SudokuPuzzle.Columns; col++)
			{
				pattern = pattern.Replace(puzzle.Values[col, row].ToString(), string.Empty);
			}

			if (pattern.Length > 0)
			{
				return false;
			}
		}

		for (var col = 0; col < SudokuPuzzle.Columns; col++)
		{
			var pattern = "123456789";

			for (var row = 0; row < SudokuPuzzle.Rows; row++)
			{
				pattern = pattern.Replace(puzzle.Values[col, row].ToString(), string.Empty);
			}

			if (pattern.Length > 0)
			{
				return false;
			}
		}

		for (var col = 0; col < SudokuPuzzle.Columns; col++)
		{
			for (var row = 0; row < SudokuPuzzle.Rows; row++)
			{
				var pattern = "123456789";
				var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);
				var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartRow(row);

				for (var miniGridCol = miniGridStartCol; miniGridCol < miniGridStartCol + 3; miniGridCol++)
				{
					for (var miniGridRow = miniGridStartRow; miniGridRow < miniGridStartRow + 3; miniGridRow++)
					{
						pattern = pattern.Replace(puzzle.Values[miniGridCol, miniGridRow].ToString(), string.Empty);
					}
				}

				if (pattern.Length > 0)
				{
					return false;
				}
			}
		}

		return true;
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