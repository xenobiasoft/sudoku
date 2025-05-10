using Azure.Identity;
using BlazorApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Web.Server;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ILogger<Program>? logger = null;

        try
        {
            builder
                .Logging
                .ClearProviders()
                .AddConsole()
                .AddDebug()
                .AddAzureWebAppDiagnostics();

            var vaultUri = builder.Configuration["KeyVault:VaultUri"] ?? string.Empty;
            if (string.IsNullOrEmpty(vaultUri))
            {
                throw new InvalidOperationException("Key Vault URI is not configured.");
            }

            builder
                .Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential());

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddHealthChecks();
            builder.Services
                .RegisterGameServices(builder.Configuration)
                .RegisterBlazorGameServices()
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