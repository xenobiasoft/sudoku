namespace XenobiaSoft.Sudoku.Solver;

public interface IPuzzleSolver
{
	Task<Cell[]> SolvePuzzle(Cell[] cells);
}