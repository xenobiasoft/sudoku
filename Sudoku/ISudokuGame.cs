namespace XenobiaSoft.Sudoku;

public interface ISudokuGame
{
	void SaveGameState();
	void Undo();
	void LoadPuzzle(SudokuPuzzle puzzle);
	void SolvePuzzle();

	SudokuPuzzle Puzzle { get; set; }
	int Score { get; set; }
	void SetCell(int col, int row, int value);
}