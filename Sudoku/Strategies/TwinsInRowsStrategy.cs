using XenobiaSoft.Sudoku.Exceptions;

namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInRowsStrategy : SolverStrategy
{
	public override bool Execute(ISudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.GetAllCells())
		{
			if (cell.Value.HasValue || cell.PossibleValues.Count != 2) continue;

			foreach (var twinCell in puzzle.GetRowCells(cell.Row).Where(x => x != cell))
			{
				if (!cell.PossibleValues.SequenceEqual(twinCell.PossibleValues)) continue;

				foreach (var nonTwinCell in puzzle.GetRowCells(cell.Row).Where(x => x != cell && x != twinCell))
				{
					if (nonTwinCell.Value.HasValue) continue;

					nonTwinCell.PossibleValues = nonTwinCell.PossibleValues.Where(x => x != cell.PossibleValues[0]).ToList();
					nonTwinCell.PossibleValues = nonTwinCell.PossibleValues.Where(x => x != cell.PossibleValues[1]).ToList();

					if (!nonTwinCell.PossibleValues.Any())
					{
						throw new InvalidMoveException();
					}

					if (nonTwinCell.PossibleValues.Count != 1) continue;

					var cellValue = nonTwinCell.PossibleValues.First();
					Console.WriteLine($"Setting cell:{nonTwinCell.Row}:{nonTwinCell.Column} to value {cellValue}");
					nonTwinCell.Value = cellValue;
					nonTwinCell.PossibleValues = [];
					changesMade = true;
                }
			}
		}

		return changesMade;
	}
}