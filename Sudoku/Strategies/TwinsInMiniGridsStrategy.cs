namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInMiniGridsStrategy : SolverStrategy
{
	private const int Score = 3;

	public override int Execute(Cell[] cells)
	{
		var totalScore = 0;

		foreach (var cell in cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 2) continue;

			var twins = new List<Cell> { cell };

			foreach (var twinCell in cells.GetMiniGridCells(cell.Row, cell.Column))
			{
				if (cell == twinCell || cell.PossibleValues != twinCell.PossibleValues) continue;

				twins.Add(twinCell);

				foreach (var nonTwinCell in cells.GetMiniGridCells(cell.Row, cell.Column).Where(x => !twins.Contains(x)))
				{
					if (nonTwinCell.Value.HasValue || nonTwinCell.PossibleValues == cell.PossibleValues) continue;

					nonTwinCell.PossibleValues = nonTwinCell.PossibleValues.Replace(cell.PossibleValues[0].ToString(), string.Empty);
					nonTwinCell.PossibleValues = nonTwinCell.PossibleValues.Replace(cell.PossibleValues[1].ToString(), string.Empty);

					if (string.IsNullOrWhiteSpace(nonTwinCell.PossibleValues))
					{
						throw new InvalidOperationException("An invalid move was made");
					}

					if (nonTwinCell.PossibleValues.Length != 1) continue;

					nonTwinCell.Value = int.Parse(nonTwinCell.PossibleValues);
					totalScore += Score;
				}
			}
		}

		return totalScore;
	}
}