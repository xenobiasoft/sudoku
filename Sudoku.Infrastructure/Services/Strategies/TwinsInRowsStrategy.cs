using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;

namespace XenobiaSoft.Sudoku.Infrastructure.Services.Strategies;

public class TwinsInRowsStrategy : SolverStrategy
{
	public override bool Execute(SudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.Cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Count != 2) continue;

			foreach (var twinCell in puzzle.GetRowCells(cell.Row).Where(x => x != cell))
			{
				if (!cell.PossibleValues.SequenceEqual(twinCell.PossibleValues)) continue;

				foreach (var nonTwinCell in puzzle.GetRowCells(cell.Row).Where(x => x != cell && x != twinCell))
				{
					if (nonTwinCell.Value.HasValue) continue;

					nonTwinCell.PossibleValues.AddRange(nonTwinCell.PossibleValues.Where(x => x != cell.PossibleValues[0]).ToList());
					nonTwinCell.PossibleValues.AddRange(nonTwinCell.PossibleValues.Where(x => x != cell.PossibleValues[1]).ToList());

					if (!nonTwinCell.PossibleValues.Any())
					{
						throw new InvalidMoveException($"Invalid move for position: {nonTwinCell.Row}, {nonTwinCell.Column}");
					}

					if (nonTwinCell.PossibleValues.Count != 1) continue;

					var cellValue = nonTwinCell.PossibleValues.First();
					Console.WriteLine($"Setting cell:{nonTwinCell.Row}:{nonTwinCell.Column} to value {cellValue}");
					nonTwinCell.SetValue(cellValue);
					nonTwinCell.PossibleValues.Clear();
					changesMade = true;
                }
			}
		}

		return changesMade;
	}
}