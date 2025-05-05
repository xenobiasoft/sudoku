using XenobiaSoft.Sudoku.Exceptions;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Strategies;

namespace XenobiaSoft.Sudoku.Solver;

public class PuzzleSolver(IEnumerable<SolverStrategy> strategies, Func<string, IGameStateStorage> gameStateMemoryFactory)
    : IPuzzleSolver
{
    private int _score;
    private ISudokuPuzzle _puzzle;
    private readonly IGameStateStorage _gameStateMemory = gameStateMemoryFactory(GameStateTypes.InMemory);

    public async Task<ISudokuPuzzle> SolvePuzzle(ISudokuPuzzle puzzle)
    {
	    _score = 0;
	    _puzzle = puzzle;

        await Task.Run(SolveLoop).ConfigureAwait(false);

        return _puzzle;
    }

    private async Task SolveLoop()
    {
        var changesMade = true;

        while (changesMade)
        {
            var previousScore = _score;

            try
            {
                await SaveAsync();
                changesMade = ApplyStrategies(previousScore);

                if (!changesMade)
                {
                    TryBruteForceMethod();
                    changesMade = true;
                }
            }
            catch (InvalidMoveException)
            {
                await UndoAsync();
            }

            if (_puzzle.IsSolved())
            {
                await _gameStateMemory.DeleteAsync(_puzzle.PuzzleId);
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
		Console.WriteLine("Solving with BruteForce technique");
		_score += 5;
		_puzzle.PopulatePossibleValues();
		_puzzle.SetCellWithFewestPossibleValues();
	}

    private Task SaveAsync()
    {
	    if (!_puzzle.IsValid())
	    {
		    throw new InvalidMoveException();
	    }
        
	    return _gameStateMemory.SaveAsync(new GameStateMemory(_puzzle.PuzzleId, _puzzle.GetAllCells(), _score));
    }

    private async Task UndoAsync()
    {
	    var memento = await _gameStateMemory.UndoAsync(_puzzle.PuzzleId);

        if (memento == null)
        {
            throw new InvalidBoardException();
        }

        _score = memento.Score;
        _puzzle.Restore(memento.Board);
    }
}