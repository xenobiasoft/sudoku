using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.HttpClients;

namespace Sudoku.Web.Server
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

            // Configure HttpClient for GameApiClient
            // In development with Aspire, use service discovery (http://sudoku-api)
            // In production, use the ApiBaseUrl from configuration or environment variable
            var apiBaseUrl = config["ApiBaseUrl"];
            
            services.AddHttpClient<IGameApiClient, GameApiClient>(client =>
            {
                if (!string.IsNullOrEmpty(apiBaseUrl))
                {
                    // Production: Use explicit URL from configuration
                    client.BaseAddress = new Uri(apiBaseUrl);
                }
                else
                {
                    // Development: Use Aspire service discovery
                    client.BaseAddress = new Uri("http://sudoku-api");
                }
            })
            .AddStandardResilienceHandler(options =>
            {
                // Configure retry policy to only retry idempotent methods (GET, HEAD, OPTIONS, etc.)
                options.Retry.ShouldHandle = args =>
                {
                    // Only retry for GET requests
                    if (args.Outcome.Result?.RequestMessage?.Method == HttpMethod.Get)
                    {
                        return ValueTask.FromResult(args.Outcome.Result.StatusCode >= System.Net.HttpStatusCode.InternalServerError);
                    }

                    // Don't retry POST, PUT, DELETE, PATCH
                    return ValueTask.FromResult(false);
                };

                // Reduce max retry attempts
                options.Retry.MaxRetryAttempts = 2;
                options.Retry.Delay = TimeSpan.FromMilliseconds(500);
            });

            // Configure HttpClient for PlayerApiClient with the same logic
            services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(client =>
            {
                if (!string.IsNullOrEmpty(apiBaseUrl))
                {
                    // Production: Use explicit URL from configuration
                    client.BaseAddress = new Uri(apiBaseUrl);
                }
                else
                {
                    // Development: Use Aspire service discovery
                    client.BaseAddress = new Uri("http://sudoku-api");
                }
            })
            .AddStandardResilienceHandler();
            
            return services;
        }
    }
}
