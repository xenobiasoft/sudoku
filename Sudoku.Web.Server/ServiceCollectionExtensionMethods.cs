using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.HttpClients;

namespace Sudoku.Web.Server
{
    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection RegisterBlazorGameServices(this IServiceCollection services, IConfiguration config)
        {
            // Register notification services
            services
                .AddScoped<ICellFocusedNotificationService, CellFocusedNotificationService>()
                .AddScoped<IInvalidCellNotificationService, InvalidCellNotificationService>()
                .AddScoped<IGameNotificationService, GameNotificationService>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<IJsRuntimeWrapper, JsRuntimeWrapper>()
                .AddScoped<IGameTimer>(sp => new GameTimer(TimeSpan.FromSeconds(1)))
                .AddScoped<IGameSessionManager, GameSessionManager>();

            // Register HTTP clients for API communication using Aspire service discovery
            services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(client =>
            {
                // This will be resolved by Aspire service discovery to sudoku-api service
                client.BaseAddress = new Uri("http://sudoku-api");
            });

            services.AddHttpClient<IGameApiClient, GameApiClient>(client =>
            {
                // This will be resolved by Aspire service discovery to sudoku-api service  
                client.BaseAddress = new Uri("http://sudoku-api");
            });

            // Register API-based services
            services.AddScoped<IAliasService, ApiBasedAliasService>();
            services.AddScoped<IApiBasedGameStateManager, ApiBasedGameStateManager>();
            
            // Register legacy compatibility service that implements IGameStateManager using the new API
            services.AddScoped<IGameStateManager, LegacyCompatibilityGameStateManager>();

            // Keep local storage for caching
            services.AddScoped<ILocalStorageService, LocalStorageService>();

            return services;
        }
    }
}
