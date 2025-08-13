using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Logging;
using Azure.Identity;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extensions for configuring Azure App Configuration with advanced options
/// </summary>
public static class AzureAppConfigurationExtensions
{
    /// <summary>
    /// Adds Azure App Configuration with advanced configuration options
    /// </summary>
    /// <param name="builder">The host application builder</param>
    /// <param name="connectionName">The connection string name (default: "appconfig")</param>
    /// <param name="configureOptions">Optional configuration action</param>
    /// <returns>The builder for chaining</returns>
    public static TBuilder AddAzureAppConfigurationAdvanced<TBuilder>(this TBuilder builder, string connectionName = "appconfig", Action<AzureAppConfigurationOptions>? configureOptions = null) where TBuilder : IHostApplicationBuilder
    {
        // Create a temporary logger for this configuration process
        using var loggerFactory = LoggerFactory.Create(logging => logging.AddConsole());
        var logger = loggerFactory.CreateLogger(typeof(AzureAppConfigurationExtensions));

        try
        {
            // Try to get connection string first
            var connectionString = builder.Configuration.GetConnectionString(connectionName);
            
            // If no connection string, try to get from the appconfig section
            var appConfigSection = builder.Configuration.GetSection(connectionName);
            var endpoint = appConfigSection["Endpoint"];
            var managedIdentityEnabled = appConfigSection.GetValue<bool>("ManagedIdentityEnabled");

            // If neither connection string nor endpoint is provided, skip configuration
            if (string.IsNullOrEmpty(connectionString) && string.IsNullOrEmpty(endpoint))
            {
                logger.LogInformation("Azure App Configuration skipped - no connection string or endpoint provided for '{ConnectionName}'", connectionName);
                return builder;
            }

            logger.LogInformation("Configuring Azure App Configuration with {Method}", 
                !string.IsNullOrEmpty(connectionString) ? "connection string" : "endpoint + managed identity");

            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                // Configure connection - prefer connection string over endpoint + managed identity
                if (!string.IsNullOrEmpty(connectionString))
                {
                    logger.LogDebug("Using connection string for Azure App Configuration");
                    options.Connect(connectionString);
                }
                else if (!string.IsNullOrEmpty(endpoint))
                {
                    logger.LogDebug("Using endpoint '{Endpoint}' with managed identity for Azure App Configuration", endpoint);
                    var endpointUri = new Uri(endpoint);
                    options.Connect(endpointUri, new DefaultAzureCredential());
                }

                // Configure key filters from settings
                var keyFilter = builder.Configuration["AzureAppConfiguration:KeyFilter"] ?? "*";
                var labelFilter = builder.Configuration["AzureAppConfiguration:LabelFilter"] ?? builder.Environment.EnvironmentName;
                
                logger.LogDebug("Using key filter '{KeyFilter}' and label filter '{LabelFilter}'", keyFilter, labelFilter);
                options.Select(keyFilter, labelFilter);

                // Configure refresh settings
                if (int.TryParse(builder.Configuration["AzureAppConfiguration:RefreshInterval"], out var refreshInterval))
                {
                    logger.LogDebug("Configuring refresh with interval of {RefreshInterval} seconds", refreshInterval);
                    options.ConfigureRefresh(refresh =>
                    {
                        refresh.Register(keyFilter, labelFilter, refreshAll: true)
                               .SetCacheExpiration(TimeSpan.FromSeconds(refreshInterval));
                    });
                }

                // Configure feature flags if enabled
                var featureFlagsEnabled = builder.Configuration.GetValue<bool>("AzureAppConfiguration:FeatureFlags:Enabled");
                if (featureFlagsEnabled)
                {
                    logger.LogDebug("Feature flags enabled for Azure App Configuration");
                    options.UseFeatureFlags(featureFlags =>
                    {
                        featureFlags.Label = labelFilter;
                        
                        if (int.TryParse(builder.Configuration["AzureAppConfiguration:FeatureFlags:RefreshInterval"], out var flagRefreshInterval))
                        {
                            logger.LogDebug("Feature flags refresh interval set to {RefreshInterval} seconds", flagRefreshInterval);
                            featureFlags.CacheExpirationInterval = TimeSpan.FromSeconds(flagRefreshInterval);
                        }
                    });
                }

                // Apply custom configuration if provided
                configureOptions?.Invoke(options);
            });

            // Add Azure App Configuration services for refresh capabilities
            builder.Services.AddAzureAppConfiguration();
            
            logger.LogInformation("Azure App Configuration successfully configured");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to configure Azure App Configuration for '{ConnectionName}'", connectionName);
            // Don't throw - allow the application to continue without Azure App Configuration
        }

        return builder;
    }
}