using Sudoku.Domain.Entities;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TripletsInMiniGridsStrategy : SolverStrategy
{
	public override bool Execute(SudokuPuzzle puzzle)
    {
        var changesMade = false;

        for (var boxRow = 0; boxRow < 3; boxRow++)
        {
            for (var boxCol = 0; boxCol < 3; boxCol++)
            {
                var miniGridCells = puzzle.GetMiniGridCells(boxRow, boxCol)
                    .Where(c => !c.Value.HasValue)
                    .ToList();

                changesMade |= TripletStrategies.HandleNakedTriplets(miniGridCells);

                changesMade |= TripletStrategies.HandleHiddenTriplets(miniGridCells);
            }
        }

        return changesMade;
    }
}