namespace XenobiaSoft.Sudoku;

public interface ISudokuGame
{
	void Reset();
	void Restore(Cell[] cells);
	void SaveGameState();
	void SetCell(int row, int col, int value);
	void SolvePuzzle();
	void Undo();

	int Score { get; set; }
	Cell[] Puzzle { get; set; }
}