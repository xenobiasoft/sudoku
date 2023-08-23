namespace XenobiaSoft.Sudoku;

public interface ISudokuGame
{
	void LoadPuzzle(SudokuPuzzle puzzle);
	void Reset();
	void SaveGameState();
	void SetCell(int col, int row, int value);
	void SolvePuzzle();
	void Undo();

	SudokuPuzzle Puzzle { get; set; }
	int Score { get; set; }
}