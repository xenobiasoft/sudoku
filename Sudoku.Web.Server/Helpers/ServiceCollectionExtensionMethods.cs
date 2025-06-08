using Azure.Identity;
using Microsoft.Extensions.Azure;
using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.GameState.Decorators;
using XenobiaSoft.Sudoku.Generator;
using XenobiaSoft.Sudoku.Services;
using XenobiaSoft.Sudoku.Solver;
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
                .AddScoped<IStorageService, AzureStorageService>()
                .AddScoped<IGameStateStorage, InMemoryGameStateStorage>()
                .AddScoped<IGameStateStorage, AzureBlobGameStateStorage>()
                .AddScoped<IGameStateStorage>(x =>
                    ActivatorUtilities.CreateInstance<CachingAzureBlobGameStateStorageDecorator>(x,
                        ActivatorUtilities.CreateInstance<AzureBlobGameStateStorage>(x)));

            typeof(SudokuPuzzle).Assembly
                .GetTypes()
                .Where(x => x.Name.EndsWith("Strategy") && x is { IsAbstract: false, IsInterface: false })
                .ToList()
                .ForEach(x =>
                {
                    services.AddTransient(typeof(SolverStrategy), x);
                });

            services.AddAzureClients(builder =>
            {
                builder.UseCredential(new DefaultAzureCredential());
                builder.AddBlobServiceClient(config["AzureStorageConnection"]);
            });

            return services;
        }
    }
}
