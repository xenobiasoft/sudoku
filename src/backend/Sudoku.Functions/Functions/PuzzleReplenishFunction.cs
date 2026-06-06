using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Functions.Functions;

public class PuzzleReplenishFunction(IPuzzlePoolService puzzlePoolService, ILogger<PuzzleReplenishFunction> logger)
{
    [Function("PuzzleReplenishFunction")]
    public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent)
    {
        // Subject format: /blobServices/default/containers/sudoku-puzzles/blobs/{difficulty}/{puzzleId}.json
        var blobName = eventGridEvent.Subject.Split("/blobs/").LastOrDefault();
        var difficultyName = blobName?.Split('/').FirstOrDefault()?.ToLowerInvariant();

        if (string.IsNullOrEmpty(difficultyName))
        {
            logger.LogWarning("Could not parse difficulty from event subject: {Subject}", eventGridEvent.Subject);
            return;
        }

        GameDifficulty difficulty;
        try
        {
            difficulty = GameDifficulty.FromName(difficultyName);
        }
        catch (InvalidGameDifficultyException)
        {
            logger.LogWarning("Unknown difficulty '{DifficultyName}' in blob path, ignoring event", difficultyName);
            return;
        }

        logger.LogInformation("Replenishing 1 puzzle for {Difficulty}", difficulty.Name);
        await puzzlePoolService.SeedAsync(difficulty, 1);
    }
}
