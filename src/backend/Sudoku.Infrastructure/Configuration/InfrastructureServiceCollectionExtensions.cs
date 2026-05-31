using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Events;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.EventHandling;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Services;
using Sudoku.Infrastructure.Services.Strategies;

namespace Sudoku.Infrastructure.Configuration;

public static class InfrastructureServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureServices(IConfiguration configuration)
        {
            services.Configure<AzureStorageOptions>(configuration.GetSection(AzureStorageOptions.SectionName));
            services.Configure<CosmosDbOptions>(configuration.GetSection(CosmosDbOptions.SectionName));

            var useCosmosDb = configuration.GetValue<bool>("UseCosmosDb");

            if (useCosmosDb)
            {
                services.AddCosmosDbServices(configuration);
            }
            else
            {
                services.AddAzureStorageServices(configuration);
            }

            // Register blob storage client for puzzle pool if configured; otherwise use no-op fallback
            // so the app starts cleanly even without blob storage (on-demand generation is the fallback).
            if (IsBlobStorageConfigured(configuration))
            {
                if (useCosmosDb)
                {
                    services.AddBlobStorageClient(configuration);
                }

                services.AddPuzzlePoolServices();
            }
            else
            {
                services.AddScoped<IPuzzlePoolService, NullPuzzlePoolService>();
            }

            services.AddDomainEventHandling();
            services.AddPuzzleServices();

            return services;
        }

        private static bool IsBlobStorageConfigured(IConfiguration configuration)
        {
            var storageOptions = configuration.GetSection(AzureStorageOptions.SectionName).Get<AzureStorageOptions>();
            return (storageOptions?.UseManagedIdentity == true && !string.IsNullOrEmpty(storageOptions.AccountName))
                || !string.IsNullOrEmpty(storageOptions?.ConnectionString)
                || !string.IsNullOrEmpty(configuration.GetConnectionString("AzureStorage"));
        }

        private IServiceCollection AddBlobStorageClient(IConfiguration configuration)
        {
            var storageOptions = configuration.GetSection(AzureStorageOptions.SectionName).Get<AzureStorageOptions>();

            if (storageOptions?.UseManagedIdentity == true)
            {
                services.AddAzureClients(builder =>
                {
                    builder.AddBlobServiceClient(new Uri($"https://{storageOptions.AccountName}.blob.core.windows.net/"));
                });
            }
            else
            {
                services.AddAzureClients(builder =>
                {
                    builder.AddBlobServiceClient(storageOptions?.ConnectionString ??
                                                 configuration.GetConnectionString("AzureStorage"));
                });
            }

            services.AddScoped<IAzureStorageService, AzureStorageService>();
            return services;
        }

        private IServiceCollection AddCosmosDbServices(IConfiguration configuration)
        {
            services.AddScoped<ICosmosDbService, CosmosDbService>();
            services.AddScoped<IGameRepository, CosmosDbGameRepository>();
            services.AddScoped<IUserProfileRepository, CosmosDbUserProfileRepository>();
            services.AddScoped<IPuzzleRepository, InMemoryPuzzleRepository>();

            return services;
        }

        private IServiceCollection AddAzureStorageServices(IConfiguration configuration)
        {
            services.AddBlobStorageClient(configuration);
            services.AddScoped<IGameRepository, AzureBlobGameRepository>();
            services.AddScoped<IPuzzleRepository, InMemoryPuzzleRepository>();

            return services;
        }

        private IServiceCollection AddPuzzleServices()
        {
            services.AddScoped<IPuzzleGenerator, PuzzleGenerator>();
            services.AddScoped<IPuzzleSolver, StrategyBasedPuzzleSolver>();

            typeof(SolverStrategy).Assembly
                .GetTypes()
                .Where(x => x.Name.EndsWith("Strategy") && x is { IsAbstract: false, IsInterface: false })
                .ToList()
                .ForEach(x =>
                {
                    services.AddTransient(typeof(SolverStrategy), x);
                });

            return services;
        }

        private IServiceCollection AddPuzzlePoolServices()
        {
            services.AddScoped<AzureBlobPuzzleRepository>();
            services.AddScoped<IPuzzleBlobStorage>(sp => sp.GetRequiredService<AzureBlobPuzzleRepository>());
            services.AddScoped<IPuzzlePoolService, PuzzlePoolService>();

            return services;
        }

        private IServiceCollection AddDomainEventHandling()
        {
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            services.AddScoped<IDomainEventHandler<GameCreatedEvent>, GameCreatedEventHandler>();
            services.AddScoped<IDomainEventHandler<MoveMadeEvent>, MoveMadeEventHandler>();
            services.AddScoped<IDomainEventHandler<GameCompletedEvent>, GameCompletedEventHandler>();

            return services;
        }
    }
}

file sealed class NullPuzzlePoolService : IPuzzlePoolService
{
    public Task<int> GetAvailableCountAsync(GameDifficulty difficulty) => Task.FromResult(0);
    public Task SeedAsync(GameDifficulty difficulty, int count) => Task.CompletedTask;
    public Task<SudokuPuzzle?> DequeueAsync(GameDifficulty difficulty) => Task.FromResult<SudokuPuzzle?>(null);
}
