using XenobiaSoft.Sudoku.Exceptions;

namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInMiniGridsStrategy : SolverStrategy
{
	public override bool Execute(ISudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.GetAllCells())
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