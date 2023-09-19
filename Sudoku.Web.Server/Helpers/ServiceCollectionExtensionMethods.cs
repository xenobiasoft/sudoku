using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Helpers
{
    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection RegisterBlazorGameServices(this IServiceCollection services)
        {
            services.AddSingleton<ICellFocusedNotificationService, CellFocusedNotificationService>();
            services.AddSingleton<IInvalidCellNotificationService, InvalidCellNotificationService>();
            services.AddSingleton<IGameNotificationService, GameNotificationService>();

            return services;
        }
    }
}
