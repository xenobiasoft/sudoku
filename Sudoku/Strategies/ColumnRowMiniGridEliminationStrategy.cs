namespace XenobiaSoft.Sudoku.Strategies;

public class ColumnRowMiniGridEliminationStrategy : SolverStrategy
{
	public override bool Execute(ISudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.GetAllCells())
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 1) continue;

			var cellValue = int.Parse(cell.PossibleValues);
			Console.WriteLine($"Setting cell:{cell.Row}:{cell.Column} to value {cellValue}");
			cell.Value = cellValue;
			cell.PossibleValues = string.Empty;
            changesMade = true;
        }

		return changesMade;
	}
}