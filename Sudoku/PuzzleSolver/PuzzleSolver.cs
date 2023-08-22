using System.Text;
using XenobiaSoft.Sudoku.Helpers;
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
	    for (var row = 0; row < SudokuPuzzle.Rows; row++)
	    {
		    var pattern = "123456789";

		    for (var col = 0; col < SudokuPuzzle.Columns; col++)
		    {
			    pattern = pattern.Replace(puzzle.Values[col, row].ToString(), string.Empty);
		    }

		    if (pattern.Length > 0)
		    {
			    return false;
		    }
	    }

	    for (var col = 0; col < SudokuPuzzle.Columns; col++)
	    {
		    var pattern = "123456789";

		    for (var row = 0; row < SudokuPuzzle.Rows; row++)
		    {
			    pattern = pattern.Replace(puzzle.Values[col, row].ToString(), string.Empty);
		    }

		    if (pattern.Length > 0)
		    {
			    return false;
		    }
	    }

	    for (var col = 0; col < SudokuPuzzle.Columns; col++)
	    {
		    for (var row = 0; row < SudokuPuzzle.Rows; row++)
		    {
			    var pattern = "123456789";
			    var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);
			    var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartRow(row);

			    for (var miniGridCol = miniGridStartCol; miniGridCol < miniGridStartCol + 3; miniGridCol++)
			    {
				    for (var miniGridRow = miniGridStartRow; miniGridRow < miniGridStartRow + 3; miniGridRow++)
				    {
					    pattern = pattern.Replace(puzzle.Values[miniGridCol, miniGridRow].ToString(), string.Empty);
				    }
			    }

			    if (pattern.Length > 0)
			    {
				    return false;
			    }
		    }
	    }

	    return true;
	}

    public bool IsValid(SudokuPuzzle puzzle)
    {
	    for (var row = 0; row < SudokuPuzzle.Rows; row++)
	    {
		    var usedNumbers = new StringBuilder();

		    for (var col = 0; col < SudokuPuzzle.Columns; col++)
		    {
				if (puzzle.Values[col, row] == 0) continue;

			    if (usedNumbers.ToString().Contains(puzzle.Values[col, row].ToString()))
			    {
				    return false;
			    }

			    usedNumbers.Append(puzzle.Values[col, row]);
		    }
	    }

	    for (var col = 0; col < SudokuPuzzle.Columns; col++)
	    {
		    var usedNumbers = new StringBuilder();

		    for (var row = 0; row < SudokuPuzzle.Rows; row++)
		    {
			    if (puzzle.Values[col, row] == 0) continue;

				if (usedNumbers.ToString().Contains(puzzle.Values[col, row].ToString()))
			    {
				    return false;
			    }

			    usedNumbers.Append(puzzle.Values[col, row]);
		    }
	    }

	    for (var col = 0; col < SudokuPuzzle.Columns; col++)
	    {
		    for (var row = 0; row < SudokuPuzzle.Rows; row++)
		    {
			    var usedNumbers = new StringBuilder();
			    var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);
			    var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartRow(row);

			    for (var miniGridCol = miniGridStartCol; miniGridCol < miniGridStartCol + 3; miniGridCol++)
			    {
				    for (var miniGridRow = miniGridStartRow; miniGridRow < miniGridStartRow + 3; miniGridRow++)
				    {
					    if (puzzle.Values[miniGridCol, miniGridRow] == 0) continue;

					    if (usedNumbers.ToString().Contains(puzzle.Values[miniGridCol, miniGridRow].ToString()))
					    {
						    return false;
					    }

					    usedNumbers.Append(puzzle.Values[miniGridCol, miniGridRow]);
				    }
			    }
		    }
	    }

	    return true;
    }
}