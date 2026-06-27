using Microsoft.Extensions.Logging.Abstractions;
using Sudoku.Application.Interfaces;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Services;
using Sudoku.Infrastructure.Services.Strategies;

namespace Sudoku.Benchmarks;

/// <summary>
/// Constructs the old and new algorithm implementations without the full Azure DI graph.
/// The legacy solver's collaborators (strategy pipeline, in-memory repository, logger) are
/// wired up directly, exactly as the production container would.
/// </summary>
public static class AlgorithmFactory
{
    public static IPuzzleSolver CreateOldSolver()
    {
        var strategies = typeof(SolverStrategy).Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(SolverStrategy)) && !t.IsAbstract)
            .Select(t => (SolverStrategy)Activator.CreateInstance(t)!)
            .ToList();

        return new StrategyBasedPuzzleSolver(
            strategies,
            new InMemoryPuzzleRepository(),
            NullLogger<StrategyBasedPuzzleSolver>.Instance);
    }

    public static IPuzzleSolver CreateNewSolver() => new BitwiseBacktrackingPuzzleSolver();

    public static IPuzzleGenerator CreateOldGenerator() => new PuzzleGenerator(CreateOldSolver());

    public static IPuzzleGenerator CreateNewGenerator() => new UniqueSolutionPuzzleGenerator();
}
