﻿namespace XenobiaSoft.Sudoku.Strategies;

public class LoneRangersInColumnsStrategy : SolverStrategy
{
	private const int Score = 2;

	public override int Execute(SudokuPuzzle puzzle)
	{
		var colPos = 0;
		var rowPos = 0;
		var changed = false;

		for (var col = 0; col < SudokuPuzzle.Columns; col++)
		{
			for (var number = 1; number <= 9; number++)
			{
				var occurrence = 0;

				for (var row = 0; row < SudokuPuzzle.Rows; row++)
				{
					if (puzzle.Values[col, row] != 0 || !puzzle.PossibleValues[col, row].Contains(number.ToString())) continue;

					occurrence += 1;

					if (occurrence > 1)
					{
						break;
					}

					colPos = col;
					rowPos = row;
				}

				if (occurrence != 1) continue;

				puzzle.Values[colPos, rowPos] = number;
				puzzle.PossibleValues[colPos, rowPos] = number.ToString();

				changed = true;
			}
		}

		return changed ? Score : 0;
	}
}