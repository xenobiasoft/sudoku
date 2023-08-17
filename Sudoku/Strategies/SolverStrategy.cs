namespace XenobiaSoft.Sudoku.Strategies;

public abstract class SolverStrategy
{
	public void SolvePuzzle(SudokuPuzzle puzzle)
	{
		puzzle.PopulatePossibleValues();

		Execute(puzzle);
	}

	public abstract void Execute(SudokuPuzzle puzzle);
}