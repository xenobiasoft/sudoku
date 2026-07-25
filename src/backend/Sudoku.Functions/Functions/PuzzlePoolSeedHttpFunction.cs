using System.Net;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Functions.Services;

namespace Sudoku.Functions.Functions;

/// <summary>
/// HTTP-triggered counterpart of <see cref="PuzzlePoolSeedFunction"/>. Performs the
/// same pool top-up as the nightly timer, but can be invoked on demand for testing.
/// Secured at <see cref="AuthorizationLevel.Function"/> — callers must supply the function key.
/// </summary>
/// <remarks>
/// Accepts optional <c>size</c>, <c>difficulty</c> and <c>count</c> query parameters to narrow the
/// run, e.g. <c>?size=16&amp;difficulty=Easy</c>. This is the fastest way to unblock local
/// development: the emulator starts with an empty pool and 16x16 has no on-demand generation
/// fallback, so a 16x16 game cannot be created until its pool is seeded.
/// </remarks>
public class PuzzlePoolSeedHttpFunction(IPuzzlePoolSeeder seeder, ILogger<PuzzlePoolSeedHttpFunction> logger)
{
    [Function("PuzzlePoolSeedHttpFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "seed-puzzle-pool")] HttpRequestData request)
    {
        logger.LogInformation("Puzzle pool seed (HTTP) triggered at {Time}", DateTime.UtcNow);

        PuzzlePoolSeedFilter? filter;
        try
        {
            filter = ParseFilter(request);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning("Rejected puzzle pool seed request: {Error}", ex.Message);
            return await WriteJsonAsync(request, HttpStatusCode.BadRequest, $"{{\"error\":\"{ex.Message}\"}}");
        }

        var seeded = await seeder.SeedPoolAsync(filter);

        logger.LogInformation("Puzzle pool seed (HTTP) completed, seeded {Count} puzzles", seeded);

        return await WriteJsonAsync(request, HttpStatusCode.OK, $"{{\"seeded\":{seeded}}}");
    }

    private static PuzzlePoolSeedFilter? ParseFilter(HttpRequestData request)
    {
        var query = HttpUtility.ParseQueryString(request.Url?.Query ?? string.Empty);
        var size = ParseSize(query["size"]);
        var difficulty = ParseDifficulty(query["difficulty"]);
        var target = ParseTarget(query["count"]);

        return size is null && difficulty is null && target is null
            ? null
            : new PuzzlePoolSeedFilter(size, difficulty, target);
    }

    private static BoardSize? ParseSize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            return BoardSize.FromValue(int.Parse(value));
        }
        catch (Exception ex) when (ex is FormatException or OverflowException or InvalidBoardSizeException)
        {
            throw new ArgumentException($"Invalid size '{value}'; expected 9 or 16.");
        }
    }

    private static GameDifficulty? ParseDifficulty(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            return GameDifficulty.FromName(value);
        }
        catch (InvalidGameDifficultyException)
        {
            throw new ArgumentException($"Invalid difficulty '{value}'; expected Easy, Medium, Hard or Expert.");
        }
    }

    private static int? ParseTarget(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!int.TryParse(value, out var target) || target < 0)
        {
            throw new ArgumentException($"Invalid count '{value}'; expected a non-negative integer.");
        }

        return target;
    }

    private static async Task<HttpResponseData> WriteJsonAsync(HttpRequestData request, HttpStatusCode status, string body)
    {
        // CreateResponse() rather than the CreateResponse(HttpStatusCode) overload: the latter is
        // virtual and returns null against a loose mock, which would break the function tests.
        var response = request.CreateResponse();
        response.StatusCode = status;
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(body);

        return response;
    }
}
