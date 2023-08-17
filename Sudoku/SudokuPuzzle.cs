namespace XenobiaSoft.Sudoku;

public class SudokuPuzzle
{
	public const int Rows = 9;
	public const int Columns = 9;

	public SudokuPuzzle()
	{
		Values = new int[9, 9];
		PossibleValues = new string[9, 9];
	}

	public int[,] Values { get; set; }
	public string[,] PossibleValues { get; set; }
	public int TotalScore { get; set; }
}