using Microsoft.Extensions.DependencyInjection;

namespace Sudoku.Application.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Common.Result).Assembly);
        });

        return services;
    }
}
