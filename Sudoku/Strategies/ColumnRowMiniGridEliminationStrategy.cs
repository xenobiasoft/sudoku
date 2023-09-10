namespace XenobiaSoft.Sudoku.Strategies;

public class ColumnRowMiniGridEliminationStrategy : SolverStrategy
{
	private const int Score = 1;

	public override int Execute(Cell[] cells)
	{
		var totalScore = 0;

		foreach (var cell in cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 1) continue;

			cell.Value = int.Parse(cell.PossibleValues);
			cell.PossibleValues = string.Empty;
			totalScore += Score;
		}

		return totalScore;
	}
}