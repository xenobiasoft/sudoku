using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.Exceptions;
using XenobiaSoft.Sudoku.Extensions;
using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Storage.Azure.GameState;

public class AzureBlobGameStateStorage(IStorageService storageService) : IPersistentGameStateStorage, IDisposable
{
    private const string ContainerName = "sudoku-puzzles";
    private const int MaxUndoHistory = 50;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public GameStateMemoryType MemoryType => GameStateMemoryType.AzureBlobPersistence;

    public async Task DeleteAsync(string alias, string puzzleId)
    {
        ValidateGameStateArgs(alias, puzzleId);

        await _semaphore.WaitAsync();

        try
        {
            await foreach (var blobItem in storageService.GetBlobNamesAsync(ContainerName, $"{alias}/{puzzleId}"))
            {
                await storageService.DeleteAsync(ContainerName, blobItem);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
    }

    public async Task<GameStateMemory> LoadAsync(string alias, string puzzleId)
    {
        ValidateGameStateArgs(alias, puzzleId);

        await _semaphore.WaitAsync();

        try
        {
            var latestBlobName = await GetLatestBlobNameAsync(alias, puzzleId);

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

    public async Task<IEnumerable<GameStateMemory>> LoadAllAsync(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            throw new ArgumentException("Alias cannot be empty.", nameof(alias));
        }

        await _semaphore.WaitAsync();

        try
        {
            var puzzleIdToLatestBlobMap = new Dictionary<string, string>();

            await foreach (var blobName in storageService.GetBlobNamesAsync(ContainerName, alias))
            {
                var parts = blobName.Split('/');
                if (parts.Length < 3) continue;

                var puzzleId = parts[1];
                var filename = parts[2];

                // Track the full blob path for each puzzle
                var blobPath = $"{alias}/{puzzleId}/{filename}";

                // If we already have a blob for this puzzle ID, check if this one is newer
                if (puzzleIdToLatestBlobMap.TryGetValue(puzzleId, out var existingBlobPath))
                {
                    var existingFilename = existingBlobPath.Split('/')[2];
                    var existingNumber = int.Parse(Path.GetFileNameWithoutExtension(existingFilename));
                    var currentNumber = int.Parse(Path.GetFileNameWithoutExtension(filename));

                    if (currentNumber > existingNumber)
                    {
                        puzzleIdToLatestBlobMap[puzzleId] = blobPath;
                    }
                }
                else
                {
                    puzzleIdToLatestBlobMap[puzzleId] = blobPath;
                }
            }

            // Load all the latest game states in parallel
            var tasks = puzzleIdToLatestBlobMap.Values
                .Select(blobPath => storageService.LoadAsync<GameStateMemory>(ContainerName, blobPath));

            return await Task.WhenAll(tasks);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<GameStateMemory> ResetAsync(string alias, string puzzleId)
    {
        ValidateGameStateArgs(alias, puzzleId);

        await _semaphore.WaitAsync();

        try
        {
            var blobList = await GetSortedBlobNamesAsync(alias, puzzleId);

            if (blobList.Count <= 1)
            {
                throw new CannotResetInitialStateException();
            }

            var initialBlobName = blobList.First();
            var blobsToDelete = blobList.Where(x => x != initialBlobName);

            foreach (var blobName in blobsToDelete)
            {
                await storageService.DeleteAsync(ContainerName, blobName);
            }

            return await storageService.LoadAsync<GameStateMemory>(ContainerName, initialBlobName);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveAsync(GameStateMemory gameState)
    {
        ValidateGameState(gameState);

        var currentGameState = await LoadAsync(gameState.Alias, gameState.PuzzleId);

        if (currentGameState != null && currentGameState.IsSameGameStateAs(gameState))
        {
            return;
        }

        await _semaphore.WaitAsync();

        try
        {
            var nextBlobName = await GetNextBlobNameAsync(gameState.Alias, gameState.PuzzleId);

            await storageService.SaveAsync(ContainerName, nextBlobName, gameState);
            await TrimHistoryIfNeededAsync(gameState.Alias, gameState.PuzzleId);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<GameStateMemory> UndoAsync(string alias, string puzzleId)
    {
        ValidateGameStateArgs(alias, puzzleId);

        await _semaphore.WaitAsync();

        try
        {
            var blobList = await GetSortedBlobNamesAsync(alias, puzzleId);

            if (blobList.Count <= 1)
            {
                throw new CannotUndoInitialStateException();
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

    private async Task<string?> GetLatestBlobNameAsync(string alias, string puzzleId)
    {
        var blobs = await GetSortedBlobNamesAsync(alias, puzzleId);

        return blobs.LastOrDefault();
    }

    private async Task<string> GetNextBlobNameAsync(string alias, string puzzleId)
    {
        var blobs = await GetSortedBlobNamesAsync(alias, puzzleId);

        var nextNumber = blobs.Count > 0
            ? int.Parse(Path.GetFileNameWithoutExtension(blobs.Last()) ?? "0") + 1
            : 1;

        return $"{alias}/{puzzleId}/{nextNumber:D5}.json";
    }

    private async Task<List<string>> GetSortedBlobNamesAsync(string alias, string puzzleId)
    {
        var blobs = new List<string>();

        await foreach (var blobItem in storageService.GetBlobNamesAsync(ContainerName, blobPrefix: $"{alias}/{puzzleId}"))
        {
            blobs.Add(blobItem);
        }

        return blobs
            .OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x)))
            .ToList();
    }

    private async Task TrimHistoryIfNeededAsync(string alias, string puzzleId)
    {
        var blobs = await GetSortedBlobNamesAsync(alias, puzzleId);
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

    private void ValidateGameState(GameStateMemory gameState)
    {
        if (gameState == null)
        {
            throw new ArgumentNullException(nameof(gameState), "Game state cannot be null.");
        }
        ValidateGameStateArgs(gameState.Alias, gameState.PuzzleId);
    }

    private void ValidateGameStateArgs(string alias, string puzzleId)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            throw new ArgumentException("Alias cannot be empty.", nameof(alias));
        }

        if (string.IsNullOrWhiteSpace(puzzleId))
        {
            throw new ArgumentException("PuzzleId cannot be empty.", nameof(puzzleId));
        }
    }
}