using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;

namespace Sudoku.Infrastructure.Services;

public class AzureStorageService(BlobServiceClient blobServiceClient, ILogger<AzureStorageService> logger) : IAzureStorageService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task<T?> LoadAsync<T>(string containerName, string blobName)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                logger.LogDebug("Blob {BlobName} not found in container {ContainerName}", blobName, containerName);
                return default;
            }

            var response = await blobClient.DownloadAsync();
            using var streamReader = new StreamReader(response.Value.Content);
            var json = await streamReader.ReadToEndAsync();
            var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);

            logger.LogDebug("Successfully loaded blob {BlobName} from container {ContainerName}", blobName, containerName);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading blob {BlobName} from container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    public async Task SaveAsync<T>(string containerName, string blobName, T data)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

            await blobClient.UploadAsync(stream, overwrite: true);

            logger.LogDebug("Successfully saved blob {BlobName} to container {ContainerName}", blobName, containerName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving blob {BlobName} to container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    public async Task DeleteAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteAsync();
                logger.LogDebug("Successfully deleted blob {BlobName} from container {ContainerName}", blobName, containerName);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting blob {BlobName} from container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            return await blobClient.ExistsAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking existence of blob {BlobName} in container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    public async IAsyncEnumerable<string> GetBlobNamesAsync(string containerName, string? prefix = null)
    {
        var containerClient = await GetContainerClientAsync(containerName);

        await foreach (var blobItem in containerClient.GetBlobsAsync(new GetBlobsOptions { Prefix = prefix }))
        {
            yield return blobItem.Name;
        }
    }

    private async Task<BlobContainerClient> GetContainerClientAsync(string containerName)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        return containerClient;
    }
}