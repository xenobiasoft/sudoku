namespace XenobiaSoft.Sudoku.Helpers;

public static class SudokuPuzzleExtensionMethods
{
    public static SudokuPuzzle PopulatePossibleValues(this SudokuPuzzle puzzle)
    {
        for (var row = 0; row < SudokuPuzzle.Rows; row++)
        {
            for (var col = 0; col < SudokuPuzzle.Columns; col++)
            {
                puzzle.PossibleValues[col, row] = puzzle.Values[col, row] == 0 ?
                    CalculatePossibleValues(puzzle, col, row) :
                    string.Empty;
            }
        }

        return puzzle;
    }

    public static Tuple<int, int> FindCellWithFewestPossibleValues(this SudokuPuzzle puzzle)
    {
        var minValue = 10;
        var minCol = 0;
        var minRow = 0;

        for (var col = 0; col < SudokuPuzzle.Columns; col++)
        {
            for (var row = 0; row < SudokuPuzzle.Rows; row++)
            {
                if (puzzle.Values[col, row] != 0 ||
                    puzzle.PossibleValues[col, row].Length >= minValue ||
                    puzzle.PossibleValues[col, row].Length == 0) continue;

                minValue = puzzle.PossibleValues[col, row].Length;
                minCol = col;
                minRow = row;
            }
        }

        return new Tuple<int, int>(minCol, minRow);
    }

    public static void SetCellWithFewestPossibleValues(this SudokuPuzzle puzzle)
    {
        var cell = puzzle.FindCellWithFewestPossibleValues();
        var possibleValues = puzzle.PossibleValues[cell.Item1, cell.Item2].Randomize();

        if (string.IsNullOrWhiteSpace(possibleValues))
        {
            throw new InvalidOperationException("An invalid move was made");
        }

        puzzle.Values[cell.Item1, cell.Item2] = int.Parse(possibleValues[0].ToString());
    }

    public static void Reset(this SudokuPuzzle puzzle)
    {
	    Array.Clear(puzzle.PossibleValues);
	    Array.Clear(puzzle.Values);
    }

    private static string CalculatePossibleValues(SudokuPuzzle puzzle, int col, int row)
    {
        var possible = puzzle.PossibleValues;
        var values = puzzle.Values;
        var possibleValues = string.IsNullOrWhiteSpace(possible[col, row]) ? "123456789" : possible[col, row];

        for (var r = 0; r < SudokuPuzzle.Rows; r++)
        {
            if (values[col, r] != 0)
            {
                possibleValues = possibleValues.Replace(values[col, r].ToString(), string.Empty);
            }
        }

        for (var c = 0; c < SudokuPuzzle.Columns; c++)
        {
            if (values[c, row] != 0)
            {
                possibleValues = possibleValues.Replace(values[c, row].ToString(), string.Empty);
            }
        }

        var miniGridStartCol = PuzzleHelper.CalculateMiniGridStartCol(col);
        var miniGridStartRow = PuzzleHelper.CalculateMiniGridStartCol(row);

        for (var miniGridRow = miniGridStartRow; miniGridRow <= miniGridStartRow + 2; miniGridRow++)
        {
            for (var miniGridCol = miniGridStartCol; miniGridCol <= miniGridStartCol + 2; miniGridCol++)
            {
                if (values[miniGridCol, miniGridRow] != 0)
                {
                    possibleValues = possibleValues.Replace(values[miniGridCol, miniGridRow].ToString(), string.Empty);
                }
            }
        }

        return possibleValues;
    }
}