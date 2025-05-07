using XenobiaSoft.Sudoku.Exceptions;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Strategies;

namespace XenobiaSoft.Sudoku.Solver;

public class PuzzleSolver(IEnumerable<SolverStrategy> strategies, Func<string, IGameStateStorage> gameStateMemoryFactory)
    : IPuzzleSolver
{
    private ISudokuPuzzle _puzzle;
    private readonly IGameStateStorage _gameStateMemory = gameStateMemoryFactory(GameStateTypes.InMemory);

    public async Task<ISudokuPuzzle> SolvePuzzle(ISudokuPuzzle puzzle)
    {
        _puzzle = puzzle;

        await Task.Run(SolveLoop).ConfigureAwait(false);

        return _puzzle;
    }

    private async Task SolveLoop()
    {
        var changesMade = true;

        while (changesMade)
        {
            try
            {
                await SaveAsync();

                if (_puzzle.IsSolved())
                {
                    break;
                }

                changesMade = ApplyStrategies();

                if (changesMade) continue;

                changesMade = TryBruteForceMethod();
            }
            catch (InvalidMoveException)
            {
                await UndoAsync();
            }
        }

        if (_puzzle.IsSolved())
        {
            await _gameStateMemory.DeleteAsync(_puzzle.PuzzleId);
        }
    }

    private bool ApplyStrategies()
    {
        var changesMade = false;

        foreach (var strategy in strategies)
        {
            Console.WriteLine($"Solving with {strategy.GetType().Name}");

            changesMade = changesMade || strategy.SolvePuzzle(_puzzle);

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

        return changesMade;
    }

    private bool TryBruteForceMethod()
    {
        Console.WriteLine("Solving with BruteForce technique");
        _puzzle.PopulatePossibleValues();
        _puzzle.SetCellWithFewestPossibleValues();

        return true;
    }

    private Task SaveAsync()
    {
        if (!_puzzle.IsValid())
        {
            throw new InvalidMoveException();
        }

        return _gameStateMemory.SaveAsync(new GameStateMemory(_puzzle.PuzzleId, _puzzle.GetAllCells(), 0));
    }

    private async Task UndoAsync()
    {
        var memento = await _gameStateMemory.UndoAsync(_puzzle.PuzzleId);

        if (memento == null)
        {
            throw new InvalidBoardException();
        }

        _puzzle.Restore(memento.Board);
    }
}
