using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TwinsInMiniGridsStrategy : SolverStrategy
{
	public override bool Execute(SudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.Cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Count != 2) continue;

			var twins = new List<Cell> { cell };

			foreach (var twinCell in puzzle.GetMiniGridCells(cell.Row, cell.Column))
			{
				if (cell == twinCell || !cell.PossibleValues.SequenceEqual(twinCell.PossibleValues)) continue;

				twins.Add(twinCell);

				foreach (var nonTwinCell in puzzle.GetMiniGridCells(cell.Row, cell.Column).Where(x => !twins.Contains(x)))
				{
					if (nonTwinCell.Value.HasValue || nonTwinCell.PossibleValues.SequenceEqual(cell.PossibleValues)) continue;

                    nonTwinCell.PossibleValues.RemoveWhere(x => cell.PossibleValues.Contains(x));

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