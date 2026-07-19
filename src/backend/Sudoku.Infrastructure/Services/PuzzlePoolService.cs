using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Services;

public class PuzzlePoolService(IPuzzleBlobStorage puzzleBlobStorage, ILogger<PuzzlePoolService> logger) : IPuzzlePoolService
{
    public async Task<int> GetAvailableCountAsync(BoardSize size, GameDifficulty difficulty)
    {
        var count = 0;
        var prefix = BuildPrefix(size, difficulty);
        await foreach (var _ in puzzleBlobStorage.GetPuzzleIdsAsync(prefix))
        {
            count++;
        }

        logger.LogDebug("Pool size for {Size} {Difficulty}: {Count}", size, difficulty.Name, count);
        return count;
    }

    public async Task SeedAsync(BoardSize size, GameDifficulty difficulty, int count)
    {
        for (var i = 0; i < count; i++)
        {
            await puzzleBlobStorage.CreateAsync(difficulty, size);
        }

        logger.LogInformation("Seeded {Count} puzzles for {Size} {Difficulty}", count, size, difficulty.Name);
    }

    public async Task<SudokuPuzzle?> DequeueAsync(BoardSize size, GameDifficulty difficulty)
    {
        var prefix = BuildPrefix(size, difficulty);
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

    private static string BuildPrefix(BoardSize size, GameDifficulty difficulty) =>
        $"{size.Size}x{size.Size}/{difficulty.Name.ToLowerInvariant()}";
}
