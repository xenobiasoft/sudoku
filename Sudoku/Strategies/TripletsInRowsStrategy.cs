using System.Text;

namespace XenobiaSoft.Sudoku.Strategies;

public class TripletsInRowsStrategy : SolverStrategy
{
	public override void Execute(SudokuPuzzle puzzle)
	{
		var tripletsLocation = new StringBuilder();

		for (var triplet1Row = 0; triplet1Row < SudokuPuzzle.Rows; triplet1Row++)
		{
			for (var triplet1Col = 0; triplet1Col < SudokuPuzzle.Columns; triplet1Col++)
			{
				if (puzzle.Values[triplet1Col, triplet1Row] != 0 || puzzle.PossibleValues[triplet1Col, triplet1Row].Length != 3) continue;

				tripletsLocation.Append(triplet1Col).Append(triplet1Row);

				for (var triplet2Col = 0; triplet2Col < SudokuPuzzle.Columns; triplet2Col++)
				{
					if (triplet2Col != triplet1Col &&
					    (
						    puzzle.PossibleValues[triplet2Col, triplet1Row] == puzzle.PossibleValues[triplet1Col, triplet1Row] ||
						    (puzzle.PossibleValues[triplet2Col, triplet1Row].Length == 2 &&
						     puzzle.PossibleValues[triplet1Col, triplet1Row].Contains(puzzle.PossibleValues[triplet2Col, triplet1Row][0].ToString()) &&
						     puzzle.PossibleValues[triplet1Col, triplet1Row].Contains(puzzle.PossibleValues[triplet2Col, triplet1Row][1].ToString()))
					    ))
					{
						tripletsLocation.Append(triplet2Col).Append(triplet1Row);
					}
				}

				if (tripletsLocation.Length != 6) continue;

				for (var nonTripletCol = 0; nonTripletCol < SudokuPuzzle.Columns; nonTripletCol++)
				{
					if (puzzle.Values[nonTripletCol, triplet1Row] != 0 ||
					    nonTripletCol == int.Parse(tripletsLocation[0].ToString()) ||
					    nonTripletCol == int.Parse(tripletsLocation[2].ToString()) ||
					    nonTripletCol == int.Parse(tripletsLocation[4].ToString())) continue;

					puzzle.PossibleValues[nonTripletCol, triplet1Row] = puzzle.PossibleValues[nonTripletCol, triplet1Row].Replace(puzzle.PossibleValues[triplet1Col, triplet1Row][0].ToString(), string.Empty);
					puzzle.PossibleValues[nonTripletCol, triplet1Row] = puzzle.PossibleValues[nonTripletCol, triplet1Row].Replace(puzzle.PossibleValues[triplet1Col, triplet1Row][1].ToString(), string.Empty);
					puzzle.PossibleValues[nonTripletCol, triplet1Row] = puzzle.PossibleValues[nonTripletCol, triplet1Row].Replace(puzzle.PossibleValues[triplet1Col, triplet1Row][2].ToString(), string.Empty);

					if (puzzle.PossibleValues[nonTripletCol, triplet1Row].Length != 1) continue;

					puzzle.Values[nonTripletCol, triplet1Row] = int.Parse(puzzle.PossibleValues[nonTripletCol, triplet1Row]);
				}
			}
		}
	}
}