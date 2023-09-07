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

				foreach (var colCell in puzzle.GetColumnCells(col))
				{
					if (colCell.Value.HasValue || !colCell.PossibleValues.Contains(number.ToString())) continue;

					occurrence += 1;

					if (occurrence > 1)
					{
						break;
					}

					colPos = colCell.Column;
					rowPos = colCell.Row;
				}

				if (occurrence != 1) continue;

				var loneRangerCell = puzzle.GetCell(rowPos, colPos);
				loneRangerCell.Value = number;
				loneRangerCell.PossibleValues = string.Empty;

				changed = true;
			}
		}

		return changed ? Score : 0;
	}
}