using Microsoft.Extensions.Logging;
using Sudoku.Domain.Events;

namespace Sudoku.Infrastructure.EventHandling;

public class GameCreatedEventHandler : IDomainEventHandler<GameCreatedEvent>
{
    private readonly ILogger<GameCreatedEventHandler> _logger;

    public GameCreatedEventHandler(ILogger<GameCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(GameCreatedEvent domainEvent)
    {
        _logger.LogInformation("Game created: {GameId} for player {PlayerAlias} with difficulty {Difficulty}",
            domainEvent.GameId.Value, domainEvent.PlayerAlias.Value, domainEvent.Difficulty);

        // Here you could:
        // - Send notifications
        // - Update statistics
        // - Trigger analytics
        // - Send to external systems

        await Task.CompletedTask;
    }
}

public class MoveMadeEventHandler : IDomainEventHandler<MoveMadeEvent>
{
    private readonly ILogger<MoveMadeEventHandler> _logger;

    public MoveMadeEventHandler(ILogger<MoveMadeEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(MoveMadeEvent domainEvent)
    {
        _logger.LogDebug("Move made in game {GameId}: Row={Row}, Column={Column}, Value={Value}",
            domainEvent.GameId.Value, domainEvent.Row, domainEvent.Column, domainEvent.Value);

        // Here you could:
        // - Update game statistics
        // - Check for achievements
        // - Send real-time updates
        // - Log for analytics

        await Task.CompletedTask;
    }
}

public class GameCompletedEventHandler : IDomainEventHandler<GameCompletedEvent>
{
    private readonly ILogger<GameCompletedEventHandler> _logger;

    public GameCompletedEventHandler(ILogger<GameCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(GameCompletedEvent domainEvent)
    {
        _logger.LogInformation("Game completed: {GameId}", domainEvent.GameId.Value);

        // Here you could:
        // - Send completion notifications
        // - Update player statistics
        // - Award achievements
        // - Send to leaderboards
        // - Trigger analytics

        await Task.CompletedTask;
    }
}