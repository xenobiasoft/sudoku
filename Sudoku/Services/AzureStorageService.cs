using Azure.Storage.Blobs;
using System.Text;
using System.Text.Json;

namespace XenobiaSoft.Sudoku.Services;

public class AzureStorageService(string connectionString) : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient = new(connectionString);

    public Task DeleteAsync(string containerName, string blobName)
    {
        var blobClient = GetBlobClient(containerName, blobName);
        
        return blobClient.DeleteIfExistsAsync();
    }

    public async Task<TBlobType> LoadAsync<TBlobType>(string containerName, string blobName) where TBlobType : class
    {
        var blobClient = GetBlobClient(containerName, blobName);

        if (!await blobClient.ExistsAsync()) return null;

        var download = await blobClient.DownloadContentAsync();
        var json = download.Value.Content.ToString();

        return JsonSerializer.Deserialize<TBlobType>(json);

    }

    public Task SaveAsync(string containerName, string blobName, object blob, CancellationToken token)
    {
        var blobClient = GetBlobClient(containerName, blobName);
        var json = JsonSerializer.Serialize(blob);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return blobClient.UploadAsync(stream, true, token);
    }

    private BlobClient GetBlobClient(string containerName, string blobName)
    {
        var containerClient = GetContainerClient(containerName);
        return containerClient.GetBlobClient(blobName);
    }

    private BlobContainerClient GetContainerClient(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        containerClient.CreateIfNotExists();

        return containerClient;
    }
}