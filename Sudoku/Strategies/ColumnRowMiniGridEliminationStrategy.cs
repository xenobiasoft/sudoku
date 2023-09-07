namespace XenobiaSoft.Sudoku.Strategies;

public class ColumnRowMiniGridEliminationStrategy : SolverStrategy
{
	private const int Score = 1;

	public override int Execute(SudokuPuzzle puzzle)
	{
		var changed = false;

		foreach (var cell in puzzle.Cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 1) continue;

			cell.Value = int.Parse(cell.PossibleValues);
			cell.PossibleValues = string.Empty;
			changed = true;
		}

		return changed ? Score : 0;
	}
}