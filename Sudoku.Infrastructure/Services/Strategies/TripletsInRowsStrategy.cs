using Sudoku.Domain.Entities;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TripletsInRowsStrategy : TripletsStrategyBase
{
	public override bool Execute(SudokuPuzzle puzzle)
    {
        var changesMade = false;

        for (var row = 0; row < 9; row++)
        {
            var rowCells = puzzle.GetRowCells(row)
                .Where(c => !c.Value.HasValue)
                .ToList();

            changesMade |= HandleNakedTriplets(rowCells);

            changesMade |= HandleHiddenTriplets(rowCells);
        }

        return changesMade;
    }
}