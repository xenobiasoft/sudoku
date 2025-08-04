using Sudoku.Domain.Entities;

namespace Sudoku.Infrastructure.Services.Strategies;

public abstract class SolverStrategy
{
	public virtual bool SolvePuzzle(SudokuPuzzle puzzle)
	{
        puzzle.PopulatePossibleValues();

        return Execute(puzzle);
	}

	public abstract bool Execute(SudokuPuzzle puzzle);
}