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
        // Subject format: /blobServices/default/containers/sudoku-puzzles/blobs/{size}x{size}/{difficulty}/{puzzleId}.json
        var blobName = eventGridEvent.Subject.Split("/blobs/").LastOrDefault();
        var segments = blobName?.Split('/');
        var sizeSegment = segments?.Length > 0 ? segments[0] : null;
        var difficultyName = segments?.Length > 1 ? segments[1].ToLowerInvariant() : null;

        if (string.IsNullOrEmpty(sizeSegment))
        {
            logger.LogWarning("Could not parse board size from event subject: {Subject}", eventGridEvent.Subject);
            return;
        }

        if (string.IsNullOrEmpty(difficultyName))
        {
            logger.LogWarning("Could not parse difficulty from event subject: {Subject}", eventGridEvent.Subject);
            return;
        }

        BoardSize size;
        try
        {
            size = ParseSize(sizeSegment);
        }
        catch (InvalidBoardSizeException)
        {
            logger.LogWarning("Unknown board size '{SizeSegment}' in blob path, ignoring event", sizeSegment);
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

        logger.LogInformation("Replenishing 1 puzzle for {Size} {Difficulty}", size, difficulty.Name);
        await puzzlePoolService.SeedAsync(size, difficulty, 1);
    }

    private static BoardSize ParseSize(string sizeSegment)
    {
        var parts = sizeSegment.Split('x', 'X');

        if (parts.Length != 2
            || !int.TryParse(parts[0], out var width)
            || !int.TryParse(parts[1], out var height)
            || width != height)
        {
            throw new InvalidBoardSizeException($"Invalid board size segment: {sizeSegment}");
        }

        return BoardSize.FromValue(width);
    }
}
