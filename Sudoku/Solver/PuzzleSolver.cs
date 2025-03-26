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
        var changesMade = true;

		// TODO: Refactor this. It has a high cyclomatic complexity

        await Task.Run(() =>
        {
	        while (changesMade)
	        {
		        var previousScore = _score;

		        try
				{
					Save();

					foreach (var strategy in strategies)
					{
						Console.WriteLine($"Solving with {strategy.GetType().Name}");
						_score += strategy.SolvePuzzle(_puzzle);
						changesMade = previousScore != _score;
						previousScore = _score;

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
        }).ConfigureAwait(false);

        return _puzzle;
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
        
	    gameStateMemory.Save(new GameStateMemento(_puzzle, _score));
    }

    private void Undo()
    {
	    var memento = gameStateMemory.Undo();

	    _score = memento.Score;
        _puzzle = memento.Puzzle;
    }
}