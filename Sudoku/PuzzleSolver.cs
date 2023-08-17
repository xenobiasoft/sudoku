using XenobiaSoft.Sudoku.Strategies;

namespace XenobiaSoft.Sudoku;

public class PuzzleSolver
{
	private readonly IEnumerable<SolverStrategy> _strategies;

	public PuzzleSolver(IEnumerable<SolverStrategy> strategies)
	{
		_strategies = strategies;
	}

	public void SolvePuzzle(SudokuPuzzle puzzle)
	{
		foreach (var strategy in _strategies)
		{
			strategy.SolvePuzzle(puzzle);
		}
	}
}