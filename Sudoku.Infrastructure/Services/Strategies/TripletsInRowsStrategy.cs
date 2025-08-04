using Sudoku.Domain.Entities;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TripletsInRowsStrategy : SolverStrategy
{
	public override bool Execute(SudokuPuzzle puzzle)
    {
        var changesMade = false;

        for (var row = 0; row < 9; row++)
        {
            var rowCells = puzzle.GetRowCells(row)
                .Where(c => !c.Value.HasValue)
                .ToList();

            changesMade |= TripletStrategies.HandleNakedTriplets(rowCells);

            changesMade |= TripletStrategies.HandleHiddenTriplets(rowCells);
        }

        return changesMade;
    }
}