using Microsoft.Extensions.Logging;
using Sudoku.Domain.Events;

namespace Sudoku.Infrastructure.EventHandling;

public class GameCreatedEventHandler(ILogger<GameCreatedEventHandler> logger) : IDomainEventHandler<GameCreatedEvent>
{
    public async Task HandleAsync(GameCreatedEvent domainEvent)
    {
        logger.LogInformation("Game created: {GameId} for player {PlayerAlias} with difficulty {Difficulty}",
            domainEvent.GameId.Value, domainEvent.PlayerAlias.Value, domainEvent.Difficulty);

        // Here you could:
        // - Send notifications
        // - Update statistics
        // - Trigger analytics
        // - Send to external systems

        await Task.CompletedTask;
    }
}