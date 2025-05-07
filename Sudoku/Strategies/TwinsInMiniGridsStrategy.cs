using XenobiaSoft.Sudoku.Exceptions;

namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInMiniGridsStrategy : SolverStrategy
{
	public override bool Execute(ISudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.GetAllCells())
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 2) continue;

			var twins = new List<Cell> { cell };

			foreach (var twinCell in puzzle.GetMiniGridCells(cell.Row, cell.Column))
			{
				if (cell == twinCell || cell.PossibleValues != twinCell.PossibleValues) continue;

				twins.Add(twinCell);

				foreach (var nonTwinCell in puzzle.GetMiniGridCells(cell.Row, cell.Column).Where(x => !twins.Contains(x)))
				{
					if (nonTwinCell.Value.HasValue || nonTwinCell.PossibleValues == cell.PossibleValues) continue;

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