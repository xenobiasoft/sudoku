using XenobiaSoft.Sudoku.Services;

namespace XenobiaSoft.Sudoku.GameState;

public class AzureStorageGameStateManager(IStorageService storageService) : IGameStateManager, IDisposable
{
    private const string ContainerName = "sudoku-puzzles";
    private const int MaxUndoHistory = 50;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public GameStateMemoryType MemoryType => GameStateMemoryType.AzureBlobPersistence;

    public async Task DeleteAsync(string puzzleId)
    {
        await _semaphore.WaitAsync();

        try
        {
            await foreach (var blobItem in storageService.GetBlobNamesAsync(ContainerName, $"{puzzleId}/"))
            {
                await storageService.DeleteAsync(ContainerName, blobItem);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<GameStateMemory> LoadAsync(string puzzleId)
    {
        await _semaphore.WaitAsync();

        try
        {
            var latestBlobName = await GetLatestBlobNameAsync(puzzleId);

            if (latestBlobName == null)
            {
                return null;
            }

            return await storageService.LoadAsync<GameStateMemory>(ContainerName, latestBlobName);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveAsync(GameStateMemory gameState)
    {
        await _semaphore.WaitAsync();

        try
        {
            var nextBlobName = await GetNextBlobNameAsync(gameState.PuzzleId);

            await storageService.SaveAsync(ContainerName, nextBlobName, gameState);
            await TrimHistoryIfNeededAsync(gameState.PuzzleId);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<GameStateMemory> UndoAsync(string puzzleId)
    {
        await _semaphore.WaitAsync();

        try
        {
            var blobList = await GetSortedBlobNamesAsync(puzzleId);

            if (blobList.Count == 0)
            {
                return null;
            }

            var latestBlobName = blobList.Last();
            await storageService.DeleteAsync(ContainerName, latestBlobName);

            blobList.RemoveAt(blobList.Count - 1);

            if (blobList.Count == 0)
            {
                return null;
            }

            var previousBlobName = blobList.Last();

            return await storageService.LoadAsync<GameStateMemory>(ContainerName, previousBlobName);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<string?> GetLatestBlobNameAsync(string puzzleId)
    {
        var blobs = await GetSortedBlobNamesAsync(puzzleId);

        return blobs.LastOrDefault();
    }

    private async Task<string> GetNextBlobNameAsync(string puzzleId)
    {
        var blobs = await GetSortedBlobNamesAsync(puzzleId);

        var nextNumber = blobs.Count > 0
            ? int.Parse(Path.GetFileNameWithoutExtension(blobs.Last()) ?? "0") + 1
            : 1;

        return $"{puzzleId}/{nextNumber:D5}.json";
    }

    private async Task<List<string>> GetSortedBlobNamesAsync(string puzzleId)
    {
        var blobs = new List<string>();

        await foreach (var blobItem in storageService.GetBlobNamesAsync(ContainerName, blobPrefix: puzzleId))
        {
            blobs.Add(blobItem);
        }

        return blobs
            .OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x.Replace(puzzleId, string.Empty))))
            .ToList();
    }

    private async Task TrimHistoryIfNeededAsync(string puzzleId)
    {
        var blobs = await GetSortedBlobNamesAsync(puzzleId);
        if (blobs.Count > MaxUndoHistory)
        {
            var excess = blobs.Count - MaxUndoHistory;
            var oldest = blobs.Take(excess);

            foreach (var blobName in oldest)
            {
                await storageService.DeleteAsync(ContainerName, blobName);
            }
        }
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}