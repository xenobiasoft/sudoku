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

		return puzzle;
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