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
            services.AddSingleton<ICellFocusedNotificationService, CellFocusedNotificationService>();
            services.AddSingleton<IInvalidCellNotificationService, InvalidCellNotificationService>();
            services.AddSingleton<IGameNotificationService, GameNotificationService>();
            services.AddSingleton<IGameStateManager, AzureStorageGameStateManager>();
            services.AddScoped<ILocalStorageService, LocalStorageService>();

            return services;
        }

        public static IServiceCollection RegisterGameServices(this IServiceCollection services, ConfigurationManager config)
        {
            services
                .AddTransient<ISudokuGame, SudokuGame>()
                .AddTransient<IPuzzleSolver, PuzzleSolver>()
                .AddTransient<IPuzzleGenerator, PuzzleGenerator>()
                .AddSingleton<IStorageService, AzureStorageService>()
                .AddSingleton<InMemoryGameStateManager>()
                .AddSingleton<AzureStorageGameStateManager>()
                .AddSingleton<Func<string, IGameStateManager>>(sp => key =>
                {
                    return key switch
                    {
                        GameStateTypes.InMemory => sp.GetRequiredService<InMemoryGameStateManager>(),
                        GameStateTypes.AzurePersistent => sp.GetRequiredService<AzureStorageGameStateManager>(),
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
