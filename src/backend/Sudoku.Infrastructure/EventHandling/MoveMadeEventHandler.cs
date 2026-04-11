using Microsoft.Extensions.Logging;
using Sudoku.Domain.Events;

namespace Sudoku.Infrastructure.EventHandling;

public class MoveMadeEventHandler(ILogger<MoveMadeEventHandler> logger) : IDomainEventHandler<MoveMadeEvent>
{
    public async Task HandleAsync(MoveMadeEvent domainEvent)
    {
        logger.LogDebug("Move made in game {GameId}: Row={Row}, Column={Column}, Value={Value}",
            domainEvent.GameId.Value, domainEvent.Row, domainEvent.Column, domainEvent.Value);

        // Here you could:
        // - Update game statistics
        // - Check for achievements
        // - Send real-time updates
        // - Log for analytics

        await Task.CompletedTask;
    }
}