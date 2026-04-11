using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Services.Strategies;

public class TwinsInMiniGridsStrategy : TwinsStrategyBase
{
	public override bool Execute(SudokuPuzzle puzzle)
    {
        var miniGrids = new List<IEnumerable<Cell>>();

        for (var startRow = 0; startRow < 9; startRow += 3)
        {
            for (var startCol = 0; startCol < 9; startCol += 3)
            {
                var cells = puzzle.GetMiniGridCells(startRow, startCol);

                miniGrids.Add(cells);
            }
        }

        return HandleNakedTwins(miniGrids);
    }

    public override int Order { get; } = 6;
}