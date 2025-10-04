using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.HttpClients;

namespace Sudoku.Web.Server
{
    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection RegisterBlazorGameServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<INotificationService, NotificationService>();

            services
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<IGameManager, GameManager>()
                .AddScoped<IJsRuntimeWrapper, JsRuntimeWrapper>()
                .AddScoped<IGameTimer>(sp => new GameTimer(TimeSpan.FromSeconds(1)))
                .AddScoped<IAliasService, AliasService>();

            services.AddHttpClient<IGameApiClient, GameApiClient>(client =>
            {
                // This will be resolved by Aspire service discovery to sudoku-api service  
                client.BaseAddress = new Uri("http://sudoku-api");
            });
            
            return services;
        }
    }
}
