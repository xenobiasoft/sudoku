using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Events;
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

            services.AddCosmosDbServices();
            services.AddBlobStorageClient(configuration);
            services.AddPuzzlePoolServices();
            services.AddDomainEventHandling();
            services.AddPuzzleServices();

            return services;
        }

        private IServiceCollection AddBlobStorageClient(IConfiguration configuration)
        {
            var storageOptions = configuration.GetSection(AzureStorageOptions.SectionName).Get<AzureStorageOptions>();

            // Aspire-injected connection string takes priority (covers local emulator)
            var aspireBlobsConnectionString = configuration.GetConnectionString("blobs");

            if (!string.IsNullOrEmpty(aspireBlobsConnectionString))
            {
                services.AddAzureClients(builder =>
                {
                    builder.AddBlobServiceClient(aspireBlobsConnectionString);
                });
            }
            else if (storageOptions?.UseManagedIdentity == true)
            {
                services.AddAzureClients(builder =>
                {
                    builder.AddBlobServiceClient(new Uri($"https://{storageOptions.AccountName}.blob.core.windows.net/"));
                });
            }
            else
            {
                var connectionString = storageOptions?.ConnectionString
                    ?? configuration.GetConnectionString("AzureStorage")
                    ?? configuration["AzureWebJobsStorage"]
                    ?? throw new InvalidOperationException(
                        "Blob storage is not configured. Provide one of: ConnectionStrings:blobs (Aspire), " +
                        "AzureStorage:ConnectionString, AzureStorage:UseManagedIdentity + AzureStorage:AccountName, " +
                        "ConnectionStrings:AzureStorage, or AzureWebJobsStorage.");

                services.AddAzureClients(builder =>
                {
                    builder.AddBlobServiceClient(connectionString);
                });
            }

            services.AddScoped<IAzureStorageService, AzureStorageService>();
            return services;
        }

        private IServiceCollection AddCosmosDbServices()
        {
            services.AddScoped<ICosmosDbService, CosmosDbService>();
            services.AddScoped<IGameRepository, CosmosDbGameRepository>();
            services.AddScoped<IUserProfileRepository, CosmosDbUserProfileRepository>();
            services.AddScoped<IPuzzleRepository, InMemoryPuzzleRepository>();

            return services;
        }

        private IServiceCollection AddPuzzleServices()
        {
            services.AddScoped<IPuzzleGenerator, UniqueSolutionPuzzleGenerator>();
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
