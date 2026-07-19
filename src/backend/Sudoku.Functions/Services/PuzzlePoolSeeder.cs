using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Functions.Services;

public class PuzzlePoolSeeder(IPuzzlePoolService puzzlePoolService, ILogger<PuzzlePoolSeeder> logger) : IPuzzlePoolSeeder
{
    public const int TargetPoolSize = 10;
    public const int TargetPoolSizeSixteen = 5;

    /// <summary>
    /// Headroom under the Azure Functions consumption-plan default timeout (5 minutes), so a
    /// single invocation always has time to persist/log before the host forcibly terminates it.
    /// </summary>
    public static readonly TimeSpan TimeBudget = TimeSpan.FromMinutes(4);

    // Fast/cheap combinations first so a single invocation reliably tops up the 9x9 pools even
    // if it runs out of budget partway through the slower 16x16 Hard/Expert generation.
    private static readonly (BoardSize Size, GameDifficulty Difficulty, int Target)[] Combinations =
    [
        (BoardSize.Nine, GameDifficulty.Easy, TargetPoolSize),
        (BoardSize.Nine, GameDifficulty.Medium, TargetPoolSize),
        (BoardSize.Nine, GameDifficulty.Hard, TargetPoolSize),
        (BoardSize.Nine, GameDifficulty.Expert, TargetPoolSize),
        (BoardSize.Sixteen, GameDifficulty.Easy, TargetPoolSizeSixteen),
        (BoardSize.Sixteen, GameDifficulty.Medium, TargetPoolSizeSixteen),
        (BoardSize.Sixteen, GameDifficulty.Hard, TargetPoolSizeSixteen),
        (BoardSize.Sixteen, GameDifficulty.Expert, TargetPoolSizeSixteen)
    ];

    public async Task<int> SeedPoolAsync()
    {
        var totalSeeded = 0;
        var stopwatch = Stopwatch.StartNew();

        foreach (var (size, difficulty, target) in Combinations)
        {
            var currentCount = await puzzlePoolService.GetAvailableCountAsync(size, difficulty);

            while (currentCount < target)
            {
                if (stopwatch.Elapsed >= TimeBudget)
                {
                    logger.LogInformation(
                        "Time budget exhausted after seeding {Total} puzzles; stopping for this invocation",
                        totalSeeded);
                    return totalSeeded;
                }

                logger.LogInformation("Seeding 1 puzzle for {Size} {Difficulty} (current: {Current}/{Target})",
                    size, difficulty.Name, currentCount, target);

                await puzzlePoolService.SeedAsync(size, difficulty, 1);

                totalSeeded++;
                currentCount++;
            }

            logger.LogInformation("Pool full for {Size} {Difficulty} ({Count}/{Target})",
                size, difficulty.Name, currentCount, target);
        }

        return totalSeeded;
    }
}
