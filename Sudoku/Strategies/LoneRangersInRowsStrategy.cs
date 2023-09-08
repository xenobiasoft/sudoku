namespace XenobiaSoft.Sudoku.Strategies;

public class LoneRangersInRowsStrategy : SolverStrategy
{
	private const int Score = 2;

	public override int Execute(Cell[] cells)
	{
		var colPos = 0;
		var rowPos = 0;
		var changed = false;

		for (var row = 0; row < SudokuGame.Rows; row++)
		{
			for (var number = 1; number <= 9; number++)
			{
				var occurrence = 0;

				foreach (var rowCell in cells.GetRowCells(row))
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

				var loneRangerCell = cells.GetCell(rowPos, colPos);
				loneRangerCell.Value = number;
				loneRangerCell.PossibleValues = number.ToString();

				changed = true;
			}
		}

		return changed ? Score : 0;
	}
}