using XenobiaSoft.Sudoku.Helpers;

namespace XenobiaSoft.Sudoku.Strategies;

public class TwinsInMiniGridsStrategy : SolverStrategy
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

				var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(twin1Col);
				var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartRow(twin1Row);

				for (var twin2Row = miniGridStartRow; twin2Row < miniGridStartRow + 2; twin2Row++)
				{
					for (var twin2Col = miniGridStartCol; twin2Col < miniGridStartCol + 2; twin2Col++)
					{
						if (twin1Col == twin2Col && twin1Row == twin2Row || puzzle.PossibleValues[twin1Col, twin1Row] != puzzle.PossibleValues[twin2Col, twin2Row]) continue;

						for (var nonTwinRow = miniGridStartRow; nonTwinRow < miniGridStartRow + 3; nonTwinRow++)
						{
							for (var nonTwinCol = miniGridStartCol; nonTwinCol < miniGridStartCol + 3; nonTwinCol++)
							{
								if (puzzle.Values[nonTwinCol, nonTwinRow] != 0 || puzzle.PossibleValues[nonTwinCol, nonTwinRow] == puzzle.PossibleValues[twin1Col, twin1Row]) continue;

								puzzle.PossibleValues[nonTwinCol, nonTwinRow] = puzzle.PossibleValues[nonTwinCol, nonTwinRow].Replace(puzzle.PossibleValues[twin1Col, twin1Row][0].ToString(), string.Empty);
								puzzle.PossibleValues[nonTwinCol, nonTwinRow] = puzzle.PossibleValues[nonTwinCol, nonTwinRow].Replace(puzzle.PossibleValues[twin1Col, twin1Row][1].ToString(), string.Empty);

								if (string.IsNullOrWhiteSpace(puzzle.PossibleValues[nonTwinCol, nonTwinRow]))
								{
									throw new InvalidOperationException("An invalid move was made");
								}

								if (puzzle.PossibleValues[nonTwinCol, nonTwinRow].Length != 1) continue;

								puzzle.Values[nonTwinCol, nonTwinRow] = int.Parse(puzzle.PossibleValues[nonTwinCol, nonTwinRow]);

								changed = true;
							}
						}
					}
				}
			}
		}

		return changed ? Score : 0;
	}
}