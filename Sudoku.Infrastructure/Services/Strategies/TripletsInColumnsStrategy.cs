using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TripletsInColumnsStrategy : SolverStrategy
{
	public override bool Execute(SudokuPuzzle puzzle)
	{
		var changesMade = false;

        foreach (var cell in puzzle.Cells)
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