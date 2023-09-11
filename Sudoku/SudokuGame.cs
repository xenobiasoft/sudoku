using XenobiaSoft.Sudoku.Exceptions;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.PuzzleSolver;

namespace XenobiaSoft.Sudoku;

public class SudokuGame : ISudokuGame
{
	private readonly IGameStateMemory _gameState;
	private readonly IPuzzleSolver _puzzleSolver;

	public int Score { get; private set; }
	public Cell[] Puzzle { get; private set; } = new Cell[GameDimensions.Columns * GameDimensions.Rows];

	public SudokuGame(IGameStateMemory gameState, IPuzzleSolver puzzleSolver)
	{
		_puzzleSolver = puzzleSolver;
		_gameState = gameState;
	}

	public void LoadPuzzle(Cell[] puzzle)
	{
		Reset();
		Restore(puzzle);
		SaveGameState();
	}

	public void Reset()
	{
		_gameState.Clear();
		Initialize();
		Score = 0;
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
		
		Puzzle.GetCell(row, col).Value = value;
	}

	public void SolvePuzzle()
	{
		try
		{
			var solverScore = _puzzleSolver.TrySolvePuzzle(Puzzle);

			Score += solverScore;

			if (_puzzleSolver.IsSolved(Puzzle)) return;

			if (solverScore > 0)
			{
				SolvePuzzle();
			}
			else
			{
				TryBruteForceMethod();
			}
		}
		catch (InvalidMoveException)
		{
			Undo();
			SolvePuzzle();
		}
	}

	public void Undo()
	{
		var memento = _gameState.Undo();

		Score = memento.Score;
		var cells = memento
			.Cells
			.Select(x => (Cell)x.Clone())
			.ToArray();
		Restore(cells);
	}

	private void Initialize()
	{
		Puzzle = new Cell[GameDimensions.Columns * GameDimensions.Rows];
		var index = 0;

		for (var row = 0; row < GameDimensions.Rows; row++)
		{
			for (var col = 0; col < GameDimensions.Columns; col++)
			{
				Puzzle[index++] = new Cell(row, col);
			}
		}
	}

	public void Restore(Cell[] cells)
	{
		Puzzle = cells;
	}

	private void SaveGameState()
	{
		if (!Puzzle.IsValid())
		{
			throw new InvalidMoveException();
		}

		var clonedPuzzle = Puzzle.Select(x => (Cell)x.Clone());

		_gameState.Save(new GameStateMemento(clonedPuzzle, Score));
	}

	private void TryBruteForceMethod()
	{
		SaveGameState();
		Score += 5;
		Puzzle.SetCellWithFewestPossibleValues();
		SolvePuzzle();
	}
}