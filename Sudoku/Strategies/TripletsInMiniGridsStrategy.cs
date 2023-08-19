using System.Text;
using XenobiaSoft.Sudoku.Helpers;

namespace XenobiaSoft.Sudoku.Strategies;

public class TripletsInMiniGridsStrategy : SolverStrategy
{
	private const int Score = 4;

	public override int Execute(SudokuPuzzle puzzle)
	{
		var changed = false;

		for (var triplet1Row = 0; triplet1Row < SudokuPuzzle.Rows; triplet1Row++)
		{
			for (var triplet1Col = 0; triplet1Col < SudokuPuzzle.Columns; triplet1Col++)
			{
				if (puzzle.Values[triplet1Col, triplet1Row] != 0 || puzzle.PossibleValues[triplet1Col, triplet1Row].Length != 3) continue;
				
				var tripletsLocation = new StringBuilder();

				tripletsLocation.Append(triplet1Col).Append(triplet1Row);

				var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(triplet1Col);
				var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartRow(triplet1Row);

				for (var triplet2Row = miniGridStartRow; triplet2Row < miniGridStartRow + 2; triplet2Row++)
				{
					for (var triplet2Col = miniGridStartCol; triplet2Col < miniGridStartCol + 2; triplet2Col++)
					{
						if (!(triplet2Col == triplet1Col && triplet2Row == triplet1Row) &&
						    (
							    (puzzle.PossibleValues[triplet2Col, triplet2Row] == puzzle.PossibleValues[triplet1Col, triplet1Row]) ||
							    (puzzle.PossibleValues[triplet2Col, triplet2Row].Length == 2 &&
							     puzzle.PossibleValues[triplet1Col, triplet1Row].Contains(puzzle.PossibleValues[triplet2Col, triplet2Row][0].ToString()) &&
							     puzzle.PossibleValues[triplet1Col, triplet1Row].Contains(puzzle.PossibleValues[triplet2Col, triplet2Row][1].ToString()))
						    ))
						{
							tripletsLocation.Append(triplet2Col).Append(triplet2Row);
						}
					}
				}

				if (tripletsLocation.Length != 6) continue;

				for (var nonTripletRow = miniGridStartRow; nonTripletRow < miniGridStartRow + 3; nonTripletRow++)
				{
					for (var nonTripletCol = miniGridStartCol; nonTripletCol < miniGridStartCol + 3; nonTripletCol++)
					{
						if (puzzle.Values[nonTripletCol, nonTripletRow] != 0 ||
						    (nonTripletCol == int.Parse(tripletsLocation[0].ToString()) &&
						    nonTripletRow == int.Parse(tripletsLocation[1].ToString())) ||
						    (nonTripletCol == int.Parse(tripletsLocation[2].ToString()) &&
						    nonTripletRow == int.Parse(tripletsLocation[3].ToString())) ||
						    (nonTripletCol == int.Parse(tripletsLocation[4].ToString()) &&
						    nonTripletRow == int.Parse(tripletsLocation[5].ToString()))) continue;

						puzzle.PossibleValues[nonTripletCol, nonTripletRow] = puzzle.PossibleValues[nonTripletCol, nonTripletRow].Replace(puzzle.PossibleValues[triplet1Col, triplet1Row][0].ToString(), string.Empty);
						puzzle.PossibleValues[nonTripletCol, nonTripletRow] = puzzle.PossibleValues[nonTripletCol, nonTripletRow].Replace(puzzle.PossibleValues[triplet1Col, triplet1Row][1].ToString(), string.Empty);
						puzzle.PossibleValues[nonTripletCol, nonTripletRow] = puzzle.PossibleValues[nonTripletCol, nonTripletRow].Replace(puzzle.PossibleValues[triplet1Col, triplet1Row][2].ToString(), string.Empty);

						if (puzzle.PossibleValues[nonTripletCol, nonTripletRow].Length != 1) continue;

						puzzle.Values[nonTripletCol, nonTripletRow] = int.Parse(puzzle.PossibleValues[nonTripletCol, nonTripletRow]);

						changed = true;
					}
				}
			}
		}

		return changed ? Score : 0;
	}
}