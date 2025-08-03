using Sudoku.Application.Interfaces;
using Sudoku.Application.Services;
using Sudoku.Infrastructure.Configuration;

namespace Sudoku.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiDefaults(this IServiceCollection services, IConfiguration config)
    {
        // Register application services
        services.AddApplicationServices();
        
        // Register infrastructure services
        services.AddInfrastructureServices(config);

        // Add CORS policy
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR and discover handlers
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(typeof(Sudoku.Application.Common.Result).Assembly);
        });

        // Register application services
        services.AddScoped<IGameApplicationService, GameApplicationService>();
        services.AddScoped<IPlayerApplicationService, PlayerApplicationService>();

        return services;
    }
}