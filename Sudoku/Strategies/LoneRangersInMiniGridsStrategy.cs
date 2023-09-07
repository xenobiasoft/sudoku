namespace XenobiaSoft.Sudoku.Strategies;

public class LoneRangersInMiniGridsStrategy : SolverStrategy
{
	private const int Score = 2;

	public override int Execute(SudokuPuzzle puzzle)
	{
		var colPos = 0;
		var rowPos = 0;
		var changed = false;

		for (var col = 0; col < SudokuPuzzle.Columns; col = col + 3)
		{
			for (var row = 0; row < SudokuPuzzle.Rows; row = row + 3)
			{
				for (var number = 1; number <= 9; number++)
				{
					var occurrence = 0;

					foreach (var miniGridCell in puzzle.GetMiniGridCells(row, col))
					{
						if (miniGridCell.Value.HasValue || !miniGridCell.PossibleValues.Contains(number.ToString())) continue;

						occurrence += 1;
						colPos = miniGridCell.Column;
						rowPos = miniGridCell.Row;

						if (occurrence > 1) break;
					}

					if (occurrence != 1) continue;

					var loneRangerCell = puzzle.GetCell(rowPos, colPos);
					loneRangerCell.Value = number;
					loneRangerCell.PossibleValues = number.ToString();
					changed = true;
				}
			}
		}

		return changed ? Score : 0;
	}
}