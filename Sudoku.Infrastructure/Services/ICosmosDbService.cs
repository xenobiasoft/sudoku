using Microsoft.Azure.Cosmos;

namespace Sudoku.Infrastructure.Services;

public interface ICosmosDbService
{
    Task DeleteItemAsync<T>(string id, string key);
    Task<bool> ExistsAsync<T>(string id, string key);
    Task<T?> GetItemAsync<T>(string id, string key);
    Task<IEnumerable<T>> QueryItemsAsync<T>(string sqlQuery, IDictionary<string, string> queryParams);
    Task<IEnumerable<T>> QueryItemsAsync<T>(string sqlQuery);
    Task<T> UpsertItemAsync<T>(T item, string? key = null);
}