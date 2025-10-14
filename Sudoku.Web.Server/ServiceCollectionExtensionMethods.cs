using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using AbstractionsV2 = Sudoku.Web.Server.Services.Abstractions.V2;
using ServicesV2 = Sudoku.Web.Server.Services.V2;
using Sudoku.Web.Server.Services.HttpClients;

namespace Sudoku.Web.Server
{
    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection RegisterBlazorGameServices(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddScoped<AbstractionsV2.INotificationService, ServicesV2.NotificationService>()
                .AddScoped<AbstractionsV2.ILocalStorageService, ServicesV2.LocalStorageService>()
                .AddScoped<AbstractionsV2.IGameStateManager, ServicesV2.GameManager>()
                .AddScoped<AbstractionsV2.IGameStatisticsManager, ServicesV2.GameManager>()
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
