using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.HttpClients;

namespace Sudoku.Web.Server
{
    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection RegisterBlazorGameServices(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<IGameStateManager, GameManager>()
                .AddScoped<IGameStatisticsManager, GameManager>()
                .AddScoped<IGameManager, GameManager>()
                .AddScoped<IPlayerManager, PlayerManager>()
                .AddScoped<IJsRuntimeWrapper, JsRuntimeWrapper>()
                .AddScoped<IGameTimer>(sp => new GameTimer(TimeSpan.FromSeconds(1)))
                .AddScoped<IPlayerApiClient, PlayerApiClient>()
                .AddScoped<IGameApiClient, GameApiClient>();

            services.AddHttpClient<IGameApiClient, GameApiClient>(client =>
            {
                // This will be resolved by Aspire service discovery to sudoku-api service  
                client.BaseAddress = new Uri("http://sudoku-api");
            });
            
            return services;
        }
    }
}
