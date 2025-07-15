# GitHub Copilot Instructions for Sudoku Project

## Project Overview

This is a modern Sudoku game built with .NET, Blazor, and C# following Clean Architecture principles. The project emphasizes Test-Driven Development (TDD), domain-driven design, and maintainable code structure.

## Architecture & Project Structure

### Solution Structure

```
Sudoku.sln
├── Sudoku.Domain/                    # Core domain entities and business rules
├── Sudoku.Application/               # Application use cases and orchestration
├── Sudoku.Infrastructure/            # External concerns (storage, external APIs)
├── Sudoku.Web.Server/                # Blazor Server presentation layer
├── Sudoku.Api/                       # REST API presentation layer
├── Sudoku.AppHost/                   # Application orchestration
├── Sudoku.Storage.Azure/             # Azure storage implementations
├── Sudoku.ServiceDefaults/           # Default service configurations
└── Tests/                           # Unit and integration tests
```

### Key Architectural Principles

1. **Clean Architecture**: Follow dependency inversion principle with clear layer separation
2. **Domain-Driven Design**: Rich domain models with encapsulated business logic
3. **CQRS Pattern**: Separate commands and queries for better performance and maintainability
4. **Repository Pattern**: Abstract data access through interfaces
5. **Event-Driven Architecture**: Use domain events for loose coupling

## Coding Standards & Conventions

### C# Coding Standards

- **Naming Conventions**:

  - Use PascalCase for public members, classes, and methods
  - Use camelCase for private fields and local variables
  - Use UPPER_CASE for constants
  - Prefix private fields with underscore: `_fieldName`

- **File Organization**:

  - One public class per file
  - File name should match class name
  - Group related classes in appropriate namespaces

- **Code Style**:
  - Use expression-bodied members when appropriate
  - Prefer `var` for local variables when type is obvious
  - Use `readonly` for immutable fields
  - Use Primary constructors when possible
  - Always use curly braces for code blocks

### Domain Layer Guidelines

```csharp
// Example domain entity structure
public class SudokuGame : AggregateRoot
{
    private readonly List<Cell> _cells;
    private readonly List<DomainEvent> _domainEvents;

    public GameId Id { get; private set; }
    public PlayerAlias PlayerAlias { get; private set; }
    public GameDifficulty Difficulty { get; private set; }
    public GameStatus Status { get; private set; }

    // Private constructor for EF Core
    private SudokuGame() { }

    // Public factory method
    public static SudokuGame Create(PlayerAlias playerAlias, GameDifficulty difficulty)
    {
        // Validation and creation logic
    }

    // Domain methods that encapsulate business logic
    public void MakeMove(int row, int column, int value)
    {
        // Business validation
        // State changes
        // Domain event raising
    }
}
```

### Application Layer Guidelines

```csharp
// Use CQRS pattern
public record CreateGameCommand(string PlayerAlias, GameDifficulty Difficulty);
public record GetGameQuery(GameId GameId);

// Command/Query handlers
public class CreateGameCommandHandler : ICommandHandler<CreateGameCommand>
{
    private readonly IGameRepository _gameRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public async Task<Result> HandleAsync(CreateGameCommand command)
    {
        // Application logic
        // Domain interaction
        // Persistence
    }
}
```

### Infrastructure Layer Guidelines

```csharp
// Repository implementations
public class AzureBlobGameRepository : IGameRepository
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobGameRepository> _logger;

    public async Task<Game?> GetByIdAsync(GameId id)
    {
        // Implementation with proper error handling
    }
}
```

## Testing Guidelines

### Unit Testing Standards

- **Test Naming**: `[MethodName]_[Scenario]_[ExpectedResult]`
- **Arrange-Act-Assert**: Clear separation of test phases
- **Test Isolation**: Each test should be independent
- **Mocking**: Use mocks for external dependencies
- **Single Assert**: Only have one assert per test

The unit tests use the library [DepenMock](https://github.com/xenobiasoft/depenmock). All test classes should inherit from BaseTestByAbstraction,
if the **sut** implements an interface or inherits from a base class. Otherwise, the test class should inherit from BaseTestByType.
The **sut** should be resolved using the method `ResolveSut()`. All mocks should be resolved in the constructor of the test and added as a private class variable using the method `Container.ResolveMock<>()`.

```csharp
[Test]
public void MakeMove_ValidMove_RaisesEvent()
{
    // Arrange
    var sut = ResolveSut();
    var initialEventCount = sut.DomainEvents.Count;

    // Act
    sut.MakeMove(0, 0, 5);

    // Assert
    sut.DomainEvents.Count.Should().Be(initialEventCount + 1);
}

[Test]
public void MakeMove_ValidMove_UpdatesCell()
{
    // Arrange
    var sut = ResolveSut();

    // Act
    sut.MakeMove(0, 0, 5);

    // Assert
    sut.Cells[0, 0].Value.Should().Be(5);
}
```

### Integration Testing

- Test complete workflows
- Use in-memory databases for testing
- Test API endpoints with real HTTP requests

## Error Handling Patterns

### Domain Exceptions

```csharp
public class InvalidMoveException : DomainException
{
    public InvalidMoveException(int row, int column, int value)
        : base($"Invalid move: {value} at position ({row}, {column})")
    {
    }
}
```

### Result Pattern

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    public static Result<T> Success(T value)
    {
        return new(true, value, null);
    }

    public static Result<T> Failure(string error)
    {
        return new(false, default, error);
    }
}
```

## Dependency Injection Guidelines

### Service Registration

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSudokuServices(this IServiceCollection services)
    {
        // Register domain services
        services.AddScoped<IGameRepository, AzureBlobGameRepository>();
        services.AddScoped<ICommandHandler<CreateGameCommand>, CreateGameCommandHandler>();

        // Register application services
        services.AddScoped<IGameApplicationService, GameApplicationService>();

        return services;
    }
}
```

