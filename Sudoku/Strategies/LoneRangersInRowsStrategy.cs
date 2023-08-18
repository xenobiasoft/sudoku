namespace XenobiaSoft.Sudoku.Strategies;

public class LoneRangersInRowsStrategy : SolverStrategy
{
	public override void Execute(SudokuPuzzle puzzle)
	{
		var colPos = 0;
		var rowPos = 0;

		for (var row = 0; row < SudokuPuzzle.Rows; row++)
		{
			for (var number = 1; number <= 9; number++)
			{
				var occurrence = 0;

				for (var rol = 0; rol < SudokuPuzzle.Columns; rol++)
				{
					if (puzzle.Values[rol, row] != 0 || !puzzle.PossibleValues[rol, row].Contains(number.ToString())) continue;

					occurrence += 1;

					if (occurrence > 1)
					{
						break;
					}

					colPos = rol;
					rowPos = row;
				}

				if (occurrence != 1) continue;

				puzzle.Values[colPos, rowPos] = number;
				puzzle.PossibleValues[colPos, rowPos] = number.ToString();
			}
		}
	}
}