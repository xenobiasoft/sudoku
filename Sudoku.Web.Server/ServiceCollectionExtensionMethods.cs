using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.Storage.Azure;

namespace Sudoku.Web.Server
{
    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection RegisterBlazorGameServices(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddScoped<ICellFocusedNotificationService, CellFocusedNotificationService>()
                .AddScoped<IInvalidCellNotificationService, InvalidCellNotificationService>()
                .AddScoped<IGameNotificationService, GameNotificationService>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<IGameManager, GameManager>()
                .AddScoped<IJsRuntimeWrapper, JsRuntimeWrapper>()
                .AddScoped<IGameTimer>(sp => new GameTimer(TimeSpan.FromSeconds(1)))
                .AddScoped<IAliasService, AliasService>();

            services.AddAzureStorage(config);

            return services;
        }
    }
}
