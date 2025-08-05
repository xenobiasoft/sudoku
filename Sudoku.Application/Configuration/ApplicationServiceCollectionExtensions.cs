using Microsoft.Extensions.DependencyInjection;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Services;

namespace Sudoku.Application.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR and discover handlers
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(typeof(Common.Result).Assembly);
        });

        // Register application services
        services.AddScoped<IGameApplicationService, GameApplicationService>();
        services.AddScoped<IPlayerApplicationService, PlayerApplicationService>();

        return services;
    }
}