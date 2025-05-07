namespace XenobiaSoft.Sudoku.Strategies;

public abstract class SolverStrategy
{
	public virtual bool SolvePuzzle(ISudokuPuzzle puzzle)
	{
		puzzle.PopulatePossibleValues();

		return Execute(puzzle);
	}

	public abstract bool Execute(ISudokuPuzzle puzzle);
}