using Microsoft.Azure.Cosmos;
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
        services.Configure<CosmosDbOptions>(configuration.GetSection(CosmosDbOptions.SectionName));

        // Check which persistence provider to use
        var useCosmosDb = configuration.GetValue<bool>("UseCosmosDb");
        
        if (useCosmosDb)
        {
            services.AddCosmosDbServices(configuration);
        }
        else
        {
            services.AddAzureStorageServices(configuration);
        }

        services.AddDomainEventHandling();
        services.AddPuzzleServices();

        return services;
    }

    private static IServiceCollection AddCosmosDbServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register CosmosClient manually
        services.AddSingleton<CosmosClient>(serviceProvider =>
        {
            var connectionString = configuration.GetConnectionString("CosmosDb");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "CosmosDb connection string not found. Please ensure it's configured in configuration " +
                    "with the key 'ConnectionStrings:cosmosdb' or 'ConnectionStrings:CosmosDb'.");
            }

            var clientOptions = new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                RequestTimeout = TimeSpan.FromSeconds(30),
                MaxRetryAttemptsOnRateLimitedRequests = 3,
                MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(15)
            };

            // Get CosmosDbOptions from configuration
            var cosmosDbOptions = configuration.GetSection(CosmosDbOptions.SectionName).Get<CosmosDbOptions>();

            if (cosmosDbOptions.DisableSslValidation)
            {
                // Disable SSL validation for development/emulator
                clientOptions.HttpClientFactory = () =>
                {
                    HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
                    };

                    return new HttpClient(httpMessageHandler);
                };
            }

            return new CosmosClient(connectionString, clientOptions);
        });

        // Register CosmosDb service and repository
        services.AddScoped<ICosmosDbService, CosmosDbService>();
        services.AddScoped<IGameRepository, CosmosDbGameRepository>();
        
        // Keep the in-memory puzzle repository for now
        services.AddScoped<IPuzzleRepository, InMemoryPuzzleRepository>();

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