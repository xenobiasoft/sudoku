using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TripletsInMiniGridsStrategy : SolverStrategy
{
	public override bool Execute(SudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.Cells)
		{
			if (cell.Value.HasValue || cell.PossibleValues.Count != 3) continue;

			var triplets = new List<Cell>() { cell };

			foreach (var miniGridCell in puzzle.GetMiniGridCells(cell.Row, cell.Column).Where(x => x != cell))
			{
				if (miniGridCell != cell &&
				    (
					    cell.PossibleValues.SequenceEqual(miniGridCell.PossibleValues) ||
					    (miniGridCell.PossibleValues.Count == 2 &&
					     cell.PossibleValues.Contains(miniGridCell.PossibleValues[0]) &&
					     cell.PossibleValues.Contains(miniGridCell.PossibleValues[1]))
				    ))
				{
					triplets.Add(miniGridCell);
				}
			}

			if (triplets.Count != 3) continue;

			foreach (var nonTripletCell in puzzle.GetMiniGridCells(cell.Row, cell.Column))
			{
				if (nonTripletCell.Value.HasValue || triplets.Contains(nonTripletCell)) continue;

				nonTripletCell.PossibleValues.AddRange(nonTripletCell.PossibleValues.Where(x => x != cell.PossibleValues[0]).ToList());
                nonTripletCell.PossibleValues.AddRange(nonTripletCell.PossibleValues.Where(x => x != cell.PossibleValues[1]).ToList());
				nonTripletCell.PossibleValues.AddRange(nonTripletCell.PossibleValues.Where(x => x != cell.PossibleValues[2]).ToList());

				if (!nonTripletCell.PossibleValues.Any())
				{
					throw new InvalidMoveException($"Invalid move at position: {nonTripletCell.Row}, {nonTripletCell.Column}");
				}

				if (nonTripletCell.PossibleValues.Count != 1) continue;

				var cellValue = nonTripletCell.PossibleValues.First();
				Console.WriteLine($"Setting cell:{nonTripletCell.Row}:{nonTripletCell.Column} to value {cellValue}");
				nonTripletCell.SetValue(cellValue);
				nonTripletCell.PossibleValues.Clear();
				changesMade = true;
            }
		}

		return changesMade;
	}
}