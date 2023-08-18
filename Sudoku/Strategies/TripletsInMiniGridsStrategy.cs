using System.Text;

namespace XenobiaSoft.Sudoku.Strategies;

public class TripletsInMiniGridsStrategy : SolverStrategy
{
	public override void Execute(SudokuPuzzle puzzle)
	{
		var tripletsLocation = new StringBuilder();

		for (var row = 0; row < SudokuPuzzle.Rows; row++)
		{
			for (var col = 0; col < SudokuPuzzle.Columns; col++)
			{
				if (puzzle.Values[col, row] != 0 || puzzle.PossibleValues[col, row].Length != 3) continue;

				tripletsLocation.Append(col).Append(row);

				var startC = col - (col - 1) % 3;
				var startR = row - (row - 1) % 3;

				for (var rr = startR; rr < startR + 2; rr++)
				{
					for (var cc = startC; cc < startC + 2; cc++)
					{
						if (!(cc == col && rr == row) &&
						    (
							    (puzzle.PossibleValues[cc, rr] == puzzle.PossibleValues[col, row]) ||
							    (puzzle.PossibleValues[cc, rr].Length == 2 &&
							     puzzle.PossibleValues[col, row].Contains(puzzle.PossibleValues[cc, rr][0].ToString()) &&
							     puzzle.PossibleValues[col, row].Contains(puzzle.PossibleValues[cc, rr][1].ToString()))
						    ))
						{
							tripletsLocation.Append(cc).Append(rr);
						}
					}
				}

				if (tripletsLocation.Length != 6) continue;

				for (var rrr = startR; rrr < startR + 2; rrr++)
				{
					for (var ccc = startC; ccc < startC + 2; ccc++)
					{
						if (puzzle.Values[ccc, rrr] != 0 ||
						    ccc == Convert.ToInt32(tripletsLocation[0].ToString()) ||
						    rrr == Convert.ToInt32(tripletsLocation[1].ToString()) ||
						    ccc == Convert.ToInt32(tripletsLocation[2].ToString()) ||
						    rrr == Convert.ToInt32(tripletsLocation[3].ToString()) ||
						    ccc == Convert.ToInt32(tripletsLocation[4].ToString()) ||
						    rrr == Convert.ToInt32(tripletsLocation[5].ToString())) continue;

						puzzle.PossibleValues[ccc, rrr] = puzzle.PossibleValues[ccc, rrr].Replace(puzzle.PossibleValues[col, row][0].ToString(), string.Empty);
						puzzle.PossibleValues[ccc, rrr] = puzzle.PossibleValues[ccc, rrr].Replace(puzzle.PossibleValues[col, row][1].ToString(), string.Empty);
						puzzle.PossibleValues[ccc, rrr] = puzzle.PossibleValues[ccc, rrr].Replace(puzzle.PossibleValues[col, row][2].ToString(), string.Empty);

						if (puzzle.PossibleValues[ccc, rrr].Length != 1) continue;

						puzzle.Values[ccc, rrr] = int.Parse(puzzle.PossibleValues[ccc, rrr]);
					}
				}
			}
		}
	}
}