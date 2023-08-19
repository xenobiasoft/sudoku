namespace XenobiaSoft.Sudoku.Strategies;

public class LoneRangersInMiniGridsStrategy : SolverStrategy
{
	private const int Score = 2;

	public override int Execute(SudokuPuzzle puzzle)
	{
		var colPos = 0;
		var rowPos = 0;
		var changed = false;

		for (var number = 1; number <= 9; number++)
		{
			for (var row = 0; row < SudokuPuzzle.Rows; row += 3)
			{
				for (var col = 0; col < SudokuPuzzle.Columns; col += 3)
				{
					var nextMiniGrid = false;
					var occurrence = 0;

					for (var miniGridRow = 0; miniGridRow <= 2; miniGridRow++)
					{
						for (var miniGridCol = 0; miniGridCol <= 2; miniGridCol++)
						{
							if (puzzle.Values[col + miniGridCol, row + miniGridRow] != 0 || 
							    !puzzle.PossibleValues[col + miniGridCol, row + miniGridRow].Contains(number.ToString())) continue;

							occurrence += 1;
							colPos = col + miniGridCol;
							rowPos = row + miniGridRow;

							if (occurrence <= 1) continue;

							nextMiniGrid = true;
							break;
						}

						if (nextMiniGrid)
						{
							break;
						}
					}

					if (nextMiniGrid || occurrence != 1) continue;

					puzzle.Values[colPos, rowPos] = number;
					puzzle.PossibleValues[colPos, rowPos] = number.ToString();

					changed = true;
				}
			}
		}

		return changed ? Score : 0;
	}
}