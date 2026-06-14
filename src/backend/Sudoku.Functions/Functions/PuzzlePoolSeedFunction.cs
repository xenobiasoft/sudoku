using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Sudoku.Functions.Services;

namespace Sudoku.Functions.Functions;

public class PuzzlePoolSeedFunction(IPuzzlePoolSeeder seeder, ILogger<PuzzlePoolSeedFunction> logger)
{
    [Function("PuzzlePoolSeedFunction")]
    public async Task Run([TimerTrigger("0 0 2 * * *")] TimerInfo myTimer)
    {
        logger.LogInformation("Puzzle pool seed function triggered at {Time}", DateTime.UtcNow);

        await seeder.SeedPoolAsync();
    }
}
