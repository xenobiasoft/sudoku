namespace Sudoku.Domain.Events;

public record ProfileCreatedEvent(ProfileId ProfileId, PlayerAlias Alias) : DomainEvent;
public record ProfileAliasUpdatedEvent(ProfileId ProfileId, PlayerAlias OldAlias, PlayerAlias NewAlias) : DomainEvent;
