using Microsoft.Extensions.Http.Resilience;
using Sudoku.Blazor.Services;
using Sudoku.Blazor.Services.Abstractions;
using Sudoku.Blazor.Services.HttpClients;

namespace Sudoku.Blazor
{
    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection RegisterBlazorGameServices(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<IGameStateManager, GameManager>()
                .AddScoped<IGameStatisticsManager, GameManager>()
                .AddScoped<IGameManager, GameManager>()
                .AddScoped<IPlayerManager, PlayerManager>()
                .AddScoped<IJsRuntimeWrapper, JsRuntimeWrapper>()
                .AddScoped<IGameTimer>(sp => new GameTimer(TimeSpan.FromSeconds(1)));

            var apiBaseUrl = config["ApiBaseUrl"];

            // Shared resilience configuration for all API clients.
            // Tweak these values while testing to find the right balance.
            static void ConfigureResilience(HttpStandardResilienceOptions options)
            {
                // Per-attempt timeout.
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(20);

                // 0 retries = 1 total attempt. Bump this as needed while testing.
                options.Retry.MaxRetryAttempts = 1;

                // TotalRequestTimeout must be strictly greater than AttemptTimeout.
                // Keep it at least AttemptTimeout * (MaxRetryAttempts + 1) + a small buffer.
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(25);

                // CircuitBreaker.SamplingDuration must be >= 2 * AttemptTimeout.
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
            }

            services.AddHttpClient<IGameApiClient, GameApiClient>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            })
            .AddStandardResilienceHandler(ConfigureResilience);

            services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            })
            .AddStandardResilienceHandler(ConfigureResilience);

            return services;
        }
    }
}
