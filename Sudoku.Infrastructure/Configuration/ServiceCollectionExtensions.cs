using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Events;
using Sudoku.Infrastructure.EventHandling;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Services;

namespace Sudoku.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Azure Storage options
        services.Configure<AzureStorageOptions>(
            configuration.GetSection(AzureStorageOptions.SectionName));

        // Register Azure Storage services
        services.AddAzureStorageServices(configuration);

        // Register repositories
        services.AddRepositories();

        // Register domain event handling
        services.AddDomainEventHandling();

        return services;
    }

    private static IServiceCollection AddAzureStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        var storageOptions = configuration.GetSection(AzureStorageOptions.SectionName).Get<AzureStorageOptions>();

        if (storageOptions?.UseManagedIdentity == true)
        {
            // Use managed identity for Azure Storage
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(new Uri($"https://{storageOptions.AccountName}.blob.core.windows.net/"));
            });
        }
        else
        {
            // Use connection string for Azure Storage
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(storageOptions?.ConnectionString ??
                    configuration.GetConnectionString("AzureStorage"));
            });
        }

        // Register Azure Storage service
        services.AddScoped<IAzureStorageService, AzureStorageService>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register the repository interface with the Azure implementation
        services.AddScoped<IGameRepository, AzureBlobGameRepository>();

        return services;
    }

    private static IServiceCollection AddDomainEventHandling(this IServiceCollection services)
    {
        // Register domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register domain event handlers
        services.AddScoped<IDomainEventHandler<GameCreatedEvent>, GameCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<MoveMadeEvent>, MoveMadeEventHandler>();
        services.AddScoped<IDomainEventHandler<GameCompletedEvent>, GameCompletedEventHandler>();

        return services;
    }
}