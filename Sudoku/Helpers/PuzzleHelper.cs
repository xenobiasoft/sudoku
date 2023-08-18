namespace XenobiaSoft.Sudoku.Helpers;

public class PuzzleHelper
{
	public static int CalculateMiniGridStartCol(int col)
	{
		return CalculateMiniGridStart(col);
	}

	public static int CalculateMiniGridStartRow(int row)
	{
		return CalculateMiniGridStart(row);
	}

	private static int CalculateMiniGridStart(int num)
	{
		return num + 1 - num % 3 - 1;
	}
}