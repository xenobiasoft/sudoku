namespace XenobiaSoft.Sudoku.Solver;

public interface IPuzzleSolver
{
	Task<ISudokuPuzzle> SolvePuzzle(ISudokuPuzzle puzzle);
}