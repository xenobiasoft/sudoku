using System.Text;
using XenobiaSoft.Sudoku.Helpers;

namespace XenobiaSoft.Sudoku.Strategies;

public class TripletsInMiniGridsStrategy : SolverStrategy
{
	private const int Score = 4;

	public override int Execute(SudokuPuzzle puzzle)
	{
		var changed = false;

		foreach (var cell in puzzle.Cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 3) continue;

			var triplets = new List<Cell>() { cell };

			foreach (var miniGridCell in puzzle.GetMiniGridCells(cell.Row, cell.Column).Where(x => x != cell))
			{
				if (miniGridCell != cell &&
				    (
					    cell.PossibleValues == miniGridCell.PossibleValues ||
					    (miniGridCell.PossibleValues.Length == 2 &&
					     cell.PossibleValues.Contains(miniGridCell.PossibleValues[0].ToString()) &&
					     cell.PossibleValues.Contains(miniGridCell.PossibleValues[1].ToString()))
				    ))
				{
					triplets.Add(miniGridCell);
				}
			}

			if (triplets.Count != 3) continue;

			foreach (var nonTripletCell in puzzle.GetMiniGridCells(cell.Row, cell.Column))
			{
				if (nonTripletCell.Value.HasValue || triplets.Contains(nonTripletCell)) continue;

				nonTripletCell.PossibleValues = nonTripletCell.PossibleValues.Replace(cell.PossibleValues[0].ToString(), string.Empty);
				nonTripletCell.PossibleValues = nonTripletCell.PossibleValues.Replace(cell.PossibleValues[1].ToString(), string.Empty);
				nonTripletCell.PossibleValues = nonTripletCell.PossibleValues.Replace(cell.PossibleValues[2].ToString(), string.Empty);

				if (string.IsNullOrWhiteSpace(nonTripletCell.PossibleValues))
				{
					throw new InvalidOperationException("An invalid move was made");
				}

				if (nonTripletCell.PossibleValues.Length != 1) continue;

				nonTripletCell.Value = int.Parse(nonTripletCell.PossibleValues);

				changed = true;
			}
		}

		return changed ? Score : 0;
	}
}