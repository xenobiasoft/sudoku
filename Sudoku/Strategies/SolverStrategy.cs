namespace XenobiaSoft.Sudoku.Strategies;

public abstract class SolverStrategy
{
	public int SolvePuzzle(ISudokuPuzzle puzzle)
	{
		puzzle.PopulatePossibleValues();

		return Execute(puzzle);
	}

	public abstract int Execute(ISudokuPuzzle puzzle);
}