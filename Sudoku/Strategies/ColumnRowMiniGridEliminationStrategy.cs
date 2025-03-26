namespace XenobiaSoft.Sudoku.Strategies;

public class ColumnRowMiniGridEliminationStrategy : SolverStrategy
{
	private const int Score = 1;

	public override int Execute(ISudokuPuzzle puzzle)
	{
		var totalScore = 0;

		foreach (var cell in puzzle.GetAllCells())
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 1) continue;

			var cellValue = int.Parse(cell.PossibleValues);
			Console.WriteLine($"Setting cell:{cell.Row}:{cell.Column} to value {cellValue}");
			cell.Value = cellValue;
			cell.PossibleValues = string.Empty;
			totalScore += Score;
		}

		return totalScore;
	}
}