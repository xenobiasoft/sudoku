﻿namespace XenobiaSoft.Sudoku.Strategies;

public class SinglesInRowsStrategy : SolverStrategy
{
	private const int Score = 2;

	public override int Execute(ISudokuPuzzle puzzle)
	{
		var colPos = 0;
		var rowPos = 0;
		var totalScore = 0;

		for (var row = 0; row < GameDimensions.Rows; row++)
		{
			for (var number = 1; number <= 9; number++)
			{
				var occurrence = 0;

				foreach (var rowCell in puzzle.GetRowCells(row))
				{
					if (rowCell.Value.HasValue || !rowCell.PossibleValues.Contains(number.ToString())) continue;

					occurrence += 1;

					if (occurrence > 1)
					{
						break;
					}

					colPos = rowCell.Column;
					rowPos = rowCell.Row;
				}

				if (occurrence != 1) continue;

				var loneRangerCell = puzzle.GetCell(rowPos, colPos);
				Console.WriteLine($"Setting cell:{loneRangerCell.Row}:{loneRangerCell.Column} to value {number}");
				loneRangerCell.Value = number;
				loneRangerCell.PossibleValues = number.ToString();
				totalScore += Score;
			}
		}

		return totalScore;
	}
}