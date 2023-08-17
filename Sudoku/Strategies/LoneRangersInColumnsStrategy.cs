namespace XenobiaSoft.Sudoku.Strategies;

public class LoneRangersInColumnsStrategy : SolverStrategy
{
	public override void Execute(SudokuPuzzle puzzle)
	{
		var colPos = 0;
		var rowPos = 0;

		for (var col = 0; col < SudokuPuzzle.Columns; col++)
		{
			for (var number = 1; number <= 9; number++)
			{
				var occurrence = 0;

				for (var row = 0; row < SudokuPuzzle.Rows; row++)
				{
					if (puzzle.Values[col, row] != 0 || !puzzle.PossibleValues[col, row].Contains(number.ToString())) continue;

					occurrence += 1;

					if (occurrence > 1)
					{
						break;
					}

					colPos = col;
					rowPos = row;
				}

				if (occurrence != 1) continue;

				puzzle.Values[colPos, rowPos] = number;
			}
		}
	}
}