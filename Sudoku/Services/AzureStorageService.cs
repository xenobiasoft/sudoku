using System.Text;
using Azure.Storage.Blobs;
using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Services;

public class AzureStorageService : IStorageService
{
    private const string ContainerName = "sudoku-puzzles";
    private readonly BlobContainerClient _blobContainerClient;


    public AzureStorageService(string connectionString)
    {
        _blobContainerClient = new BlobContainerClient(connectionString, ContainerName);
        _blobContainerClient.CreateIfNotExists();
    }

    public Task DeleteAsync(string puzzleId)
    {
        var blobClient = _blobContainerClient.GetBlobClient(GetBlobName(puzzleId));
        
        return blobClient.DeleteIfExistsAsync();
    }

    public async Task<GameStateMemento> LoadAsync(string puzzleId)
    {
        var blobClient = _blobContainerClient.GetBlobClient(GetBlobName(puzzleId));

        if (!await blobClient.ExistsAsync()) return null;

        var download = await blobClient.DownloadContentAsync();
        var json = download.Value.Content.ToString();

        return json.FromJson();

    }

    public Task SaveAsync(string puzzleId, GameStateMemento gameState, CancellationToken token)
    {
        var blobClient = _blobContainerClient.GetBlobClient(GetBlobName(puzzleId));
        var json = gameState.ToJson();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return blobClient.UploadAsync(stream, true, token);
    }

    private string GetBlobName(string puzzleId)
    {
        return $"{puzzleId}/game-state.json";
    }
}