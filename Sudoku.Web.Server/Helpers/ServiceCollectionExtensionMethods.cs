using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Generator;
using XenobiaSoft.Sudoku.Solver;
using XenobiaSoft.Sudoku.Storage.Azure;
using XenobiaSoft.Sudoku.Strategies;

namespace Sudoku.Web.Server.Helpers
{
    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection RegisterBlazorGameServices(this IServiceCollection services)
        {
            services
                .AddScoped<ICellFocusedNotificationService, CellFocusedNotificationService>()
                .AddScoped<IInvalidCellNotificationService, InvalidCellNotificationService>()
                .AddScoped<IGameNotificationService, GameNotificationService>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<IGameStateManager, GameStateManager>()
                .AddScoped<IJsRuntimeWrapper, JsRuntimeWrapper>()
                .AddScoped<IGameTimer>(sp => new GameTimer(TimeSpan.FromSeconds(1)))
                .AddScoped<IGameSessionManager, GameSessionManager>()
                .AddScoped<IAliasService, AliasService>();

            return services;
        }

        public static IServiceCollection RegisterGameServices(this IServiceCollection services, ConfigurationManager config)
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

            services.AddAzureStorage(config);

            return services;
        }
    }
}
