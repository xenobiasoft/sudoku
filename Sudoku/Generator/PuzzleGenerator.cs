using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.Exceptions;
using XenobiaSoft.Sudoku.Solver;

namespace XenobiaSoft.Sudoku.Generator;

[Obsolete("Use Sudoku.Infrastructure.Services.PuzzleGenerator instead.")]
public class PuzzleGenerator(IPuzzleSolver puzzleSolver) : IPuzzleGenerator
{
    public async Task<ISudokuPuzzle> GenerateAsync(GameDifficulty difficulty)
	{
		var puzzle = await GenerateEmptyPuzzleAsync().ConfigureAwait(false);

        try
        {
            puzzle = await puzzleSolver.SolvePuzzle(puzzle).ConfigureAwait(false);
        }
        catch (InvalidBoardException)
        {
            return await GenerateAsync(difficulty);
        }

		puzzle = CreateEmptyCells(puzzle, difficulty);

        puzzle = LockCompletedCells(puzzle);

		return puzzle;
	}

    public async Task<ISudokuPuzzle> GenerateEmptyPuzzleAsync()
    {
        return await Task.FromResult(new SudokuPuzzle());
    }

    private ISudokuPuzzle CreateEmptyCells(ISudokuPuzzle puzzle, GameDifficulty difficulty)
	{
		var numberOfEmptyCells = difficulty switch
		{
			GameDifficulty.Easy => RandomGenerator.RandomNumber(40, 45),
			GameDifficulty.Medium => RandomGenerator.RandomNumber(46, 49),
			GameDifficulty.Hard => RandomGenerator.RandomNumber(50, 53),
			GameDifficulty.ExtremelyHard => RandomGenerator.RandomNumber(54, 58),
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

			if (puzzle.GetAllCells().Count(x => !x.Value.HasValue) >= numberOfEmptyCells) break;
		}

		return puzzle;
	}

    private ISudokuPuzzle LockCompletedCells(ISudokuPuzzle puzzle)
    {
		puzzle
            .GetAllCells()
            .ToList()
            .ForEach(x => x.Locked = x.Value.HasValue);

        return puzzle;
    }

    private Tuple<int, int>[] GetRandomCellCoordinates()
	{
		var randomCells = new List<Tuple<int, int>>();
		var row = RandomGenerator.RandomNumber(0, 9);
		var col = RandomGenerator.RandomNumber(0, 9);

		randomCells.Add(new Tuple<int, int>(row, col));
		randomCells.Add(new Tuple<int, int>(8 - row, 8 - col));

		return randomCells.ToArray();
	}
}