using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Functions.Functions;

public class PuzzlePoolSeedFunction(IPuzzlePoolService puzzlePoolService, ILogger<PuzzlePoolSeedFunction> logger)
{
    private const int TargetPoolSize = 10;

    private static readonly GameDifficulty[] Difficulties =
    [
        GameDifficulty.Easy,
        GameDifficulty.Medium,
        GameDifficulty.Hard,
        GameDifficulty.Expert
    ];

    [Function("PuzzlePoolSeedFunction")]
    public async Task Run([TimerTrigger("0 0 2 * * *")] TimerInfo myTimer)
    {
        logger.LogInformation("Puzzle pool seed function triggered at {Time}", DateTime.UtcNow);

        foreach (var difficulty in Difficulties)
        {
            var currentCount = await puzzlePoolService.GetAvailableCountAsync(difficulty);
            var needed = TargetPoolSize - currentCount;

            if (needed > 0)
            {
                logger.LogInformation("Seeding {Count} puzzles for {Difficulty} (current: {Current})",
                    needed, difficulty.Name, currentCount);
                await puzzlePoolService.SeedAsync(difficulty, needed);
            }
            else
            {
                logger.LogInformation("Pool full for {Difficulty} ({Count}/{Target})",
                    difficulty.Name, currentCount, TargetPoolSize);
            }
        }
    }
}
