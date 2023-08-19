namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInRowsStrategy : SolverStrategy
{
	private const int Score = 3;

	public override int Execute(SudokuPuzzle puzzle)
	{
		var changed = false;

		for (var twin1Row = 0; twin1Row < SudokuPuzzle.Rows; twin1Row++)
		{
			for (var twin1Col = 0; twin1Col < SudokuPuzzle.Columns; twin1Col++)
			{
				if (puzzle.Values[twin1Col, twin1Row] != 0 || puzzle.PossibleValues[twin1Col, twin1Row].Length != 2) continue;

				for (var twin2Col = twin1Col + 1; twin2Col < SudokuPuzzle.Columns; twin2Col++)
				{
					if (puzzle.PossibleValues[twin2Col, twin1Row] != puzzle.PossibleValues[twin1Col, twin1Row]) continue;

					for (var nonTwinCol = 0; nonTwinCol < SudokuPuzzle.Columns; nonTwinCol++)
					{
						if (puzzle.Values[nonTwinCol, twin1Row] != 0 || nonTwinCol == twin1Col || nonTwinCol == twin2Col) continue;

						puzzle.PossibleValues[nonTwinCol, twin1Row] = puzzle.PossibleValues[nonTwinCol, twin1Row].Replace(puzzle.PossibleValues[twin1Col, twin1Row][0].ToString(), string.Empty);
						puzzle.PossibleValues[nonTwinCol, twin1Row] = puzzle.PossibleValues[nonTwinCol, twin1Row].Replace(puzzle.PossibleValues[twin1Col, twin1Row][1].ToString(), string.Empty);

						if (puzzle.PossibleValues[nonTwinCol, twin1Row].Length != 1) continue;

						puzzle.Values[nonTwinCol, twin1Row] = int.Parse(puzzle.PossibleValues[nonTwinCol, twin1Row]);

						changed = true;
					}
				}
			}
		}

		return changed ? Score : 0;
	}
}