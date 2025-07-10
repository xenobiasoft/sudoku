using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using XenobiaSoft.Sudoku.Abstractions;

namespace XenobiaSoft.Sudoku.Storage.Azure.Services;

public class AzureStorageService(BlobServiceClient blobServiceClient) : IStorageService
{
    public Task DeleteAsync(string containerName, string blobName)
    {
        var blobClient = GetBlobClient(containerName, blobName);

        return blobClient.DeleteIfExistsAsync();
    }

    public async IAsyncEnumerable<string> GetBlobNamesAsync(string containerName, string blobPrefix)
    {
        var containerClient = GetContainerClient(containerName);
        var blobs = containerClient
            .GetBlobsAsync(prefix: $"{blobPrefix}/")
            .AsPages(null, 50);

        await foreach (var page in blobs)
        {
            foreach (var blobItem in page.Values)
            {
                yield return blobItem.Name;
            }
        }
    }

    public async Task<TBlobType> LoadAsync<TBlobType>(string containerName, string blobName) where TBlobType : class
    {
        var blobClient = GetBlobClient(containerName, blobName);

        if (!await blobClient.ExistsAsync()) return null;

        var download = await blobClient.DownloadContentAsync();
        var json = download.Value.Content.ToString();

        return JsonSerializer.Deserialize<TBlobType>(json);
    }

    public Task SaveAsync(string containerName, string blobName, object blob, bool? overwrite = false)
    {
        var blobClient = GetBlobClient(containerName, blobName);
        var json = JsonSerializer.Serialize(blob);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return blobClient.UploadAsync(stream, overwrite.GetValueOrDefault());
    }

    private BlobClient GetBlobClient(string containerName, string blobName)
    {
        var containerClient = GetContainerClient(containerName);
        return containerClient.GetBlobClient(blobName);
    }

    private BlobContainerClient GetContainerClient(string containerName)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        containerClient.CreateIfNotExists();

        return containerClient;
    }
}