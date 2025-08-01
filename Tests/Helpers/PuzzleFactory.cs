﻿using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers;

public class PuzzleFactory
{
	public static ISudokuPuzzle GetPuzzle(GameDifficulty difficulty, bool rotateGrid = false)
    {
		var puzzleValues = difficulty switch
		{
			GameDifficulty.Easy => EasyPuzzle,
			GameDifficulty.Medium => MediumPuzzle,
			GameDifficulty.Hard => HardPuzzle,
			GameDifficulty.ExtremelyHard => ExtremePuzzle,
			_ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
		};

		if (rotateGrid)
		{
			puzzleValues = RotateGrid(puzzleValues);
		}

		return PopulateCells(puzzleValues);
	}

	public static int?[,] RotateGrid(int?[,] values)
	{
		var grid = new int?[9, 9];

		for (var col = 0; col < 9; col++)
		{
			for (var row = 0; row < 9; row++)
			{
				grid[col, row] = values[row, col];
			}
		}

		return grid;
	}

	public static ISudokuPuzzle GetEmptyPuzzle()
	{
		return new SudokuPuzzle();
	}

	public static ISudokuPuzzle GetSolvedPuzzle()
	{
        return PopulateCells(SolvedPuzzle);
	}

	public static ISudokuPuzzle PopulateCells(int?[,] values)
	{
		var puzzle = new SudokuPuzzle();
        var cells = new Cell[81];
		var index = 0;

		for (var col = 0; col < 9; col++)
		{
			for (var row = 0; row < 9; row++)
			{
				var cell = new Cell(row, col) { Value = values[row, col] };
				cells[index++] = cell;
			}
		}

        puzzle.Load(new GameStateMemory { PuzzleId = puzzle.PuzzleId, Board = cells });

        return puzzle;
    }

	private static readonly int?[,] EasyPuzzle = {
		{ 5, 3, null, null, 7, null, null, null, null },
		{ 6, null, null, 1, 9, 5, null, null, null },
		{ null, 9, 8, null, null, null, null, 6, null },
		{ 8, null, null, null, 6, null, null, null, 3 },
		{ 4, null, null, 8, null, 3, null, null, 1 },
		{ 7, null, null, null, 2, null, null, null, 6 },
		{ null, 6, null, null, null, null, 2, 8, null },
		{ null, null, null, 4, 1, 9, null, null, 5 },
		{ null, null, null, null, 8, null, null, 7, 9 }
	};

	private static readonly int?[,] MediumPuzzle = {
		{ 7, null, null, null, null, null, 5, null, null },
		{ null, null, 3, null, null, 7, null, 2, 8 },
		{ 4, null, 2, 5, 8, null, null, null, null },
		{ 8, null, null, null, 7, null, 2, null, null },
		{ null, null, null, 2, 1, 3, null, null, null },
		{ null, null, 9, null, 6, null, null, null, 4 },
		{ null, null, null, null, 3, 8, 4, null, 9 },
		{ 3, 8, null, 7, null, null, 6, null, null },
		{ null, null, 1, null, null, null, null, null, 2 }
	};

	private static readonly int?[,] HardPuzzle = {
		{ 7, null, 8, null, null, null, null, 2, null },
		{ null, null, 1, 4, 8, null, null, null, 3 },
		{ null, null, null, null, 5, 7, 4, null, null },
		{ null, 7, null, 2, null, null, null, null, 1 },
		{ 3, null, null, null, 6, null, null, null, 8 },
		{ 1, null, null, null, null, 5, null, 4, null },
		{ null, null, 7, 5, 1, null, null, null, null },
		{ 8, null, null, null, 2, 6, 7, null, null },
		{ null, 1, null, null, null, null, 6, null, 2 }
	};

	private static readonly int?[,] ExtremePuzzle = {
		{ null, null, null, null, null, 8, null, null, 9 },
		{ null, 5, null, null, 9, null, null, null, null },
		{ null, null, 9, null, null, 4, 8, null, null },
		{ null, null, 2, 1, 4, null, null, null, 3 },
		{ null, null, 6, null, null, null, 9, null, null },
		{ 4, null, null, null, 6, 7, 1, null, null },
		{ null, null, null, 9, null, null, 3, null, null },
		{ null, null, null, null, 2, null, null, 7, null },
		{ 8, null, null, 4, null, null, null, null, null }
	};

	private static readonly int?[,] DefaultPuzzle = {
		{ null, null, null, null, null, null, null, null, null },
		{ null, null, null, null, null, null, null, null, null },
		{ null, null, null, null, null, null, null, null, null },
		{ null, null, null, null, null, null, null, null, null },
		{ null, null, null, null, null, null, null, null, null },
		{ null, null, null, null, null, null, null, null, null },
		{ null, null, null, null, null, null, null, null, null },
		{ null, null, null, null, null, null, null, null, null },
		{ null, null, null, null, null, null, null, null, null }
	};

	private static readonly int?[,] SolvedPuzzle =
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