---
paths:
  - "src/backend/Sudoku.Domain/**/*.cs"
---

# Domain Layer Guidelines

## Entity Structure
```csharp
public class SudokuGame : AggregateRoot
{
    private readonly List<Cell> _cells;
    private readonly List<DomainEvent> _domainEvents;

    public GameId Id { get; private set; }
    public PlayerAlias PlayerAlias { get; private set; }
    public GameDifficulty Difficulty { get; private set; }
    public GameStatus Status { get; private set; }

    private SudokuGame() { } // Private constructor for EF Core

    public static SudokuGame Create(PlayerAlias playerAlias, GameDifficulty difficulty)
    {
        // Validation and creation logic
    }

    public void MakeMove(int row, int column, int value)
    {
        // Business validation → state changes → raise domain event
    }
}
```

## Value Objects
```csharp
public record GameId(Guid Value)
{
    public static GameId New() => new(Guid.NewGuid());
    public static GameId FromString(string value) => new(Guid.Parse(value));
}
```

## Domain Events
```csharp
public record GameCreatedEvent(GameId GameId, PlayerAlias PlayerAlias, GameDifficulty Difficulty) : DomainEvent;
public record MoveMadeEvent(GameId GameId, int Row, int Column, int Value) : DomainEvent;
```

## Specifications
```csharp
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}

public class GameByPlayerSpecification : ISpecification<Game>
{
    private readonly PlayerAlias _playerAlias;

    public Expression<Func<Game, bool>> Criteria =>
        game => game.PlayerAlias == _playerAlias;
}
```

## Rules
- No business logic in controllers or handlers — all invariants enforced inside aggregates
- Raise domain events for every significant state change
- Use private setters; expose state only through domain methods
- Use factory methods (`Create()`) instead of public constructors
