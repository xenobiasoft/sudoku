using Sudoku.Application.Configuration;
using Sudoku.Infrastructure.Configuration;

namespace Sudoku.Api;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiDefaults(this IServiceCollection services, IConfiguration config)
    {
        services.AddApplicationServices();
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
}