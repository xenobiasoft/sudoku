using XenobiaSoft.Sudoku.Exceptions;

namespace XenobiaSoft.Sudoku.Strategies;

[Obsolete("This class is obsolete. Use Sudoku.Infrastructure.Services.Strategies.TwinsInColumnsStrategy instead.")]
public class TwinsInColumnsStrategy : SolverStrategy
{
	public override bool Execute(ISudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.GetAllCells())
		{
			if (cell.Value.HasValue || cell.PossibleValues.Count != 2) continue;

			foreach (var colCell in puzzle.GetColumnCells(cell.Column).Where(x => x != cell))
			{
				if (!cell.PossibleValues.SequenceEqual(colCell.PossibleValues)) continue;

				foreach (var nonTwinCell in puzzle.GetColumnCells(cell.Column))
				{
					if (nonTwinCell.Value.HasValue || nonTwinCell.Row == cell.Row || nonTwinCell.Row == colCell.Row) continue;

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