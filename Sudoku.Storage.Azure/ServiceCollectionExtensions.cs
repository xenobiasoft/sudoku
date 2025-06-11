using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;
using Xenobiasoft.Sudoku.Storage.Azure.GameState;

namespace Xenobiasoft.Sudoku.Storage.Azure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(builder =>
        {
            builder.UseCredential(new DefaultAzureCredential());
            builder.AddBlobServiceClient(configuration["AzureStorageConnection"]);
        });

        services.AddScoped<IStorageService, Services.AzureStorageService>();
        services.AddScoped<IPersistentGameStateStorage, AzureBlobGameStateStorage>();

        return services;
    }
}