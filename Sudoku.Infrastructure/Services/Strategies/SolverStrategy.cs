using Sudoku.Domain.Entities;

namespace XenobiaSoft.Sudoku.Infrastructure.Services.Strategies;

public abstract class SolverStrategy
{
	public virtual bool SolvePuzzle(SudokuPuzzle puzzle)
	{
        // TODO: Implement a method to populate possible values for each cell in the puzzle.
        //puzzle.PopulatePossibleValues();

        return Execute(puzzle);
	}

	public abstract bool Execute(SudokuPuzzle puzzle);
}