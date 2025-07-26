# Proposed Clean Architecture for Sudoku Application

## Current Issues Identified

1. **Mixed Responsibilities**: The core domain library contains both business logic and infrastructure concerns
2. **Tight Coupling**: Direct dependencies between layers
3. **Inconsistent Service Patterns**: Multiple service interfaces with overlapping responsibilities
4. **Missing Application Layer**: Business use cases are scattered across different services
5. **No Clear Domain Events**: State changes are handled through direct method calls

## Proposed Clean Architecture Structure

```
Sudoku.sln
├── Sudoku.Domain/                    # Core domain entities and business rules
├── Sudoku.Application/               # Application use cases and orchestration
├── Sudoku.Infrastructure/            # External concerns (storage, external APIs)
├── Sudoku.Web.Server/                # Blazor Server presentation layer
├── Sudoku.Api/                       # REST API presentation layer
├── Sudoku.AppHost/                   # Application orchestration
└── Tests/                           # Unit and integration tests
```

## Detailed Layer Responsibilities

### 1. **Domain Layer** (`Sudoku.Domain`)

- **Entities**: `SudokuGame`, `SudokuPuzzle`, `Cell`, `Player`
- **Value Objects**: `GameId`, `PuzzleId`, `PlayerAlias`, `GameDifficulty`
- **Domain Events**: `GameCreated`, `MoveMade`, `GameCompleted`
- **Domain Services**: `PuzzleValidator`, `GameRulesEngine`
- **Exceptions**: Domain-specific exceptions

### 2. **Application Layer** (`Sudoku.Application`)

- **Use Cases**: `CreateGameUseCase`, `MakeMoveUseCase`, `SaveGameUseCase`
- **Commands/Queries**: CQRS pattern implementation
- **Application Services**: `GameApplicationService`
- **DTOs**: Request/Response models
- **Interfaces**: Repository and service contracts

### 3. **Infrastructure Layer** (`Sudoku.Infrastructure`)

- **Repositories**: `AzureBlobGameRepository`, `InMemoryGameRepository`
- **External Services**: `AzureStorageService`
- **Configuration**: Service registration and configuration
- **Logging**: Infrastructure logging concerns

### 4. **Presentation Layers**

- **Web.Server**: Blazor Server components and pages
- **API**: REST API controllers and middleware

## Key Architectural Improvements

### 1. **Implement CQRS Pattern**

```csharp
// Commands
public record CreateGameCommand(string PlayerAlias, GameDifficulty Difficulty);
public record MakeMoveCommand(GameId GameId, int Row, int Column, int Value);
public record SaveGameCommand(GameId GameId);

// Queries
public record GetGameQuery(GameId GameId);
public record GetPlayerGamesQuery(string PlayerAlias);

// Handlers
public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command);
}

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> HandleAsync(TQuery query);
}
```

### 2. **Domain-Driven Design Improvements**

```csharp
// Rich Domain Model
public class SudokuGame : AggregateRoot
{
    private readonly List<Cell> _cells;
    private readonly List<DomainEvent> _domainEvents;

    public GameId Id { get; private set; }
    public PlayerAlias PlayerAlias { get; private set; }
    public GameDifficulty Difficulty { get; private set; }
    public GameStatus Status { get; private set; }
    public GameStatistics Statistics { get; private set; }

    public void MakeMove(int row, int column, int value)
    {
        // Business logic validation
        if (Status != GameStatus.InProgress)
            throw new GameNotInProgressException();

        if (!IsValidMove(row, column, value))
            throw new InvalidMoveException();

        // Update state
        UpdateCell(row, column, value);
        UpdateStatistics();

        // Raise domain event
        AddDomainEvent(new MoveMadeEvent(Id, row, column, value));

        // Check for game completion
        if (IsGameComplete())
        {
            CompleteGame();
            AddDomainEvent(new GameCompletedEvent(Id));
        }
    }
}
```

### 3. **Repository Pattern with Specification**

```csharp
public interface IGameRepository
{
    Task<Game?> GetByIdAsync(GameId id);
    Task<IEnumerable<Game>> GetByPlayerAsync(PlayerAlias playerAlias);
    Task<IEnumerable<Game>> GetBySpecificationAsync(ISpecification<Game> spec);
    Task SaveAsync(Game game);
    Task DeleteAsync(GameId id);
}

public class GameByPlayerAndStatusSpecification : ISpecification<Game>
{
    private readonly PlayerAlias _playerAlias;
    private readonly GameStatus _status;

    public GameByPlayerAndStatusSpecification(PlayerAlias playerAlias, GameStatus status)
    {
        _playerAlias = playerAlias;
        _status = status;
    }

    public Expression<Func<Game, bool>> Criteria =>
        game => game.PlayerAlias == _playerAlias && game.Status == _status;
}
```

### 4. **Event-Driven Architecture**

```csharp
public interface IDomainEventDispatcher
{
    Task DispatchAsync(DomainEvent domainEvent);
}

public class GameCreatedEventHandler : IDomainEventHandler<GameCreatedEvent>
{
    public async Task HandleAsync(GameCreatedEvent @event)
    {
        // Handle game creation side effects
        // e.g., send notifications, update statistics
    }
}
```

### 5. **Result Pattern for Error Handling**

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}

// Usage in application services
public async Task<Result<GameDto>> CreateGameAsync(CreateGameCommand command)
{
    try
    {
        var game = Game.Create(command.PlayerAlias, command.Difficulty);
        await _gameRepository.SaveAsync(game);

        return Result<GameDto>.Success(GameDto.FromGame(game));
    }
    catch (DomainException ex)
    {
        return Result<GameDto>.Failure(ex.Message);
    }
}
```

### 6. **Improved Service Registration**

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IGameApplicationService, GameApplicationService>();
        services.AddScoped<ICreateGameUseCase, CreateGameUseCase>();
        services.AddScoped<IMakeMoveUseCase, MakeMoveUseCase>();

        // Register all command handlers
        services.Scan(scan => scan
            .FromAssemblyOf<CreateGameCommand>()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IGameRepository, AzureBlobGameRepository>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
```

## Migration Strategy

### Phase 1: Extract Domain Layer

1. Create `Sudoku.Domain` project
2. Move domain entities and business logic
3. Define domain events and exceptions

### Phase 2: Create Application Layer

1. Create `Sudoku.Application` project
2. Implement CQRS pattern
3. Create use cases and application services

### Phase 3: Refactor Infrastructure

1. Create `Sudoku.Infrastructure` project
2. Implement repository pattern
3. Add domain event handling

### Phase 4: Update Presentation Layers

1. Update API controllers to use application services
2. Implement proper error handling

## Benefits of Proposed Architecture

1. **Better Testability**: Clear separation allows for easier unit testing
2. **Maintainability**: Changes in one layer don't affect others
3. **Scalability**: Easy to add new features and modify existing ones
4. **Domain Focus**: Business logic is centralized and protected
5. **Flexibility**: Easy to swap implementations (e.g., different storage providers)
6. **Event-Driven**: Loose coupling through domain events
7. **Error Handling**: Consistent error handling across the application

## Technology Stack Recommendations

- **MediatR**: For CQRS implementation
- **FluentValidation**: For command validation
- **AutoMapper**: For DTO mapping
- **Serilog**: For structured logging
- **Polly**: For resilience and transient fault handling
- **HealthChecks**: For monitoring and health checks
