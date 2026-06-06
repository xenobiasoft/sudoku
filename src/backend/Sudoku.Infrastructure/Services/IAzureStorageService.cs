namespace Sudoku.Infrastructure.Services;

public interface IAzureStorageService
{
    Task<T?> LoadAsync<T>(string containerName, string blobName);
    Task SaveAsync<T>(string containerName, string blobName, T data);
    Task DeleteAsync(string containerName, string blobName);
    Task<bool> ExistsAsync(string containerName, string blobName);
    IAsyncEnumerable<string> GetBlobNamesAsync(string containerName, string? prefix = null);
}