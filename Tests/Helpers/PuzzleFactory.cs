using XenobiaSoft.Sudoku;

namespace UnitTests.Helpers;

public class PuzzleFactory
{
	public static SudokuPuzzle GetPuzzle(Level level)
	{
		var puzzle = level switch
		{
			Level.Easy => EasyPuzzle,
			Level.Medium => MediumPuzzle,
			Level.Hard => HardPuzzle,
			Level.ExtremelyHard => ExtremePuzzle,
			_ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
		};

		return new SudokuPuzzle
		{
			Values = (int[,])puzzle.Clone()
		};
	}

	public static SudokuPuzzle GetEmptyPuzzle()
	{
		return new SudokuPuzzle
		{
			Values = (int[,])DefaultPuzzle.Clone()
		};
	}

	public static SudokuPuzzle GetSolvedPuzzle()
	{
		return new SudokuPuzzle
		{
			Values = (int[,])SolvedPuzzle.Clone()
		};
	}

	private static readonly int[,] EasyPuzzle = {
		{
			5, 3, 0, 0, 7, 0, 0, 0, 0
		},
		{
			6, 0, 0, 1, 9, 5, 0, 0, 0
		},
		{
			0, 9, 8, 0, 0, 0, 0, 6, 0
		},
		{
			8, 0, 0, 0, 6, 0, 0, 0, 3
		},
		{
			4, 0, 0, 8, 0, 3, 0, 0, 1
		},
		{
			7, 0, 0, 0, 2, 0, 0, 0, 6
		},
		{
			0, 6, 0, 0, 0, 0, 2, 8, 0
		},
		{
			0, 0, 0, 4, 1, 9, 0, 0, 5
		},
		{
			0, 0, 0, 0, 8, 0, 0, 7, 9
		}
	};

	private static readonly int[,] MediumPuzzle = {
		{
			7, 0, 0, 0, 0, 0, 5, 0, 0
		},
		{
			0, 0, 3, 0, 0, 7, 0, 2, 8
		},
		{
			4, 0, 2, 5, 8, 0, 0, 0, 0
		},
		{
			8, 0, 0, 0, 7, 0, 2, 0, 0
		},
		{
			0, 0, 0, 2, 1, 3, 0, 0, 0
		},
		{
			0, 0, 9, 0, 6, 0, 0, 0, 4
		},
		{
			0, 0, 0, 0, 3, 8, 4, 0, 9
		},
		{
			3, 8, 0, 7, 0, 0, 6, 0, 0
		},
		{
			0, 0, 1, 0, 0, 0, 0, 0, 2
		}
	};

	private static readonly int[,] HardPuzzle = {
		{
			7, 0, 8, 0, 0, 0, 0, 2, 0
		},
		{
			0, 0, 1, 4, 8, 0, 0, 0, 3
		},
		{
			0, 0, 0, 0, 5, 7, 4, 0, 0
		},
		{
			0, 7, 0, 2, 0, 0, 0, 0, 1
		},
		{
			3, 0, 0, 0, 6, 0, 0, 0, 8
		},
		{
			1, 0, 0, 0, 0, 5, 0, 4, 0
		},
		{
			0, 0, 7, 5, 1, 0, 0, 0, 0
		},
		{
			8, 0, 0, 0, 2, 6, 7, 0, 0
		},
		{
			0, 1, 0, 0, 0, 0, 6, 0, 2
		}
	};

	private static readonly int[,] ExtremePuzzle = {
		{
			0, 0, 0, 0, 0, 8, 0, 0, 9
		},
		{
			0, 5, 0, 0, 9, 0, 0, 0, 0
		},
		{
			0, 0, 9, 0, 0, 4, 8, 0, 0
		},
		{
			0, 0, 2, 1, 4, 0, 0, 0, 3
		},
		{
			0, 0, 6, 0, 0, 0, 9, 0, 0
		},
		{
			4, 0, 0, 0, 6, 7, 1, 0, 0
		},
		{
			0, 0, 5, 9, 0, 0, 3, 0, 0
		},
		{
			0, 0, 0, 0, 2, 0, 0, 7, 0
		},
		{
			8, 0, 0, 4, 0, 0, 0, 0, 0
		}
	};

	private static readonly int[,] DefaultPuzzle = {
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0
		},
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0
		},
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0
		},
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0
		},
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0
		},
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0
		},
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0
		},
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0
		},
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0
		}
	};

	private static readonly int[,] SolvedPuzzle =
	{
		{ 1, 2, 3, 4, 5, 6, 7, 8, 9 },
		{ 4, 5, 6, 7, 8, 9, 1, 2, 3 },
		{ 7, 8, 9, 1, 2, 3, 4, 5, 6 },
		{ 2, 3, 1, 6, 7, 4, 8, 9, 5 },
		{ 8, 7, 5, 9, 1, 2, 3, 6, 4 },
		{ 6, 9, 4, 5, 3, 8, 2, 1, 7 },
		{ 3, 1, 7, 2, 6, 5, 9, 4, 8 },
		{ 5, 4, 2, 8, 9, 7, 6, 3, 1 },
		{ 9, 6, 8, 3, 4, 1, 5, 7, 2 }
	};
}