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
			0, 5, 0, 0, 7, 0, 0, 8, 3
		},
		{
			0, 0, 4, 0, 0, 0, 0, 6, 0
		},
		{
			0, 0, 0, 0, 5, 0, 0, 0, 0
		},
		{
			8, 3, 0, 6, 0, 0, 0, 0, 0
		},
		{
			0, 0, 0, 9, 0, 0, 1, 0, 0
		},
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0
		},
		{
			5, 0, 7, 0, 0, 0, 4, 0, 0
		},
		{
			0, 0, 0, 3, 0, 2, 0, 0, 0
		},
		{
			1, 0, 0, 0, 0, 0, 0, 0, 0
		}
	};
}