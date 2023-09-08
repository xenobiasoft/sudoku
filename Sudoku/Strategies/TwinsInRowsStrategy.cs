﻿namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInRowsStrategy : SolverStrategy
{
	private const int Score = 3;

	public override int Execute(Cell[] cells)
	{
		var changed = false;

		foreach (var cell in cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 2) continue;

			foreach (var twinCell in cells.GetRowCells(cell.Row).Where(x => x != cell))
			{
				if (cell.PossibleValues != twinCell.PossibleValues) continue;

				foreach (var nonTwinCell in cells.GetRowCells(cell.Row).Where(x => x != cell && x != twinCell))
				{
					if (nonTwinCell.Value.HasValue) continue;

					nonTwinCell.PossibleValues = nonTwinCell.PossibleValues.Replace(cell.PossibleValues[0].ToString(), string.Empty);
					nonTwinCell.PossibleValues = nonTwinCell.PossibleValues.Replace(cell.PossibleValues[1].ToString(), string.Empty);

					if (string.IsNullOrWhiteSpace(nonTwinCell.PossibleValues))
					{
						throw new InvalidOperationException("An invalid move was made");
					}

					if (nonTwinCell.PossibleValues.Length != 1) continue;

					nonTwinCell.Value = int.Parse(nonTwinCell.PossibleValues);

					changed = true;
				}
			}
		}

		return changed ? Score : 0;
	}
}