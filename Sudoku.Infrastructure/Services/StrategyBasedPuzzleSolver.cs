using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Infrastructure.Services.Strategies;

namespace Sudoku.Infrastructure.Services;

public class StrategyBasedPuzzleSolver(IEnumerable<SolverStrategy> strategies, IPuzzleRepository puzzleRepository, ILogger<StrategyBasedPuzzleSolver> logger)
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

                changesMade = await ApplyStrategies();

                changesMade = TryBruteForceMethod();
            }
            catch (InvalidPuzzleException)
            {
                await UndoAsync();
            }
            catch (InvalidMoveException)
            {
                await UndoAsync();
            }
        }
    }

    private async Task<bool> ApplyStrategies()
    {
        var changesMade = false;

        foreach (var strategy in strategies.OrderBy(x => x.Order))
        {
            logger.LogInformation($"Solving with {strategy.GetType().Name}");

            changesMade = changesMade || strategy.SolvePuzzle(_puzzle);

            if (!_puzzle.IsValid())
            {
                logger.LogWarning($"Failure in solving puzzle using {strategy.GetType().Name} strategy");
                await UndoAsync();
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
        logger.LogInformation("Solving with BruteForce technique");
        _puzzle.PopulatePossibleValues();
        SetCellWithFewestPossibleValues();

        return true;
    }

    private void SetCellWithFewestPossibleValues()
    {
        var rnd = Random.Shared;
        var cell = _puzzle.Cells
            .Where(c => !c.HasValue && c.PossibleValues.Any())
            .OrderBy(c => c.PossibleValues.Count)
            .FirstOrDefault();

        if (cell == null)
        {
            logger.LogWarning("No cell with possible values found, puzzle might be solved or invalid.");
            return;
        }

        var value = cell.PossibleValues.ElementAt(rnd.Next(cell.PossibleValues.Count));
        logger.LogInformation($"Setting cell at ({cell.Row}, {cell.Column}) to {value}");
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
