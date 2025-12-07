using Azure.Identity;
using BlazorApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Sudoku.Web.Server.Services.HttpClients;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Web.Server;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        ILogger<Program>? logger = null;

        try
        {
            builder
                .Logging
                .ClearProviders()
                .AddConsole()
                .AddDebug()
                .AddAzureWebAppDiagnostics();

            builder.Configuration.AddEnvironmentVariables();

            // Try to get the key vault URI from the connection string (Aspire way) or direct configuration
            var vaultUri = builder.Configuration["ConnectionStrings:AzureKeyVault"]
                ?? builder.Configuration["KeyVault:VaultUri"]
                ?? string.Empty;

            if (!string.IsNullOrEmpty(vaultUri))
            {
                builder.Configuration.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential());
            }

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddHealthChecks();

            // Configure HttpClient for GameApiClient with custom resilience settings
            builder.Services.AddHttpClient<IGameApiClient, GameApiClient>(client =>
                {
                    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001";
                    client.BaseAddress = new Uri(apiBaseUrl);
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

            builder.Services
                .RegisterBlazorGameServices(builder.Configuration)
                .AddBlazorApplicationInsights(x =>
                {
                    x.InstrumentationKey = builder.Configuration["AppInsightsKey"];
                })
                .AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
                {
                    ConnectionString = builder.Configuration["AppInsightsConnectionString"]
                })
                .AddLogging(logging =>
                {
                    logging
                        .AddApplicationInsights()
                        .AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Information)
                        .AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
                });

            var app = builder.Build();

            app.MapDefaultEndpoints();

            logger = app.Services.GetRequiredService<ILogger<Program>>();

            if (!app.Environment.IsDevelopment())
            {
                logger.LogInformation("Configuring exception handler and HSTS for production environment.");
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            logger.LogInformation("Configuring middleware pipeline.");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            logger.LogInformation("Mapping endpoints.");
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            app.MapHealthChecks("/health-check");

            logger.LogInformation("Starting the application.");
            app.Run();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An unhandled exception occurred.");
            throw;
        }
    }
}