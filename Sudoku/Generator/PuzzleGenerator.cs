using XenobiaSoft.Sudoku.Solver;

namespace XenobiaSoft.Sudoku.Generator;

public class PuzzleGenerator : IPuzzleGenerator
{
	private readonly IPuzzleSolver _puzzleSolver;

	public PuzzleGenerator(IPuzzleSolver puzzleSolver)
	{
		_puzzleSolver = puzzleSolver;
	}

	public async Task<Cell[]> Generate(Level level)
	{
		var puzzle = await GenerateEmptyPuzzle();

		puzzle = await _puzzleSolver.SolvePuzzle(puzzle);

		puzzle = CreateEmptyCells(puzzle, level);

		return puzzle;
	}

	private Cell[] CreateEmptyCells(Cell[] puzzle, Level level)
	{
		var numberOfEmptyCells = level switch
		{
			Level.Easy => RandomGenerator.RandomNumber(40, 45),
			Level.Medium => RandomGenerator.RandomNumber(46, 49),
			Level.Hard => RandomGenerator.RandomNumber(50, 53),
			Level.ExtremelyHard => RandomGenerator.RandomNumber(54, 58),
			_ => 0
		};
		var emptyCellCoords = new List<Tuple<int, int>>();

		while (emptyCellCoords.Count < numberOfEmptyCells)
		{
			var emptyCells = GetRandomCellCoordinates();
			if (!emptyCellCoords.Contains(emptyCells[0]) && emptyCellCoords.Count < numberOfEmptyCells)
			{
				emptyCellCoords.Add(emptyCells[0]);
			}
			if (!emptyCellCoords.Contains(emptyCells[1]) && emptyCellCoords.Count < numberOfEmptyCells)
			{
				emptyCellCoords.Add(emptyCells[1]);
			}
		}

		foreach (var rowCol in emptyCellCoords)
		{
			puzzle.SetCell(rowCol.Item1, rowCol.Item2, null);
			puzzle.SetCell(rowCol.Item1, rowCol.Item1, null);

			if (puzzle.Count(x => !x.Value.HasValue) >= numberOfEmptyCells) break;
		}

		return puzzle;
	}

	private Tuple<int, int>[] GetRandomCellCoordinates()
	{
		var randomCells = new List<Tuple<int, int>>();
		var row = RandomGenerator.RandomNumber(0, 9);
		var col = RandomGenerator.RandomNumber(0, 9);

		if (row != col)
		{
			randomCells.Add(new Tuple<int, int>(row, col));
			randomCells.Add(new Tuple<int, int>(col, row));
		}
		else
		{
			randomCells.Add(new Tuple<int, int>(row, row));
			randomCells.Add(new Tuple<int, int>(8 - row, 8 - row));
		}

		return randomCells.ToArray();
	}

	public async Task<Cell[]> GenerateEmptyPuzzle()
	{
		var puzzle = new Cell[GameDimensions.Columns * GameDimensions.Rows];
		var index = 0;

		await Task.Run(() =>
		{
			for (var row = 0; row < GameDimensions.Rows; row++)
			{
				for (var col = 0; col < GameDimensions.Columns; col++)
				{
					puzzle[index++] = new Cell(row, col);
				}
			}
		});

		return puzzle;
	}
}