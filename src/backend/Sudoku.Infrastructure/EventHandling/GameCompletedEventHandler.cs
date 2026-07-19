using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Models;
using Sudoku.Domain.Events;

namespace Sudoku.Infrastructure.EventHandling;

public class GameCompletedEventHandler(
    IGameCompletionRepository completionRepository,
    ILogger<GameCompletedEventHandler> logger) : IDomainEventHandler<GameCompletedEvent>
{
    public async Task HandleAsync(GameCompletedEvent domainEvent)
    {
        // GameStatistics is mutable and captured on the event by reference, so read it now.
        var completion = new GameCompletion(
            domainEvent.GameId.ToString(),
            domainEvent.ProfileId.ToString(),
            domainEvent.Difficulty.Name,
            domainEvent.Statistics.PlayDuration,
            domainEvent.CompletedAt,
            domainEvent.Size.Size);

        await completionRepository.UpsertAsync(completion);

        logger.LogInformation(
            "Recorded completion for game {GameId} (profile {ProfileId}, {Difficulty})",
            completion.GameId, completion.ProfileId, completion.Difficulty);
    }
}
