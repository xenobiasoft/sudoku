using Sudoku.Domain.Entities;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TwinsInColumnsStrategy : TwinsStrategyBase
{
    public override bool Execute(SudokuPuzzle puzzle)
    {
        var columnGroups = Enumerable.Range(0, 9).Select(puzzle.GetColumnCells);

        return HandleNakedTwins(columnGroups);
    }

    public override int Order { get; } = 4;
}