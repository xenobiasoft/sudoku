using XenobiaSoft.Sudoku.Strategies;

namespace XenobiaSoft.Sudoku.PuzzleSolver;

public class PuzzleSolver : IPuzzleSolver
{
    private readonly IEnumerable<SolverStrategy> _strategies;

    public PuzzleSolver(IEnumerable<SolverStrategy> strategies)
    {
	    _strategies = strategies;
    }

    public int TrySolvePuzzle(SudokuPuzzle puzzle)
    {
	    var score = 0;
        var changesMade = true;

        while (changesMade)
        {
	        var previousScore = score;

	        foreach (var strategy in _strategies)
            {
                score += strategy.SolvePuzzle(puzzle);
                changesMade = previousScore != score;
                previousScore = score;

                if (IsSolved(puzzle))
                {
                    return score;
                }
            }
        }

        return score;
    }

    public bool IsSolved(SudokuPuzzle puzzle)
    {
	    foreach (var cell in puzzle.Cells)
	    {
		    var pattern = puzzle.GetColumnCells(cell.Column).Aggregate("123456789", (current, columnCell) => current.Replace(columnCell.Value.GetValueOrDefault().ToString(), string.Empty));

		    if (pattern.Length > 0)
		    {
			    return false;
		    }

		    pattern = puzzle.GetRowCells(cell.Row).Aggregate("123456789", (current, rowCell) => current.Replace(rowCell.Value.GetValueOrDefault().ToString(), string.Empty));

		    if (pattern.Length > 0)
			{
				return false;
			}

		    pattern = puzzle.GetMiniGridCells(cell.Row, cell.Column).Aggregate("123456789", (current, gridCell) => current.Replace(gridCell.Value.GetValueOrDefault().ToString(), string.Empty));

		    if (pattern.Length > 0)
			{
				return false;
			}
		}

	    return true;
	}

    public bool IsValid(SudokuPuzzle puzzle)
    {
	    foreach (var cell in puzzle.Cells)
	    {
		    if (!cell.Value.HasValue) continue;

		    var usedNumbers = new List<int?>();

		    foreach (var colCell in puzzle.GetColumnCells(cell.Column))
		    {
			    if (!colCell.Value.HasValue) continue;

			    if (usedNumbers.Contains(colCell.Value))
			    {
				    return false;
			    }

				usedNumbers.Add(colCell.Value);
		    }

			usedNumbers.Clear();

			foreach (var rowCell in puzzle.GetRowCells(cell.Row))
			{
				if (!rowCell.Value.HasValue) continue;

				if (usedNumbers.Contains(rowCell.Value))
				{
					return false;
				}

				usedNumbers.Add(rowCell.Value);
			}

			usedNumbers.Clear();

			foreach (var miniGridCell in puzzle.GetMiniGridCells(cell.Column, cell.Row))
			{
				if (!miniGridCell.Value.HasValue) continue;

				if (usedNumbers.Contains(miniGridCell.Value))
				{
					return false;
				}

				usedNumbers.Add(miniGridCell.Value);
			}
	    }

	    return true;
    }
}