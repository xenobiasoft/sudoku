namespace XenobiaSoft.Sudoku;

public class SudokuPuzzle
{
	public const int Rows = 9;
	public const int Columns = 9;

	public int[,] Values { get; set; } = new int[9, 9];
	public string[,] PossibleValues { get; set; } = new string[9, 9];
}