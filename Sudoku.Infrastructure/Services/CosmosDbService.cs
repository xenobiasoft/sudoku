using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sudoku.Infrastructure.Configuration;

namespace Sudoku.Infrastructure.Services;

public class CosmosDbService(
    CosmosClient cosmosClient,
    IOptions<CosmosDbOptions> options,
    ILogger<CosmosDbService> logger)
    : ICosmosDbService
{
    private readonly CosmosDbOptions _options = options.Value;

    private Container? _container;

    public async Task DeleteItemAsync<T>(string id, string key)
    {
        try
        {
            if (_container == null)
            {
                await EnsureDatabaseExistsAsync();
            }

            var partitionKey = new PartitionKey(key);
            await _container!.DeleteItemAsync<T>(id, partitionKey);
            logger.LogDebug("Deleted item with ID {Id} from CosmosDB", id);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogInformation("Item with ID {Id} not found for deletion in CosmosDB", id);
            // Don't throw exception for item not found during delete
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting item with ID {Id} from CosmosDB", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync<T>(string id, string key)
    {
        try
        {
            if (_container == null)
            {
                await EnsureDatabaseExistsAsync();
            }

            var partitionKey = new PartitionKey(key);
            var response = await _container!.ReadItemAsync<T>(id, partitionKey);
            return response.Resource != null;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking existence of item with ID {Id} in CosmosDB", id);
            throw;
        }
    }

    public async Task<T?> GetItemAsync<T>(string id, string key)
    {
        try
        {
            if (_container == null)
            {
                await EnsureDatabaseExistsAsync();
            }

            var partitionKey = new PartitionKey(key);
            var response = await _container!.ReadItemAsync<T>(id, partitionKey);
            logger.LogDebug("Retrieved item with ID {Id} from CosmosDB", id);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogDebug("Item with ID {Id} not found in CosmosDB", id);
            return default;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving item with ID {Id} from CosmosDB", id);
            throw;
        }
    }

    public async Task<IEnumerable<T>> QueryItemsAsync<T>(string sqlQuery, IDictionary<string, string> queryParams)
    {
        var queryDefinition = new QueryDefinition(sqlQuery);

        try
        {
            if (_container == null)
            {
                await EnsureDatabaseExistsAsync();
            }

            foreach (var param in queryParams)
            {
                queryDefinition.WithParameter(param.Key, param.Value);
            }
            var results = new List<T>();
            using var feedIterator = _container!.GetItemQueryIterator<T>(queryDefinition);

            while (feedIterator.HasMoreResults)
            {
                try
                {
                    using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                    var response = await feedIterator.ReadNextAsync(cancellationTokenSource.Token);
                    results.AddRange(response);
                    
                    logger.LogDebug("Query executed with RU charge: {RequestCharge}", response.RequestCharge);
                }
                catch (OperationCanceledException)
                {
                    logger.LogWarning("Cosmos DB query operation timed out after 30 seconds");
                    throw new TimeoutException("The Cosmos DB query operation timed out. Please check if the emulator is running properly.");
                }
            }

            logger.LogDebug("Query returned {Count} items from CosmosDB", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing query in CosmosDB: {Query}", queryDefinition.QueryText);
            throw;
        }
    }

    public async Task<IEnumerable<T>> QueryItemsAsync<T>(string sqlQuery)
    {
        if (_container == null)
        {
            await EnsureDatabaseExistsAsync();
        }

        return await QueryItemsAsync<T>(sqlQuery, new Dictionary<string, string>());
    }

    public async Task<T> UpsertItemAsync<T>(T item, string? key = null)
    {
        try
        {
            if (_container == null)
            {
                await EnsureDatabaseExistsAsync();
            }

            var response = await _container!.UpsertItemAsync(item, new PartitionKey(key));
            
            logger.LogDebug("Upserted item to CosmosDB with RU charge: {RequestCharge}", response.RequestCharge);
            return response.Resource;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error upserting item to CosmosDB");
            throw;
        }
    }

    private async Task EnsureDatabaseExistsAsync()
    {
        try
        {
            await InitializeCosmosDbAsync();

            var database = cosmosClient.GetDatabase(_options.DatabaseName);
            _container = database.GetContainer(_options.ContainerName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error initializing CosmosDB: {Message}", ex.Message);
            throw;
        }
    }

    private async Task InitializeCosmosDbAsync()
    {
        logger.LogInformation("Ensuring CosmosDB database and container exist at endpoint URI: {Endpoint}", cosmosClient.Endpoint);

        var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(
            _options.DatabaseName,
            throughput: 400);

        logger.LogInformation("Database {DatabaseName} ensured", _options.DatabaseName);

        var containerProperties = new ContainerProperties(
            id: _options.ContainerName,
            partitionKeyPath: "/gameId"
        );

        await database.Database.CreateContainerIfNotExistsAsync(containerProperties);

        logger.LogInformation("Container {ContainerName} ensured", _options.ContainerName);
    }
}