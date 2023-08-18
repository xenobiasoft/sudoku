namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInRowsStrategy : SolverStrategy
{
	public override void Execute(SudokuPuzzle puzzle)
	{
		for (var row = 0; row < SudokuPuzzle.Rows; row++)
		{
			for (var col = 0; col < SudokuPuzzle.Columns; col++)
			{
				if (puzzle.Values[col, row] != 0 || puzzle.PossibleValues[col, row].Length != 2) continue;

				for (var nextCol = col + 1; nextCol < SudokuPuzzle.Columns; nextCol++)
				{
					if (puzzle.PossibleValues[nextCol, row] != puzzle.PossibleValues[col, row]) continue;

					for (var nonTwinCol = 0; nonTwinCol < SudokuPuzzle.Columns; nonTwinCol++)
					{
						if (puzzle.Values[nonTwinCol, row] != 0 || nonTwinCol == col || nonTwinCol == nextCol) continue;

						puzzle.PossibleValues[nonTwinCol, row] = puzzle.PossibleValues[nonTwinCol, row].Replace(puzzle.PossibleValues[col, row][0].ToString(), string.Empty);
						puzzle.PossibleValues[nonTwinCol, row] = puzzle.PossibleValues[nonTwinCol, row].Replace(puzzle.PossibleValues[col, row][1].ToString(), string.Empty);

						if (puzzle.PossibleValues[nonTwinCol, row].Length != 1) continue;

						puzzle.Values[nonTwinCol, row] = int.Parse(puzzle.PossibleValues[nonTwinCol, row]);
					}
				}
			}
		}
	}
}