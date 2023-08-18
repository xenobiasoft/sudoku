using System.Text;

namespace XenobiaSoft.Sudoku.Strategies;

public class TripletsInColumnsStrategy : SolverStrategy
{
	public override void Execute(SudokuPuzzle puzzle)
	{
		for (var triplet1Col = 0; triplet1Col < SudokuPuzzle.Columns; triplet1Col++)
		{
			for (var triplet1Row = 0; triplet1Row < SudokuPuzzle.Rows; triplet1Row++)
			{
				if (puzzle.Values[triplet1Col, triplet1Row] != 0 || puzzle.PossibleValues[triplet1Col, triplet1Row].Length != 3) continue;

				var tripletsLocation = new StringBuilder();

				tripletsLocation.Append(triplet1Col).Append(triplet1Row);

				for (var triplet2Row = triplet1Row + 1; triplet2Row < SudokuPuzzle.Rows; triplet2Row++)
				{
					if (puzzle.PossibleValues[triplet1Col, triplet1Row] == puzzle.PossibleValues[triplet1Col, triplet2Row] ||
					    (puzzle.PossibleValues[triplet1Col, triplet2Row].Length == 2 &&
					     puzzle.PossibleValues[triplet1Col, triplet1Row].Contains(puzzle.PossibleValues[triplet1Col, triplet2Row][0].ToString()) &&
					     puzzle.PossibleValues[triplet1Col, triplet1Row].Contains(puzzle.PossibleValues[triplet1Col, triplet2Row][1].ToString()))
					    )
					{
						tripletsLocation.Append(triplet1Col).Append(triplet2Row);
					}
				}

				if (tripletsLocation.Length != 6) continue;

				for (var nonTripletRow = 0; nonTripletRow < SudokuPuzzle.Rows; nonTripletRow++)
				{
					if (puzzle.Values[triplet1Col, nonTripletRow] != 0 ||
					    nonTripletRow == int.Parse(tripletsLocation[1].ToString()) ||
					    nonTripletRow == int.Parse(tripletsLocation[3].ToString()) ||
					    nonTripletRow == int.Parse(tripletsLocation[5].ToString())) continue;

					puzzle.PossibleValues[triplet1Col, nonTripletRow] = puzzle.PossibleValues[triplet1Col, nonTripletRow].Replace(puzzle.PossibleValues[triplet1Col, triplet1Row][0].ToString(), string.Empty);
					puzzle.PossibleValues[triplet1Col, nonTripletRow] = puzzle.PossibleValues[triplet1Col, nonTripletRow].Replace(puzzle.PossibleValues[triplet1Col, triplet1Row][1].ToString(), string.Empty);
					puzzle.PossibleValues[triplet1Col, nonTripletRow] = puzzle.PossibleValues[triplet1Col, nonTripletRow].Replace(puzzle.PossibleValues[triplet1Col, triplet1Row][2].ToString(), string.Empty);

					if (puzzle.PossibleValues[triplet1Col, nonTripletRow].Length != 1) continue;

					puzzle.Values[triplet1Col, nonTripletRow] = int.Parse(puzzle.PossibleValues[triplet1Col, nonTripletRow]);
				}
			}
		}
	}
}