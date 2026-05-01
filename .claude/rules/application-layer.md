---
paths:
  - "src/backend/Sudoku.Application/**/*.cs"
---

# Application Layer Guidelines

## CQRS Pattern
```csharp
public record CreateGameCommand(string PlayerAlias, GameDifficulty Difficulty);
public record GetGameQuery(GameId GameId);
```

## Command/Query Handlers
```csharp
public class CreateGameCommandHandler : ICommandHandler<CreateGameCommand>
{
    private readonly IGameRepository _gameRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public async Task<Result> HandleAsync(CreateGameCommand command)
    {
        // 1. Application logic / validation
        // 2. Domain interaction
        // 3. Persistence
        // 4. Dispatch domain events
    }
}
```

## Rules
- All operations flow through `IMediator`
- Handlers always return `Result<T>` — never throw exceptions for expected failures
- Repository interfaces are defined here; implementations live in Infrastructure
- No business logic in handlers — delegate to domain aggregates
