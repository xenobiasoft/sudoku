using XenobiaSoft.Sudoku.Services;

namespace XenobiaSoft.Sudoku.GameState;

public class AzureStorageGameStateMemory(IStorageService storageService) : IGameStateMemoryPersistence
{
    private const string ContainerName = "sudoku-puzzles";
    private const string BlobName = "game-state.json";

    private readonly TimeSpan _saveDebounceDelay = TimeSpan.FromSeconds(1);
    private readonly SemaphoreSlim _debounceSemaphore = new(1, 1);
    private CancellationTokenSource? _debounceCts;

    public Task ClearAsync(string puzzleId)
    {
        var blobName = GetBlobName(puzzleId);
        return storageService.DeleteAsync(ContainerName, blobName);
    }

    public Task<GameStateMemento> LoadAsync(string puzzleId)
    {
        var blobName = GetBlobName(puzzleId);

        return storageService.LoadAsync<GameStateMemento>(ContainerName, blobName);
    }

    public async Task SaveAsync(GameStateMemento gameState)
    {
        await DebounceSaveAsync(gameState);
    }

    private async Task DebounceSaveAsync(GameStateMemento gameState)
    {
        await _debounceSemaphore.WaitAsync();

        try
        {
            _debounceCts?.Cancel();
            _debounceCts?.Dispose();
            _debounceCts = new CancellationTokenSource();
            try
            {
                await Task.Delay(_saveDebounceDelay, _debounceCts.Token);
                await SaveNowAsync(gameState, _debounceCts.Token);
            }
            catch (TaskCanceledException)
            { }
        }
        finally
        {
            _debounceSemaphore.Release();
        }
    }

    private async Task SaveNowAsync(GameStateMemento gameState, CancellationToken token)
    {
        var blobName = GetBlobName(gameState.PuzzleId);

        await storageService.SaveAsync(ContainerName, blobName, gameState, token);
    }

    private string GetBlobName(string puzzleId)
    {
        return $"{puzzleId}/{BlobName}";
    }
}