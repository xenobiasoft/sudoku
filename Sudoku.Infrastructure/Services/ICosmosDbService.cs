using Microsoft.Azure.Cosmos;

namespace Sudoku.Infrastructure.Services;

public interface ICosmosDbService
{
    Task<T?> GetItemAsync<T>(string id, PartitionKey partitionKey);
    Task<T> UpsertItemAsync<T>(T item, PartitionKey? partitionKey = null);
    Task DeleteItemAsync<T>(string id, PartitionKey partitionKey);
    Task<IEnumerable<T>> QueryItemsAsync<T>(QueryDefinition queryDefinition);
    Task<IEnumerable<T>> QueryItemsAsync<T>(string sqlQuery);
    Task<bool> ExistsAsync<T>(string id, PartitionKey partitionKey);
}