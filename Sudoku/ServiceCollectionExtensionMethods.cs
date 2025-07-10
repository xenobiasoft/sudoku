using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Generator;
using XenobiaSoft.Sudoku.Solver;
using XenobiaSoft.Sudoku.Strategies;

namespace XenobiaSoft.Sudoku;

public static class ServiceCollectionExtensionMethods
{
    public static IServiceCollection RegisterGameServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddScoped<ISudokuGame, SudokuGame>()
            .AddScoped<IPuzzleSolver, StrategyBasedPuzzleSolver>()
            .AddScoped<IPuzzleGenerator, PuzzleGenerator>()
            .AddScoped<IGameStateStorage, InMemoryGameStateStorage>()
            .AddScoped<IInMemoryGameStateStorage, InMemoryGameStateStorage>();

        typeof(SudokuPuzzle).Assembly
            .GetTypes()
            .Where(x => x.Name.EndsWith("Strategy") && x is { IsAbstract: false, IsInterface: false })
            .ToList()
            .ForEach(x =>
            {
                services.AddTransient(typeof(SolverStrategy), x);
            });

        return services;
    }
}