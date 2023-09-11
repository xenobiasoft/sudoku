namespace XenobiaSoft.Sudoku.Solver;

public interface IPuzzleSolver
{
	Task<int> TrySolvePuzzle(Cell[] cells);
	bool IsSolved(Cell[] cells);
}