namespace XenobiaSoft.Sudoku.PuzzleSolver;

public interface IPuzzleSolver
{
	int TrySolvePuzzle(Cell[] cells);
	bool IsSolved(Cell[] cells);
	bool IsValid(Cell[] cells);
}