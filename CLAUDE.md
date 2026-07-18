# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture

This solution follows **Clean Architecture** with **DDD** and **CQRS** across six backend projects and one frontend project.

### Layer Dependency Flow
```
Presentation (API, React)
    ↓
Application (CQRS: Commands, Queries, Handlers)
    ↓
Domain (Entities, Value Objects, Domain Events)
    ↓
Infrastructure (Repositories, Azure Services, Event Dispatching)
```

Outer layers depend on inner layers only. No business logic lives in controllers or handlers — all invariants are enforced inside domain aggregates.

### Key Aggregates
- **SudokuGame** — The primary aggregate root. Owns game state transitions via methods like `MakeMove()`, `StartGame()`, `PauseGame()`, `ResumeGame()`. Raises domain events on state changes.
- **SudokuPuzzle** — Represents the puzzle grid. Contains a collection of `Cell` value objects and handles validation.
- **Cell** — Value object representing a single cell: value, fixed status, possible values.

### CQRS via MediatR
All application operations flow through `IMediator`. There are 14 commands (e.g., `CreateGame`, `MakeMove`, `UndoLastMove`, `ResetGame`) and 4 queries (e.g., `GetGame`, `GetPlayerGames`, `ValidateGame`). All handlers return `Result<T>` for unified error handling — never throw exceptions for expected failures.

### Domain Events
Domain aggregates raise events (e.g., `GameCreatedEvent`, `MoveMadeEvent`, `GameCompletedEvent`) with no knowledge of handlers. `IDomainEventDispatcher` in Infrastructure dispatches them after persistence.

### Repositories
Interfaces are defined in the **Application** layer; implementations live in **Infrastructure**:
- `CosmosDbGameRepository` — primary persistent store
- `AzureBlobGameRepository` — legacy store (being migrated away)
- `InMemoryPuzzleRepository` — puzzle generation only (performance optimization, not persisted)

### Testing Patterns
Tests use a container-based DI mocking framework (`DepenMock`). Test classes inherit from `BaseTestByAbstraction` or `BaseTestByType`. Resolve the SUT via `ResolveSut()` and mocks via `Container.ResolveMock<T>()`. Use AutoFixture for test data, FluentAssertions for assertions. CI enforces **80% line coverage**.

### CI/CD
GitHub Actions (`.github/workflows/main.yml`) builds, tests (with coverage threshold enforcement), and deploys to Azure on every merge to `main`. Coverage reports are posted as PR annotations.
