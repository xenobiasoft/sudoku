using Sudoku.Domain.Entities;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TripletsInMiniGridsStrategy : TripletsStrategyBase
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

                changesMade |= HandleNakedTriplets(miniGridCells);

                changesMade |= HandleHiddenTriplets(miniGridCells);
            }
        }

        return changesMade;
    }
}