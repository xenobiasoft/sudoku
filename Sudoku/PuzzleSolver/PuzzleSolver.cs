using XenobiaSoft.Sudoku.Strategies;

namespace XenobiaSoft.Sudoku.PuzzleSolver;

public class PuzzleSolver : IPuzzleSolver
{
    private readonly IEnumerable<SolverStrategy> _strategies;

    public PuzzleSolver(IEnumerable<SolverStrategy> strategies)
    {
	    _strategies = strategies;
    }

    public int TrySolvePuzzle(Cell[] cells)
    {
	    var score = 0;
        var changesMade = true;

        while (changesMade)
        {
	        var previousScore = score;

	        foreach (var strategy in _strategies)
            {
				Console.WriteLine($"Solving with {strategy.GetType().Name}");
                score += strategy.SolvePuzzle(cells);
                changesMade = previousScore != score;
                previousScore = score;

                if (!cells.IsValid())
                {
					Console.WriteLine($"Failure in solving puzzle using {strategy.GetType().Name} strategy");
	                throw new InvalidOperationException();
                }

                if (IsSolved(cells))
                {
                    return score;
                }
            }
        }

        return score;
    }

    public bool IsSolved(Cell[] cells)
    {
	    foreach (var cell in cells)
	    {
		    var pattern = cells.GetColumnCells(cell.Column).Aggregate("123456789", (current, columnCell) => current.Replace(columnCell.Value.GetValueOrDefault().ToString(), string.Empty));

		    if (pattern.Length > 0)
		    {
			    return false;
		    }

		    pattern = cells.GetRowCells(cell.Row).Aggregate("123456789", (current, rowCell) => current.Replace(rowCell.Value.GetValueOrDefault().ToString(), string.Empty));

		    if (pattern.Length > 0)
			{
				return false;
			}

		    pattern = cells.GetMiniGridCells(cell.Row, cell.Column).Aggregate("123456789", (current, gridCell) => current.Replace(gridCell.Value.GetValueOrDefault().ToString(), string.Empty));

		    if (pattern.Length > 0)
			{
				return false;
			}
		}

	    return true;
	}
}