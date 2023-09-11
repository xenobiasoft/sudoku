using XenobiaSoft.Sudoku.Exceptions;

namespace XenobiaSoft.Sudoku.Strategies;

public class TripletsInMiniGridsStrategy : SolverStrategy
{
	private const int Score = 4;

	public override int Execute(Cell[] cells)
	{
		var totalScore = 0;

		foreach (var cell in cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 3) continue;

			var triplets = new List<Cell>() { cell };

			foreach (var miniGridCell in cells.GetMiniGridCells(cell.Row, cell.Column).Where(x => x != cell))
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

			foreach (var nonTripletCell in cells.GetMiniGridCells(cell.Row, cell.Column))
			{
				if (nonTripletCell.Value.HasValue || triplets.Contains(nonTripletCell)) continue;

				nonTripletCell.PossibleValues = nonTripletCell.PossibleValues.Replace(cell.PossibleValues[0].ToString(), string.Empty);
				nonTripletCell.PossibleValues = nonTripletCell.PossibleValues.Replace(cell.PossibleValues[1].ToString(), string.Empty);
				nonTripletCell.PossibleValues = nonTripletCell.PossibleValues.Replace(cell.PossibleValues[2].ToString(), string.Empty);

				if (string.IsNullOrWhiteSpace(nonTripletCell.PossibleValues))
				{
					throw new InvalidMoveException();
				}

				if (nonTripletCell.PossibleValues.Length != 1) continue;

				var cellValue = int.Parse(nonTripletCell.PossibleValues);
				Console.WriteLine($"Setting cell:{nonTripletCell.Row}:{nonTripletCell.Column} to value {cellValue}");
				nonTripletCell.Value = cellValue;
				nonTripletCell.PossibleValues = string.Empty;
				totalScore += Score;
			}
		}

		return totalScore;
	}
}