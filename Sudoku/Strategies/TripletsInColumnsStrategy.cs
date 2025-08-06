using XenobiaSoft.Sudoku.Exceptions;

namespace XenobiaSoft.Sudoku.Strategies;

[Obsolete("This class is obsolete. Use Sudoku.Infrastructure.Services.Strategies.TripletsInColumnsStrategy instead.")]
public class TripletsInColumnsStrategy : SolverStrategy
{
	public override bool Execute(ISudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.GetAllCells())
		{
			if (cell.Value.HasValue || cell.PossibleValues.Count != 3) continue;

			var triplets = new List<Cell> { cell };

			foreach (var colCell in puzzle.GetColumnCells(cell.Column).Where(x => x != cell))
			{
				if (cell.PossibleValues == colCell.PossibleValues ||
					(colCell.PossibleValues.Count == 3 &&
					 cell.PossibleValues.Contains(colCell.PossibleValues[0]) &&
					 cell.PossibleValues.Contains(colCell.PossibleValues[1]))
					)
				{
					triplets.Add(colCell);
				}
			}

			if (triplets.Count != 3) continue;

			foreach (var nonTripletCell in puzzle.GetColumnCells(cell.Column))
			{
				if (nonTripletCell.Value.HasValue || triplets.Contains(nonTripletCell)) continue;

				nonTripletCell.PossibleValues = nonTripletCell.PossibleValues.Where(x => x != cell.PossibleValues[0]).ToList();
				nonTripletCell.PossibleValues = nonTripletCell.PossibleValues.Where(x => x != cell.PossibleValues[1]).ToList();
                nonTripletCell.PossibleValues = nonTripletCell.PossibleValues.Where(x => x != cell.PossibleValues[2]).ToList();

                if (!nonTripletCell.PossibleValues.Any())
				{
					throw new InvalidMoveException();
				}

				if (nonTripletCell.PossibleValues.Count != 1) continue;

				var cellValue = nonTripletCell.PossibleValues.First();
				Console.WriteLine($"Setting cell:{nonTripletCell.Row}:{nonTripletCell.Column} to value {cellValue}");
				nonTripletCell.Value = cellValue;
				nonTripletCell.PossibleValues = [];
                changesMade = true;
            }
		}

		return changesMade;
	}
}