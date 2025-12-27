using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sudoku.Infrastructure.Configuration;

namespace Sudoku.Infrastructure.Services;

public class CosmosDbService : ICosmosDbService
{
    private readonly Container _container;
    private readonly ILogger<CosmosDbService> _logger;
    private readonly CosmosDbOptions _options;

    public CosmosDbService(CosmosClient cosmosClient, IOptions<CosmosDbOptions> options, ILogger<CosmosDbService> logger)
    {
        _logger = logger;
        _options = options.Value;
        
        var database = cosmosClient.GetDatabase(_options.DatabaseName);
        _container = database.GetContainer(_options.ContainerName);
    }

    public async Task DeleteItemAsync<T>(string id, string key)
    {
        try
        {
            var partitionKey = new PartitionKey(key);
            await _container.DeleteItemAsync<T>(id, partitionKey);
            _logger.LogDebug("Deleted item with ID {Id} from CosmosDB", id);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogDebug("Item with ID {Id} not found for deletion in CosmosDB", id);
            // Don't throw exception for item not found during delete
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item with ID {Id} from CosmosDB", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync<T>(string id, string key)
    {
        try
        {
            var partitionKey = new PartitionKey(key);
            var response = await _container.ReadItemAsync<T>(id, partitionKey);
            return response.Resource != null;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of item with ID {Id} in CosmosDB", id);
            throw;
        }
    }

    public async Task<T?> GetItemAsync<T>(string id, string key)
    {
        try
        {
            var partitionKey = new PartitionKey(key);
            var response = await _container.ReadItemAsync<T>(id, partitionKey);
            _logger.LogDebug("Retrieved item with ID {Id} from CosmosDB", id);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogDebug("Item with ID {Id} not found in CosmosDB", id);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving item with ID {Id} from CosmosDB", id);
            throw;
        }
    }

    public async Task<IEnumerable<T>> QueryItemsAsync<T>(string sqlQuery, IDictionary<string, string> queryParams)
    {
        var queryDefinition = new QueryDefinition(sqlQuery);

        try
        {
            foreach (var param in queryParams)
            {
                queryDefinition.WithParameter(param.Key, param.Value);
            }
            var results = new List<T>();
            using var feedIterator = _container.GetItemQueryIterator<T>(queryDefinition);

            while (feedIterator.HasMoreResults)
            {
                try
                {
                    // Set timeout for this specific operation
                    using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                    var response = await feedIterator.ReadNextAsync(cancellationTokenSource.Token);
                    results.AddRange(response);
                    
                    _logger.LogDebug("Query executed with RU charge: {RequestCharge}", response.RequestCharge);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Cosmos DB query operation timed out after 30 seconds");
                    throw new TimeoutException("The Cosmos DB query operation timed out. Please check if the emulator is running properly.");
                }
            }

            _logger.LogDebug("Query returned {Count} items from CosmosDB", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query in CosmosDB: {Query}", queryDefinition.QueryText);
            throw;
        }
    }

    public async Task<IEnumerable<T>> QueryItemsAsync<T>(string sqlQuery)
    {
        return await QueryItemsAsync<T>(sqlQuery, new Dictionary<string, string>());
    }

    public async Task<T> UpsertItemAsync<T>(T item, string? key = null)
    {
        try
        {
            var response = await _container.UpsertItemAsync(item, new PartitionKey(key));
            
            _logger.LogDebug("Upserted item to CosmosDB with RU charge: {RequestCharge}", response.RequestCharge);
            return response.Resource;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting item to CosmosDB");
            throw;
        }
    }
}