using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Functions.Services;

public class PuzzlePoolSeeder(IPuzzlePoolService puzzlePoolService, ILogger<PuzzlePoolSeeder> logger) : IPuzzlePoolSeeder
{
    public const int TargetPoolSize = 10;

    private static readonly GameDifficulty[] Difficulties =
    [
        GameDifficulty.Easy,
        GameDifficulty.Medium,
        GameDifficulty.Hard,
        GameDifficulty.Expert
    ];

    public async Task<int> SeedPoolAsync()
    {
        var totalSeeded = 0;

        foreach (var difficulty in Difficulties)
        {
            var currentCount = await puzzlePoolService.GetAvailableCountAsync(difficulty);
            var needed = TargetPoolSize - currentCount;

            if (needed > 0)
            {
                logger.LogInformation("Seeding {Count} puzzles for {Difficulty} (current: {Current})",
                    needed, difficulty.Name, currentCount);
                await puzzlePoolService.SeedAsync(difficulty, needed);
                totalSeeded += needed;
            }
            else
            {
                logger.LogInformation("Pool full for {Difficulty} ({Count}/{Target})",
                    difficulty.Name, currentCount, TargetPoolSize);
            }
        }

        return totalSeeded;
    }
}
