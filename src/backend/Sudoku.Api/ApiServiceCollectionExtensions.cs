using Sudoku.Application.Configuration;
using Sudoku.Infrastructure.Configuration;

namespace Sudoku.Api;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiDefaults(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(config);

        // Add CORS policy
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                if (env.IsDevelopment())
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                }
                else
                {
                    // In production, configure specific allowed origins
                    var allowedOrigins = config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                }
            });
        });

        return services;
    }
}