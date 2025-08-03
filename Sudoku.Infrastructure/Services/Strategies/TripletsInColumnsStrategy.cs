using Sudoku.Domain.Entities;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TripletsInColumnsStrategy : SolverStrategy
{
    public override bool Execute(SudokuPuzzle puzzle)
    {
        var changesMade = false;

        for (var col = 0; col < 9; col++)
        {
            var columnCells = puzzle.GetColumnCells(col)
                .Where(c => !c.Value.HasValue)
                .ToList();

            changesMade |= TripletStrategies.HandleNakedTriplets(columnCells);

            changesMade |= TripletStrategies.HandleHiddenTriplets(columnCells);
        }

        return changesMade;
    }
}
