using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Infrastructure.Services.Strategies;

namespace Sudoku.Infrastructure.Services;

public class StrategyBasedPuzzleSolver(IEnumerable<SolverStrategy> strategies, IPuzzleRepository puzzleRepository)
    : IPuzzleSolver
{
    private const string Alias = "SudokuSolverAlias";

    private SudokuPuzzle _puzzle;

    public async Task<SudokuPuzzle> SolvePuzzle(SudokuPuzzle puzzle)
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

                if (IsPuzzleSolved())
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
                throw new InvalidPuzzleException();
            }

            if (IsPuzzleSolved())
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
        SetCellWithFewestPossibleValues();

        return true;
    }

    private void SetCellWithFewestPossibleValues()
    {
        var cell = _puzzle.Cells
            .Where(c => !c.HasValue && c.PossibleValues.Any())
            .OrderBy(c => c.PossibleValues.Count)
            .FirstOrDefault();

        if (cell == null)
        {
            Console.WriteLine("No cell with possible values found, puzzle might be solved or invalid.");
            return;
        }

        var random = new Random();
        var value = cell.PossibleValues.ElementAt(random.Next(cell.PossibleValues.Count));
        Console.WriteLine($"Setting cell at ({cell.Row}, {cell.Column}) to {value}");
        cell.SetValue(value);
    }

    private Task SaveAsync()
    {
        if (!_puzzle.IsValid())
        {
            throw new InvalidPuzzleException();
        }

        return puzzleRepository.SaveAsync(_puzzle);
    }

    private async Task UndoAsync()
    {
        var memento = await puzzleRepository.UndoAsync(Alias, _puzzle.PuzzleId);

        _puzzle = memento ?? throw new NoMoveHistoryException();
    }

    private bool IsPuzzleSolved()
    {
        return _puzzle.Cells.All(c => c.HasValue) && _puzzle.IsValid();
    }
}
