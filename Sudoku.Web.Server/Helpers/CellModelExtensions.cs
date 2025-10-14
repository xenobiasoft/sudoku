using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Helpers;

public static class CellModelExtensions
{
    public static bool IsInSameMiniGrid(this CellModel cell1, CellModel cell2)
    {
        var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(cell2.Column);
        var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartRow(cell2.Row);

        return cell1.Row >= miniGridStartRow &&
               cell1.Row < miniGridStartRow + 3 &&
               cell1.Column >= miniGridStartCol &&
               cell1.Column < miniGridStartCol + 3;
    }
}