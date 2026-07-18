namespace Sudoku.Domain.Events;

public record GameCreatedEvent(GameId GameId, ProfileId ProfileId, PlayerAlias DisplayName, GameDifficulty Difficulty, BoardSize Size) : DomainEvent;

public record GameStartedEvent(GameId GameId) : DomainEvent;

public record MoveMadeEvent(GameId GameId, int Row, int Column, int? Value, GameStatistics Statistics) : DomainEvent;

public record GamePausedEvent(GameId GameId) : DomainEvent;

public record GameResumedEvent(GameId GameId) : DomainEvent;

public record GameCompletedEvent(
    GameId GameId,
    ProfileId ProfileId,
    GameDifficulty Difficulty,
    GameStatistics Statistics,
    DateTime CompletedAt,
    BoardSize Size) : DomainEvent;

public record GameAbandonedEvent(GameId GameId) : DomainEvent;

public record MoveUndoneEvent(GameId GameId, int Row, int Column, int? Value) : DomainEvent;

public record GameResetEvent(GameId GameId) : DomainEvent;

public record PossibleValueAddedEvent(GameId GameId, int Row, int Column, int Value) : DomainEvent;

public record PossibleValueRemovedEvent(GameId GameId, int Row, int Column, int Value) : DomainEvent;

public record PossibleValuesClearedEvent(GameId GameId, int Row, int Column) : DomainEvent;

public record HintRevealedEvent(GameId GameId, int Row, int Column, int Value, GameStatistics Statistics) : DomainEvent;
