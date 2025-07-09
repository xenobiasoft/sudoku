using XenobiaSoft.Sudoku.Services;

namespace Sudoku.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiDefaults(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IPlayerService, PlayerService>();

        return services;
    }
}