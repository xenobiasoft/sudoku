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
		int previousScore;

		do
		{
			previousScore = TotalScore;

			foreach (var strategy in _strategies)
			{
				TotalScore += strategy.SolvePuzzle(puzzle);
			}
		} while (TotalScore > previousScore);
	}

	public int TotalScore { get; private set; }
}