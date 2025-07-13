namespace Sudoku.Domain.Events;

public record GameCreatedEvent(GameId GameId, PlayerAlias PlayerAlias, GameDifficulty Difficulty) : DomainEvent;

public record GameStartedEvent(GameId GameId) : DomainEvent;

public record MoveMadeEvent(GameId GameId, int Row, int Column, int Value, GameStatistics Statistics) : DomainEvent;

public record GamePausedEvent(GameId GameId) : DomainEvent;

public record GameResumedEvent(GameId GameId) : DomainEvent;

public record GameCompletedEvent(GameId GameId, GameStatistics Statistics) : DomainEvent;

public record GameAbandonedEvent(GameId GameId) : DomainEvent;