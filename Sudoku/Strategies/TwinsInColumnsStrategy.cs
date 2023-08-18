namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInColumnsStrategy : SolverStrategy
{
	public override void Execute(SudokuPuzzle puzzle)
	{
		for (var col = 0; col < SudokuPuzzle.Columns; col++)
		{
			for (var row = 0; row < SudokuPuzzle.Rows; row++)
			{
				if (puzzle.Values[col, row] != 0 || puzzle.PossibleValues[col, row].Length != 2) continue;

				for (var nextRow = row + 1; nextRow < 9; nextRow++)
				{
					if (puzzle.PossibleValues[col, nextRow] != puzzle.PossibleValues[col, row]) continue;

					for (var rrr = 0; rrr < 9; rrr++)
					{
						if (puzzle.Values[col, rrr] != 0 || rrr == row || rrr == nextRow) continue;

						puzzle.PossibleValues[col, rrr] = puzzle.PossibleValues[col, rrr].Replace(puzzle.PossibleValues[col, row][0].ToString(), string.Empty);
						puzzle.PossibleValues[col, rrr] = puzzle.PossibleValues[col, rrr].Replace(puzzle.PossibleValues[col, row][1].ToString(), string.Empty);

						if (puzzle.PossibleValues[col, rrr].Length != 1) continue;

						puzzle.Values[col, rrr] = int.Parse(puzzle.PossibleValues[col, rrr]);
					}
				}
			}
		}
	}
}