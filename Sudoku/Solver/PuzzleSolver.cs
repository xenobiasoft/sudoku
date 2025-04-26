using XenobiaSoft.Sudoku.Exceptions;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Strategies;

namespace XenobiaSoft.Sudoku.Solver;

public class PuzzleSolver(IEnumerable<SolverStrategy> strategies, IGameStateMemory gameStateMemory)
    : IPuzzleSolver
{
    private int _score;
    private ISudokuPuzzle _puzzle;

    public async Task<ISudokuPuzzle> SolvePuzzle(ISudokuPuzzle puzzle)
    {
	    _score = 0;
	    _puzzle = puzzle;

        await Task.Run(SolveLoop).ConfigureAwait(false);

        return _puzzle;
    }

    private void SolveLoop()
    {
        var changesMade = true;

        while (changesMade)
        {
            var previousScore = _score;

            try
            {
                Save();
                changesMade = ApplyStrategies(previousScore);

                if (!changesMade)
                {
                    TryBruteForceMethod();
                    changesMade = true;
                }
            }
            catch (InvalidMoveException)
            {
                Undo();
            }

            if (_puzzle.IsSolved())
            {
                break;
            }
        }
    }

    private bool ApplyStrategies(int previousScore)
    {
        foreach (var strategy in strategies)
        {
            Console.WriteLine($"Solving with {strategy.GetType().Name}");
            _score += strategy.SolvePuzzle(_puzzle);

            if (!_puzzle.IsValid())
            {
                Console.WriteLine($"Failure in solving puzzle using {strategy.GetType().Name} strategy");
                throw new InvalidMoveException();
            }

            if (_puzzle.IsSolved())
            {
                break;
            }
        }

        return previousScore != _score;
    }

    private void TryBruteForceMethod()
	{
		Console.WriteLine($"Solving with BruteForce technique");
		_score += 5;
		_puzzle.PopulatePossibleValues();
		_puzzle.SetCellWithFewestPossibleValues();
	}

    private void Save()
    {
	    if (!_puzzle.IsValid())
	    {
		    throw new InvalidMoveException();
	    }
        
	    gameStateMemory.Save(new GameStateMemento(_puzzle.PuzzleId, _puzzle.GetAllCells(), _score));
    }

    private void Undo()
    {
	    var memento = gameStateMemory.Undo();

	    _score = memento.Score;
        _puzzle.Restore(memento.Board);
    }
}