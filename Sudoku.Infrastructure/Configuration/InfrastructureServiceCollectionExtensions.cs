using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Events;
using Sudoku.Infrastructure.EventHandling;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Services;
using Sudoku.Infrastructure.Services.Strategies;

namespace Sudoku.Infrastructure.Configuration;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureStorageOptions>(configuration.GetSection(AzureStorageOptions.SectionName));

        services.AddAzureStorageServices(configuration);
        services.AddDomainEventHandling();
        services.AddPuzzleServices();
        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddPuzzleServices(this IServiceCollection services)
    {
        services.AddScoped<IPuzzleGenerator, PuzzleGenerator>();
        services.AddScoped<IPuzzleSolver, StrategyBasedPuzzleSolver>();

        typeof(SudokuPuzzle).Assembly
            .GetTypes()
            .Where(x => x.Name.EndsWith("Strategy") && x is { IsAbstract: false, IsInterface: false })
            .ToList()
            .ForEach(x =>
            {
                services.AddTransient(typeof(SolverStrategy), x);
            });

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

        services.AddScoped<IAzureStorageService, AzureStorageService>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGameRepository, AzureBlobGameRepository>();
        services.AddScoped<IPuzzleRepository, InMemoryPuzzleRepository>();

        return services;
    }

    private static IServiceCollection AddDomainEventHandling(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddScoped<IDomainEventHandler<GameCreatedEvent>, GameCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<MoveMadeEvent>, MoveMadeEventHandler>();
        services.AddScoped<IDomainEventHandler<GameCompletedEvent>, GameCompletedEventHandler>();

        return services;
    }
}