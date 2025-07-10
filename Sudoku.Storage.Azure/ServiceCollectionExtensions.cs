using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.Storage.Azure.GameState;
using XenobiaSoft.Sudoku.Storage.Azure.GameState.Decorators;

namespace XenobiaSoft.Sudoku.Storage.Azure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureStorage(this IServiceCollection services, IConfiguration configuration)
    {
        // Try to get the connection string from key-vault first, then fall back to configuration
        var connectionString = configuration["AzureStorage:ConnectionString"]
            ?? configuration["ConnectionStrings:AzureStorage"]
            ?? configuration["AzureStorageConnection"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Azure Storage connection string not found. Please ensure it's configured in Azure Key Vault " +
                "with the key 'AzureStorage-ConnectionString' or in configuration with key 'AzureStorage:ConnectionString'.");
        }

        services.AddAzureClients(builder =>
        {
            builder.UseCredential(new DefaultAzureCredential());
            builder.AddBlobServiceClient(connectionString);
        });

        services.AddScoped<IStorageService, Services.AzureStorageService>()
            .AddScoped<IPersistentGameStateStorage, AzureBlobGameStateStorage>()
            .AddScoped<IPersistentGameStateStorage>(x =>
                ActivatorUtilities.CreateInstance<CachingAzureBlobGameStateStorageDecorator>(x,
                    ActivatorUtilities.CreateInstance<AzureBlobGameStateStorage>(x)));

        return services;
    }
}