namespace XenobiaSoft.Sudoku.Strategies;

public class SinglesInColumnsStrategy : SolverStrategy
{
	public override bool Execute(ISudokuPuzzle puzzle)
	{
		var colPos = 0;
		var rowPos = 0;
        var changesMade = false;

		for (var col = 0; col < GameDimensions.Columns; col++)
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
				Console.WriteLine($"Setting cell:{loneRangerCell.Row}:{loneRangerCell.Column} to value {number}");
				loneRangerCell.Value = number;
				loneRangerCell.PossibleValues = string.Empty;
				changesMade = true;
            }
		}

		return changesMade;
	}
}