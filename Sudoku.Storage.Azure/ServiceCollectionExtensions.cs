using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;
using XenobiaSoft.Sudoku.Storage.Azure.GameState;
using XenobiaSoft.Sudoku.Storage.Azure.GameState.Decorators;

namespace XenobiaSoft.Sudoku.Storage.Azure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(builder =>
        {
            builder.UseCredential(new DefaultAzureCredential());
            builder.AddBlobServiceClient(configuration["AzureStorageConnection"]);
        });

        services.AddScoped<IStorageService, Services.AzureStorageService>()
            .AddScoped<IPersistentGameStateStorage, AzureBlobGameStateStorage>()
            .AddScoped<IPersistentGameStateStorage>(x =>
                ActivatorUtilities.CreateInstance<CachingAzureBlobGameStateStorageDecorator>(x,
                    ActivatorUtilities.CreateInstance<AzureBlobGameStateStorage>(x)));

        return services;
    }
}