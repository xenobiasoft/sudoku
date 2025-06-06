﻿namespace XenobiaSoft.Sudoku.Strategies;

public class SinglesInMiniGridsStrategy : SolverStrategy
{
	public override bool Execute(ISudokuPuzzle puzzle)
	{
		var colPos = 0;
		var rowPos = 0;
        var changesMade = false;

		for (var col = 0; col < GameDimensions.Columns; col += 3)
		{
			for (var row = 0; row < GameDimensions.Rows; row += 3)
			{
				for (var number = 1; number <= 9; number++)
				{
					var occurrence = 0;

					foreach (var miniGridCell in puzzle.GetMiniGridCells(row, col))
					{
						if (miniGridCell.Value == number) break;
						if (miniGridCell.Value.HasValue || !miniGridCell.PossibleValues.Contains(number)) continue;

						occurrence += 1;
						colPos = miniGridCell.Column;
						rowPos = miniGridCell.Row;

						if (occurrence > 1) break;
					}

					if (occurrence != 1) continue;

					var loneRangerCell = puzzle.GetCell(rowPos, colPos);
					Console.WriteLine($"Setting cell:{loneRangerCell.Row}:{loneRangerCell.Column} to value {number}");
					loneRangerCell.Value = number;
					loneRangerCell.PossibleValues = [];
                    changesMade = true;
                }
			}
		}

		return changesMade;
	}
}