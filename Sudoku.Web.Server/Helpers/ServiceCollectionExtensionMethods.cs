using Azure.Identity;
using Microsoft.Extensions.Azure;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;
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
            services.AddScoped<ICellFocusedNotificationService, CellFocusedNotificationService>();
            services.AddScoped<IInvalidCellNotificationService, InvalidCellNotificationService>();
            services.AddScoped<IGameNotificationService, GameNotificationService>();
            services.AddScoped<ILocalStorageService, LocalStorageService>();
            services.AddScoped<IGameStateManager, GameStateManager>();
            services.AddScoped<IJsRuntimeWrapper, JsRuntimeWrapper>();

            return services;
        }

        public static IServiceCollection RegisterGameServices(this IServiceCollection services, ConfigurationManager config)
        {
            services
                .AddScoped<ISudokuGame, SudokuGame>()
                .AddScoped<IPuzzleSolver, PuzzleSolver>()
                .AddScoped<IPuzzleGenerator, PuzzleGenerator>()
                .AddScoped<IStorageService, AzureStorageService>()
                .AddScoped<InMemoryGameStateStorage>()
                .AddScoped<AzureBlobGameStateStorage>()
                .AddScoped<Func<string, IGameStateStorage>>(sp => key =>
                {
                    return key switch
                    {
                        GameStateTypes.InMemory => sp.GetRequiredService<InMemoryGameStateStorage>(),
                        GameStateTypes.AzurePersistent => sp.GetRequiredService<AzureBlobGameStateStorage>(),
                        _ => throw new ArgumentException($"Unknown game state memory type: {key}")
                    };
                });

            typeof(SudokuPuzzle).Assembly
                .GetTypes()
                .Where(x => x.Name.EndsWith("Strategy") && !x.IsAbstract && !x.IsInterface)
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
