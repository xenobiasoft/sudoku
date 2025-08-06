namespace XenobiaSoft.Sudoku.Strategies;

[Obsolete("This class is obsolete. Use Sudoku.Infrastructure.Services.Strategies.SolverStrategy instead.")]
public abstract class SolverStrategy
{
	public virtual bool SolvePuzzle(ISudokuPuzzle puzzle)
	{
		puzzle.PopulatePossibleValues();

		return Execute(puzzle);
	}

	public abstract bool Execute(ISudokuPuzzle puzzle);
}