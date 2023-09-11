﻿namespace XenobiaSoft.Sudoku.Strategies;

public class LoneRangersInColumnsStrategy : SolverStrategy
{
	private const int Score = 2;

	public override int Execute(Cell[] cells)
	{
		var colPos = 0;
		var rowPos = 0;
		var totalScore = 0;

		for (var col = 0; col < GameDimensions.Columns; col++)
		{
			for (var number = 1; number <= 9; number++)
			{
				var occurrence = 0;

				foreach (var colCell in cells.GetColumnCells(col))
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

				var loneRangerCell = cells.GetCell(rowPos, colPos);
				Console.WriteLine($"Setting cell:{loneRangerCell.Row}:{loneRangerCell.Column} to value {number}");
				loneRangerCell.Value = number;
				loneRangerCell.PossibleValues = string.Empty;

				totalScore += Score;
			}
		}

		return totalScore;
	}
}