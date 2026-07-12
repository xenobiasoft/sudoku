using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Services;

public class PuzzlePoolService(IPuzzleBlobStorage puzzleBlobStorage, ILogger<PuzzlePoolService> logger) : IPuzzlePoolService
{
    public async Task<int> GetAvailableCountAsync(GameDifficulty difficulty)
    {
        var count = 0;
        await foreach (var _ in puzzleBlobStorage.GetPuzzleIdsAsync(difficulty.Name.ToLowerInvariant()))
        {
            count++;
        }

        logger.LogDebug("Pool size for {Difficulty}: {Count}", difficulty.Name, count);
        return count;
    }

    public async Task SeedAsync(GameDifficulty difficulty, int count)
    {
        for (var i = 0; i < count; i++)
        {
            await puzzleBlobStorage.CreateAsync(difficulty);
        }

        logger.LogInformation("Seeded {Count} puzzles for {Difficulty}", count, difficulty.Name);
    }

    public async Task<SudokuPuzzle?> DequeueAsync(GameDifficulty difficulty)
    {
        var prefix = difficulty.Name.ToLowerInvariant();
        var puzzleIds = new List<string>();

        await foreach (var id in puzzleBlobStorage.GetPuzzleIdsAsync(prefix))
        {
            puzzleIds.Add(id);
        }

        if (puzzleIds.Count == 0)
        {
            return null;
        }

        // Optimistic dequeue: load the chosen blob then delete it as an exclusive claim.
        // Under concurrent requests, two callers may receive the same grid (per-spec acceptable
        // at 10-puzzle pool size); replenishment via Event Grid restores the count immediately.
        var puzzleId = puzzleIds[Random.Shared.Next(puzzleIds.Count)];
        var puzzle = await puzzleBlobStorage.LoadAsync(prefix, puzzleId);

        if (puzzle is null)
        {
            return null;
        }

        await puzzleBlobStorage.DeleteAsync(prefix, puzzleId);
        return puzzle;
    }
}
