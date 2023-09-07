namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInColumnsStrategy : SolverStrategy
{
	private const int Score = 3;

	public override int Execute(SudokuPuzzle puzzle)
	{
		var changed = false;

		foreach (var cell in puzzle.Cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 2) continue;

			foreach (var colCell in puzzle.GetColumnCells(cell.Column).Where(x => x != cell))
			{
				if (cell.PossibleValues != colCell.PossibleValues) continue;

				foreach (var nonTwinCell in puzzle.GetColumnCells(cell.Column))
				{
					if (nonTwinCell.Value.HasValue || nonTwinCell.Row == cell.Row || nonTwinCell.Row == colCell.Row) continue;

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