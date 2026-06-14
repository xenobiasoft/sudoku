using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Sudoku.Functions.Services;

namespace Sudoku.Functions.Functions;

/// <summary>
/// HTTP-triggered counterpart of <see cref="PuzzlePoolSeedFunction"/>. Performs the
/// same pool top-up as the nightly timer, but can be invoked on demand for testing.
/// Secured at <see cref="AuthorizationLevel.Function"/> — callers must supply the function key.
/// </summary>
public class PuzzlePoolSeedHttpFunction(IPuzzlePoolSeeder seeder, ILogger<PuzzlePoolSeedHttpFunction> logger)
{
    [Function("PuzzlePoolSeedHttpFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "seed-puzzle-pool")] HttpRequestData request)
    {
        logger.LogInformation("Puzzle pool seed (HTTP) triggered at {Time}", DateTime.UtcNow);

        var seeded = await seeder.SeedPoolAsync();

        logger.LogInformation("Puzzle pool seed (HTTP) completed, seeded {Count} puzzles", seeded);

        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync($"{{\"seeded\":{seeded}}}");

        return response;
    }
}
