using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.PuzzleSolver;

namespace XenobiaSoft.Sudoku;

public class SudokuGame : ISudokuGame
{
	private readonly IGameStateMemory _gameState;
	private readonly IPuzzleSolver _puzzleSolver;

	private const int SolveMaxAttempts = 50;
	private int _solveAttempts = 0;

	public SudokuGame(IGameStateMemory gameState, IPuzzleSolver puzzleSolver)
	{
		_puzzleSolver = puzzleSolver;
		_gameState = gameState;
	}

	public void SaveGameState()
	{
		_gameState.Save(new GameStateMemento((string[,])Puzzle.PossibleValues.Clone(), (int[,])Puzzle.Values.Clone(), Score));
	}

	public void Undo()
	{
		var memento = _gameState.Undo();

		Score = memento.Score;
		Puzzle.PossibleValues = (string[,])memento.PossibleValues.Clone();
		Puzzle.Values = (int[,])memento.Values.Clone();
	}

	public void LoadPuzzle(SudokuPuzzle puzzle)
	{
		Puzzle = new SudokuPuzzle
		{
			PossibleValues = (string[,])puzzle.PossibleValues.Clone(),
			Values = (int[,])puzzle.Values.Clone()
		};
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