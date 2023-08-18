namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInMiniGridsStrategy : SolverStrategy
{
	public override void Execute(SudokuPuzzle puzzle)
	{
		for (var row = 0; row < SudokuPuzzle.Rows; row++)
		{
			for (var col = 0; col < SudokuPuzzle.Columns; col++)
			{
				if (puzzle.Values[col, row] != 0 || puzzle.PossibleValues[col, row].Length != 2) continue;

				var miniGridStartCol = col - (col - 1) % 3;
				var miniGridStartRow = row - (row - 1) % 3;

				for (var miniGridRow = miniGridStartRow; miniGridRow < miniGridStartRow + 2; miniGridRow++)
				{
					for (var miniGridColumn = miniGridStartCol; miniGridColumn < miniGridStartCol + 2; miniGridColumn++)
					{
						if (miniGridColumn == col && miniGridRow == row || puzzle.PossibleValues[miniGridColumn, miniGridRow] != puzzle.PossibleValues[col, row]) continue;

						for (var twinsRow = miniGridStartRow; twinsRow < miniGridStartRow + 2; twinsRow++)
						{
							for (var twinsCol = miniGridStartCol; twinsCol < miniGridStartCol + 2; twinsCol++)
							{
								if (puzzle.Values[twinsCol, twinsRow] != 0 || puzzle.PossibleValues[twinsCol, twinsRow] == puzzle.PossibleValues[col, row]) continue;

								puzzle.PossibleValues[twinsCol, twinsRow] = puzzle.PossibleValues[twinsCol, twinsRow].Replace(puzzle.PossibleValues[col, row][0].ToString(), string.Empty);
								puzzle.PossibleValues[twinsCol, twinsRow] = puzzle.PossibleValues[twinsCol, twinsRow].Replace(puzzle.PossibleValues[col, row][1].ToString(), string.Empty);

								if (puzzle.PossibleValues[twinsCol, twinsRow].Length == 1)
								{
									puzzle.Values[twinsCol, twinsRow] = int.Parse(puzzle.PossibleValues[twinsCol, twinsRow]);
								}
							}
						}
					}
				}
			}
		}
	}
}