using Sudoku.Domain.Entities;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TripletsInColumnsStrategy : TripletsStrategyBase
{
    public override bool Execute(SudokuPuzzle puzzle)
    {
        var changesMade = false;

        for (var col = 0; col < 9; col++)
        {
            var columnCells = puzzle.GetColumnCells(col)
                .Where(c => !c.Value.HasValue)
                .ToList();

            changesMade |= HandleNakedTriplets(columnCells);

            changesMade |= HandleHiddenTriplets(columnCells);
        }

        return changesMade;
    }

    public override int Order { get; } = 7;
}
