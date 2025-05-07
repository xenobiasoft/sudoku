using XenobiaSoft.Sudoku.Exceptions;

namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInRowsStrategy : SolverStrategy
{
	public override bool Execute(ISudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.GetAllCells())
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 2) continue;

			foreach (var twinCell in puzzle.GetRowCells(cell.Row).Where(x => x != cell))
			{
				if (cell.PossibleValues != twinCell.PossibleValues) continue;

				foreach (var nonTwinCell in puzzle.GetRowCells(cell.Row).Where(x => x != cell && x != twinCell))
				{
					if (nonTwinCell.Value.HasValue) continue;

					nonTwinCell.PossibleValues = nonTwinCell.PossibleValues.Replace(cell.PossibleValues[0].ToString(), string.Empty);
					nonTwinCell.PossibleValues = nonTwinCell.PossibleValues.Replace(cell.PossibleValues[1].ToString(), string.Empty);

					if (string.IsNullOrWhiteSpace(nonTwinCell.PossibleValues))
					{
						throw new InvalidMoveException();
					}

					if (nonTwinCell.PossibleValues.Length != 1) continue;

					var cellValue = int.Parse(nonTwinCell.PossibleValues);
					Console.WriteLine($"Setting cell:{nonTwinCell.Row}:{nonTwinCell.Column} to value {cellValue}");
					nonTwinCell.Value = cellValue;
					nonTwinCell.PossibleValues = string.Empty;
					changesMade = true;
                }
			}
		}

		return changesMade;
	}
}