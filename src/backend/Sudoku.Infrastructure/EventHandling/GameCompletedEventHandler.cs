using Microsoft.Extensions.Logging;
using Sudoku.Domain.Events;

namespace Sudoku.Infrastructure.EventHandling;

public class GameCompletedEventHandler(ILogger<GameCompletedEventHandler> logger) : IDomainEventHandler<GameCompletedEvent>
{
    public async Task HandleAsync(GameCompletedEvent domainEvent)
    {
        logger.LogInformation("Game completed: {GameId}", domainEvent.GameId.Value);

        // Here you could:
        // - Send completion notifications
        // - Update player statistics
        // - Award achievements
        // - Send to leaderboards
        // - Trigger analytics

        await Task.CompletedTask;
    }
}