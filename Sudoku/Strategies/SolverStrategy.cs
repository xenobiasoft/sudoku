namespace XenobiaSoft.Sudoku.Strategies;

public abstract class SolverStrategy
{
	public int SolvePuzzle(Cell[] cells)
	{
		cells.PopulatePossibleValues();

		return Execute(cells);
	}

	public abstract int Execute(Cell[] cells);
}