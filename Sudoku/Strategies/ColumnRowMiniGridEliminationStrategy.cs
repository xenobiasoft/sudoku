namespace XenobiaSoft.Sudoku.Strategies;

public class ColumnRowMiniGridEliminationStrategy : SolverStrategy
{
	public override bool Execute(ISudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.GetAllCells())
		{
			if (cell.Value.HasValue || cell.PossibleValues.Count != 1) continue;

			var cellValue = cell.PossibleValues.First();
			Console.WriteLine($"Setting cell:{cell.Row}:{cell.Column} to value {cellValue}");
			cell.Value = cellValue;
			cell.PossibleValues = [];
            changesMade = true;
        }

		return changesMade;
	}
}