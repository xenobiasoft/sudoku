namespace XenobiaSoft.Sudoku;

public interface ISudokuGame
{
	Task LoadPuzzle(ISudokuPuzzle puzzle);
	Task New(Level level);
	Task Reset();
	void SetCell(int row, int col, int value);
	Task SolvePuzzle();

	ISudokuPuzzle Puzzle { get; }
}