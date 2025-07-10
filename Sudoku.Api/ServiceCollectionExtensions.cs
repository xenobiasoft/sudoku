using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.Services;
using XenobiaSoft.Sudoku.Storage.Azure;

namespace Sudoku.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiDefaults(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IGameService, GameService>();

        services.AddAzureStorage(config);

        return services;
    }
}