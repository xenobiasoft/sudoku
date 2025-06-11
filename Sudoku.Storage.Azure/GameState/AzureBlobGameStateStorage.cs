using XenobiaSoft.Sudoku.Exceptions;
using XenobiaSoft.Sudoku.Extensions;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace Sudoku.Storage.Azure.GameState;

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