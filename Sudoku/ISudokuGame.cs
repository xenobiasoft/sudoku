namespace XenobiaSoft.Sudoku;

public interface ISudokuGame
{
	void LoadPuzzle(Cell[] puzzle);
	void Reset();
	void SetCell(int row, int col, int value);
	void SolvePuzzle();
	void Undo();

	int Score { get; }
	Cell[] Puzzle { get; }
}