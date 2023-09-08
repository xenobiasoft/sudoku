namespace XenobiaSoft.Sudoku.Strategies;

public class TripletsInRowsStrategy : SolverStrategy
{
	private const int Score = 4;

	public override int Execute(Cell[] cells)
	{
		var changed = false;

		foreach (var cell in cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Length != 3) continue;

			var triplets = new List<Cell>() { cell };

			foreach (var rowCell in cells.GetRowCells(cell.Row).Where(x => x != cell))
			{
				if (cell.PossibleValues == rowCell.PossibleValues ||
				    (rowCell.PossibleValues.Length == 2 &&
				     cell.PossibleValues.Contains(rowCell.PossibleValues[0].ToString()) &&
				     cell.PossibleValues.Contains(rowCell.PossibleValues[1].ToString()))
				   )
				{
					triplets.Add(rowCell);
				}
			}

			if (triplets.Count != 3) continue;

			foreach (var nonTripletCell in cells.GetRowCells(cell.Row))
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