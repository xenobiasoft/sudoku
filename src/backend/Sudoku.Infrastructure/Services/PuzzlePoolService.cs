using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Services;

public class PuzzlePoolService(
    IPuzzleBlobStorage puzzleBlobStorage,
    ILogger<PuzzlePoolService> logger) : IPuzzlePoolService
{
    public async Task<int> GetAvailableCountAsync(GameDifficulty difficulty)
    {
        var puzzles = await puzzleBlobStorage.LoadAllAsync(difficulty.Name.ToLowerInvariant());
        var count = puzzles.Count();
        logger.LogInformation("Pool size for {Difficulty}: {Count}", difficulty.Name, count);
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
        var puzzles = (await puzzleBlobStorage.LoadAllAsync(difficulty.Name.ToLowerInvariant())).ToList();

        if (puzzles.Count == 0)
        {
            return null;
        }

        var puzzle = puzzles[Random.Shared.Next(puzzles.Count)];
        await puzzleBlobStorage.DeleteAsync(difficulty.Name.ToLowerInvariant(), puzzle.PuzzleId);

        return puzzle;
    }
}
