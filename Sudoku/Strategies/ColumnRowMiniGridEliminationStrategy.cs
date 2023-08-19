﻿namespace XenobiaSoft.Sudoku.Strategies;

public class ColumnRowMiniGridEliminationStrategy : SolverStrategy
{
	private const int Score = 1;

	public override int Execute(SudokuPuzzle puzzle)
	{
		var changed = false;

		for (var row = 0; row < SudokuPuzzle.Rows; row++)
		{
			for (var col = 0; col < SudokuPuzzle.Columns; col++)
			{
				if (puzzle.Values[col, row] != 0 || puzzle.PossibleValues[col, row].Length != 1) continue;

				puzzle.Values[col, row] = int.Parse(puzzle.PossibleValues[col, row]);

				changed = true;
			}
		}

		return changed ? Score : 0;
	}
}