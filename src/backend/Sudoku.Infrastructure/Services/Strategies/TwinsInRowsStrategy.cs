using Sudoku.Domain.Entities;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TwinsInRowsStrategy : TwinsStrategyBase
{
	public override bool Execute(SudokuPuzzle puzzle)
    {
        var rowGroups = Enumerable.Range(0, 9).Select(puzzle.GetRowCells);

        return HandleNakedTwins(rowGroups);
    }

    public override int Order { get; } = 5;
}