using XenobiaSoft.Sudoku.Generator;
using XenobiaSoft.Sudoku.Solver;

namespace XenobiaSoft.Sudoku;

public class SudokuGame(IPuzzleSolver puzzleSolver, IPuzzleGenerator puzzleGenerator)
    : ISudokuGame
{
    public Cell[] Puzzle { get; private set; } = new Cell[GameDimensions.Columns * GameDimensions.Rows];

    public async Task New(Level level)
	{
		var puzzle = await puzzleGenerator.Generate(level).ConfigureAwait(false);

		await LoadPuzzle(puzzle).ConfigureAwait(false);
	}

	public async Task LoadPuzzle(Cell[] puzzle)
	{
		await Reset().ConfigureAwait(false);

		Puzzle = puzzle;
	}

	public async Task Reset()
	{
		Puzzle = await puzzleGenerator.GenerateEmptyPuzzle().ConfigureAwait(false);
	}

	public void SetCell(int row, int col, int value)
	{
		if (col is < 0 or > 8)
		{
			throw new ArgumentException("Invalid column", nameof(col));
		}
		if (row is < 0 or > 8)
		{
			throw new ArgumentException("Invalid row", nameof(row));
		}
		if (value is < 0 or > 9)
		{
			throw new ArgumentException("Invalid value", nameof(value));
		}
		
		Puzzle.SetCell(row, col, value);
	}

	public async Task SolvePuzzle()
	{
		Puzzle = await puzzleSolver.SolvePuzzle(Puzzle).ConfigureAwait(false);
	}
}