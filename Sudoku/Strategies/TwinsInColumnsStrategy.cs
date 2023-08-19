namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInColumnsStrategy : SolverStrategy
{
	private const int Score = 3;

	public override int Execute(SudokuPuzzle puzzle)
	{
		var changed = false;

		for (var twin1Col = 0; twin1Col < SudokuPuzzle.Columns; twin1Col++)
		{
			for (var twin1Row = 0; twin1Row < SudokuPuzzle.Rows; twin1Row++)
			{
				if (puzzle.Values[twin1Col, twin1Row] != 0 || puzzle.PossibleValues[twin1Col, twin1Row].Length != 2) continue;

				for (var twin2Row = twin1Row + 1; twin2Row < SudokuPuzzle.Rows; twin2Row++)
				{
					if (puzzle.PossibleValues[twin1Col, twin1Row] != puzzle.PossibleValues[twin1Col, twin2Row]) continue;

					for (var nonTwinRow = 0; nonTwinRow < SudokuPuzzle.Rows; nonTwinRow++)
					{
						if (puzzle.Values[twin1Col, nonTwinRow] != 0 || nonTwinRow == twin1Row || nonTwinRow == twin2Row) continue;

						puzzle.PossibleValues[twin1Col, nonTwinRow] = puzzle.PossibleValues[twin1Col, nonTwinRow].Replace(puzzle.PossibleValues[twin1Col, twin1Row][0].ToString(), string.Empty);
						puzzle.PossibleValues[twin1Col, nonTwinRow] = puzzle.PossibleValues[twin1Col, nonTwinRow].Replace(puzzle.PossibleValues[twin1Col, twin1Row][1].ToString(), string.Empty);

						if (puzzle.PossibleValues[twin1Col, nonTwinRow].Length != 1) continue;

						puzzle.Values[twin1Col, nonTwinRow] = int.Parse(puzzle.PossibleValues[twin1Col, nonTwinRow]);

						changed = true;
					}
				}
			}
		}

		return changed ? Score : 0;
	}
}