using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace XenobiaSoft.Sudoku.Infrastructure.Services;

public interface IAzureStorageService
{
    Task<T?> LoadAsync<T>(string containerName, string blobName);
    Task SaveAsync<T>(string containerName, string blobName, T data);
    Task DeleteAsync(string containerName, string blobName);
    Task<bool> ExistsAsync(string containerName, string blobName);
    IAsyncEnumerable<string> GetBlobNamesAsync(string containerName, string? prefix = null);
}

public class AzureStorageService : IAzureStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureStorageService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AzureStorageService(BlobServiceClient blobServiceClient, ILogger<AzureStorageService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<T?> LoadAsync<T>(string containerName, string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                _logger.LogDebug("Blob {BlobName} not found in container {ContainerName}", blobName, containerName);
                return default;
            }

            var response = await blobClient.DownloadAsync();
            using var streamReader = new StreamReader(response.Value.Content);
            var json = await streamReader.ReadToEndAsync();
            var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);

            _logger.LogDebug("Successfully loaded blob {BlobName} from container {ContainerName}", blobName, containerName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading blob {BlobName} from container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    public async Task SaveAsync<T>(string containerName, string blobName, T data)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(blobName);
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

            await blobClient.UploadAsync(stream, overwrite: true);

            _logger.LogDebug("Successfully saved blob {BlobName} to container {ContainerName}", blobName, containerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving blob {BlobName} to container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    public async Task DeleteAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteAsync();
                _logger.LogDebug("Successfully deleted blob {BlobName} from container {ContainerName}", blobName, containerName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blob {BlobName} from container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            return await blobClient.ExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of blob {BlobName} in container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    public async IAsyncEnumerable<string> GetBlobNamesAsync(string containerName, string? prefix = null)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            yield return blobItem.Name;
        }
    }
}