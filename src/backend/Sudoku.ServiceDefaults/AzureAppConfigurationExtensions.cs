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
        using var loggerFactory = LoggerFactory.Create(logging => logging.AddConsole());
        var logger = loggerFactory.CreateLogger(typeof(AzureAppConfigurationExtensions));

        var connectionString = builder.Configuration.GetConnectionString(connectionName);
        var appConfigSection = builder.Configuration.GetSection(connectionName);
        var endpoint = appConfigSection["Endpoint"];
        var managedIdentityEnabled = appConfigSection.GetValue<bool>("ManagedIdentityEnabled");

        if (string.IsNullOrEmpty(connectionString) && string.IsNullOrEmpty(endpoint))
        {
            throw new InvalidOperationException(
                $"Azure App Configuration connection is required but was not found. " +
                $"Please configure either 'ConnectionStrings:{connectionName}' or '{connectionName}:Endpoint' in your configuration.");
        }

        try
        {
            var useManagedIdentity = managedIdentityEnabled && !string.IsNullOrEmpty(endpoint);

            logger.LogInformation("Configuring Azure App Configuration with {Method}", useManagedIdentity ? "endpoint + managed identity" : "connection string");

            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                if (useManagedIdentity)
                {
                    logger.LogDebug("Using endpoint '{Endpoint}' with DefaultAzureCredential for Azure App Configuration", endpoint);
                    var endpointUri = new Uri(endpoint);
                    options.Connect(endpointUri, new DefaultAzureCredential());
                }
                else if (!string.IsNullOrEmpty(connectionString))
                {
                    logger.LogDebug("Using connection string for Azure App Configuration");
                    options.Connect(connectionString);
                }
                else if (!string.IsNullOrEmpty(endpoint))
                {
                    logger.LogDebug("Using endpoint '{Endpoint}' with DefaultAzureCredential for Azure App Configuration (fallback)", endpoint);
                    var endpointUri = new Uri(endpoint);
                    options.Connect(endpointUri, new DefaultAzureCredential());
                }

                var keyFilter = builder.Configuration["AzureAppConfiguration:KeyFilter"] ?? "*";
                var labelFilter = builder.Configuration["AzureAppConfiguration:LabelFilter"] ?? builder.Environment.EnvironmentName;
                
                logger.LogDebug("Using key filter '{KeyFilter}' and label filter '{LabelFilter}'", keyFilter, labelFilter);
                options.Select(keyFilter, labelFilter);

                if (int.TryParse(builder.Configuration["AzureAppConfiguration:RefreshInterval"], out var refreshInterval))
                {
                    logger.LogDebug("Configuring refresh with interval of {RefreshInterval} seconds", refreshInterval);
                    options.ConfigureRefresh(refresh =>
                    {
                        refresh.Register(keyFilter, labelFilter, refreshAll: true)
                               .SetCacheExpiration(TimeSpan.FromSeconds(refreshInterval));
                    });
                }

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

                configureOptions?.Invoke(options);
            });

            builder.Services.AddAzureAppConfiguration();
            
            logger.LogInformation("Azure App Configuration successfully configured");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to configure Azure App Configuration for '{ConnectionName}'", connectionName);
            throw;
        }

        return builder;
    }
}
