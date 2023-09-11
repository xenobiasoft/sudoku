using XenobiaSoft.Sudoku.Exceptions;

namespace XenobiaSoft.Sudoku.Strategies;

public class TripletsInColumnsStrategy : SolverStrategy
{
	private const int Score = 4;

	public override int Execute(Cell[] cells)
	{
		var totalScore = 0;

		foreach (var cell in cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 3) continue;

			var triplets = new List<Cell> { cell };

			foreach (var colCell in cells.GetColumnCells(cell.Column).Where(x => x != cell))
			{
				if (cell.PossibleValues == colCell.PossibleValues ||
					(colCell.PossibleValues.Length == 3 &&
					 cell.PossibleValues.Contains(colCell.PossibleValues[0].ToString()) &&
					 cell.PossibleValues.Contains(colCell.PossibleValues[1].ToString()))
					)
				{
					triplets.Add(colCell);
				}
			}

			if (triplets.Count != 3) continue;

			foreach (var nonTripletCell in cells.GetColumnCells(cell.Column))
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

				nonTripletCell.Value = int.Parse(nonTripletCell.PossibleValues);
				totalScore += Score;
			}
		}

		return totalScore;
	}
}