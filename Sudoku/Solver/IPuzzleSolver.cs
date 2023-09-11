namespace XenobiaSoft.Sudoku.Solver;

public interface IPuzzleSolver
{
	int TrySolvePuzzle(Cell[] cells);
	bool IsSolved(Cell[] cells);
}