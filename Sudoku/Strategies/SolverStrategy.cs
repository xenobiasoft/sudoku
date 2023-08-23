using XenobiaSoft.Sudoku.Helpers;

namespace XenobiaSoft.Sudoku.Strategies;

public abstract class SolverStrategy
{
	public int SolvePuzzle(SudokuPuzzle puzzle)
	{
		puzzle.PopulatePossibleValues();

		return Execute(puzzle);
	}

	public abstract int Execute(SudokuPuzzle puzzle);
}