## Blazor Component Guidelines

### Component Structure

```csharp
@page "/game/{GameId}"
@using Sudoku.Application
@inject IGameApplicationService GameService

<PageTitle>Sudoku Game</PageTitle>

<div class="sudoku-board">
    @if (game != null)
    {
        <SudokuBoard Game="@game" OnMoveMade="HandleMoveMade" />
    }
</div>

@code {
    [Parameter] public string GameId { get; set; } = string.Empty;
    private GameDto? game;

    protected override async Task OnInitializedAsync()
    {
        var result = await GameService.GetGameAsync(GameId);
        if (result.IsSuccess)
        {
            game = result.Value;
        }
    }

    private async Task HandleMoveMade(int row, int column, int value)
    {
        // Handle move logic
    }
}
```

## API Design Guidelines

### REST API Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameApplicationService _gameService;

    [HttpPost]
    public async Task<ActionResult<GameDto>> CreateGame(CreateGameRequest request)
    {
        var command = new CreateGameCommand(request.PlayerAlias, request.Difficulty);
        var result = await _gameService.CreateGameAsync(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else
        {
            return BadRequest(result.Error);
        }
    }
}
```

## Performance Considerations

1. **Async/Await**: Use consistently throughout the application
2. **Caching**: Implement appropriate caching strategies
3. **Database Queries**: Optimize queries and use proper indexing
4. **Memory Management**: Dispose of resources properly

## Security Guidelines

1. **Input Validation**: Validate all user inputs
2. **Authentication**: Implement proper authentication mechanisms
3. **Authorization**: Use role-based access control
4. **Data Protection**: Encrypt sensitive data

## Documentation Standards

1. **XML Documentation**: Use XML comments for public APIs
2. **README Files**: Keep documentation up to date
3. **Architecture Decisions**: Document significant architectural decisions

## Git Workflow

1. **Branch Naming**: Use descriptive branch names (e.g., `feature/game-validation`)
2. **Commit Messages**: Use conventional commit format
3. **Pull Requests**: Require code review before merging
4. **Testing**: Ensure all tests pass before merging

## Common Patterns to Follow

### Value Objects

```csharp
public record GameId(Guid Value)
{
    public static GameId New()
    {
        return new(Guid.NewGuid());
    }

    public static GameId FromString(string value)
    {
        return new(Guid.Parse(value));
    }
}
```

### Domain Events

```csharp
public record GameCreatedEvent(GameId GameId, PlayerAlias PlayerAlias, GameDifficulty Difficulty) : DomainEvent;
public record MoveMadeEvent(GameId GameId, int Row, int Column, int Value) : DomainEvent;
```

### Specifications

```csharp
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}

public class GameByPlayerSpecification : ISpecification<Game>
{
    private readonly PlayerAlias _playerAlias;

    public Expression<Func<Game, bool>> Criteria
    {
        get
        {
            return game => game.PlayerAlias == _playerAlias;
        }
    }
}
```

## When Suggesting Code

1. **Follow the established patterns** in the codebase
2. **Consider the architectural layers** and their responsibilities
3. **Include proper error handling** and validation
4. **Write testable code** that can be easily unit tested
5. **Use dependency injection** for external dependencies
6. **Follow C# best practices** and conventions
7. **Consider performance implications** of your suggestions
8. **Include XML documentation** for public APIs
9. **Use the Result pattern** for error handling in application services
10. **Raise domain events** for important state changes

## Anti-Patterns to Avoid

1. **Anemic Domain Models**: Don't create domain entities that are just data containers
2. **Tight Coupling**: Avoid direct dependencies between layers
3. **God Objects**: Don't create classes with too many responsibilities
4. **Primitive Obsession**: Use value objects instead of primitive types for domain concepts
5. **Magic Numbers**: Use constants or enums instead of magic numbers
6. **Long Methods**: Keep methods focused and under 20 lines when possible
7. **Deep Nesting**: Avoid deeply nested if statements and loops

Remember: This is a learning project focused on TDD, clean architecture, and modern .NET development practices. Prioritize code quality, testability, and maintainability over quick solutions.
