namespace Sudoku.Web.Server.Helpers;

public static class CellExtensionMethods
{
    public static bool IsInSameMiniGrid(this Cell cell1, Cell cell2)
    {
        var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(cell2.Column);
        var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartRow(cell2.Row);

        return cell1.Row >= miniGridStartRow &&
               cell1.Row < miniGridStartRow + 3 &&
               cell1.Column >= miniGridStartCol &&
               cell1.Column < miniGridStartCol + 3;
    }
}