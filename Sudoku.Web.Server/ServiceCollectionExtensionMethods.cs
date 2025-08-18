using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.Abstractions.V2;
using Sudoku.Web.Server.Services.HttpClients;
using Sudoku.Web.Server.Services.V2;

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
            services.AddScoped<IAliasService, AliasService>();
            services.AddScoped<Services.Abstractions.V2.IGameStateManager, Services.V2.GameStateManager>();
            
            // Keep local storage for caching
            services.AddScoped<ILocalStorageService, LocalStorageService>();

            return services;
        }
    }
}
