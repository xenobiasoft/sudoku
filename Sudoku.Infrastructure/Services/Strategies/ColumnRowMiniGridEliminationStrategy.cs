using Sudoku.Domain.Entities;

namespace XenobiaSoft.Sudoku.Infrastructure.Services.Strategies;

public class ColumnRowMiniGridEliminationStrategy : SolverStrategy
{
	public override bool Execute(SudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.Cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Count != 1) continue;

			var cellValue = cell.PossibleValues.First();
			Console.WriteLine($"Setting cell:{cell.Row}:{cell.Column} to value {cellValue}");
			cell.SetValue(cellValue);
			cell.PossibleValues.Clear();
            changesMade = true;
        }

		return changesMade;
	}
}