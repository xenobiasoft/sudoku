namespace XenobiaSoft.Sudoku;

public interface ISudokuGame
{
	void LoadPuzzle(Cell[] puzzle);
	Task New(Level level);
	void Reset();
	void SetCell(int row, int col, int value);
	Task SolvePuzzle();
	void Undo();

	int Score { get; }
	Cell[] Puzzle { get; }
}