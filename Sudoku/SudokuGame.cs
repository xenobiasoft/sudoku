using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.PuzzleSolver;

namespace XenobiaSoft.Sudoku;

public class SudokuGame : ISudokuGame
{
	private readonly IGameStateMemory _gameState;
	private readonly IPuzzleSolver _puzzleSolver;

	private const int SolveMaxAttempts = 50;
	private int _solveAttempts;

	public SudokuGame(IGameStateMemory gameState, IPuzzleSolver puzzleSolver)
	{
		_puzzleSolver = puzzleSolver;
		_gameState = gameState;
	}

	public void LoadPuzzle(SudokuPuzzle puzzle)
	{
		Reset();
		Puzzle = puzzle;
	}

	public void Reset()
	{
		_gameState.Clear();
		Puzzle.Reset();
		Score = 0;
	}

	public void SaveGameState()
	{
		_gameState.Save(new GameStateMemento(Puzzle.Cells, Score));
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
		catch (InvalidOperationException)
		{
			Undo();
			RetrySolvePuzzle();
		}
	}

	public void Undo()
	{
		var memento = _gameState.Undo();

		Score = memento.Score;
		Puzzle.Restore(memento.Cells);
	}

	private void TryBruteForceMethod()
	{
		SaveGameState();
		Score += 5;
		Puzzle.SetCellWithFewestPossibleValues();
		RetrySolvePuzzle();
	}

	private void RetrySolvePuzzle()
	{
		_solveAttempts++;
		
		if (_solveAttempts >= SolveMaxAttempts) return;

		SolvePuzzle();
	}

	public SudokuPuzzle Puzzle { get; set; }

	public int Score { get; set; }
}