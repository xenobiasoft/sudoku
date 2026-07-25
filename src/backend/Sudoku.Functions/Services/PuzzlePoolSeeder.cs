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

    // Driven off BoardSize.SupportedDifficulties so the pools stay in step with what players can
    // actually start. Fast/cheap 9x9 combinations run first so a single invocation reliably tops
    // those up even if it runs out of budget partway through the slower 16x16 generation.
    private static readonly (BoardSize Size, GameDifficulty Difficulty, int Target)[] Combinations =
    [
        .. BoardSize.Nine.SupportedDifficulties.Select(d => (BoardSize.Nine, d, TargetPoolSize)),
        .. BoardSize.Sixteen.SupportedDifficulties.Select(d => (BoardSize.Sixteen, d, TargetPoolSizeSixteen))
    ];

    public async Task<int> SeedPoolAsync(PuzzlePoolSeedFilter? filter = null)
    {
        var totalSeeded = 0;
        var stopwatch = Stopwatch.StartNew();

        foreach (var (size, difficulty, target) in Filter(filter))
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

    // Filtering the supported matrix rather than building combinations from the filter keeps the
    // BoardSize.SupportedDifficulties guarantee intact: asking for an unsupported pairing such as
    // 16x16 Hard simply matches nothing instead of generating a puzzle players can never start.
    private static IEnumerable<(BoardSize Size, GameDifficulty Difficulty, int Target)> Filter(PuzzlePoolSeedFilter? filter) =>
        Combinations
            .Where(c => filter?.Size is null || c.Size == filter.Size)
            .Where(c => filter?.Difficulty is null || c.Difficulty == filter.Difficulty)
            .Select(c => (c.Size, c.Difficulty, filter?.Target ?? c.Target));
}
