# ADR-001 — Adoption of Clean Architecture

| Field      | Value                    |
|------------|--------------------------|
| **Date**   | 2026-04-15               |
| **Status** | Accepted                 |
| **Deciders** | Project maintainers    |

---

## Context

The Sudoku project began as a single-project Blazor application with no clear separation between UI, business logic, and data access. As the project grew in scope — adding puzzle generation, game state management, player tracking, and multiple frontend targets (Blazor and React/Vite) — the lack of layering made it difficult to:

- Test business logic in isolation without depending on infrastructure concerns
- Swap or evolve persistence backends (Blob Storage → Cosmos DB)
- Support multiple UI targets (Blazor and React/Vite) sharing the same backend logic
- Reason about where new code should live

A structural approach was needed that would enforce clear boundaries between business logic, application orchestration, and external concerns.

---

## Decision

The project adopts **Clean Architecture** (also known as Ports and Adapters / Hexagonal Architecture) as its structural foundation. The solution is organized into the following layers, each with a clearly defined responsibility:

### Layer Map

```
┌──────────────────────────────────────────────────────┐
│                  Presentation Layer                  │
│   Sudoku.Api (REST)  │  Sudoku.Blazor  │ Sudoku.React│
└──────────────────┬───────────────────────────────────┘
                   │ depends on
┌──────────────────▼───────────────────────────────────┐
│                 Application Layer                    │
│               Sudoku.Application                     │
│  Commands · Queries · Handlers · DTOs · Specs        │
└──────────────────┬───────────────────────────────────┘
                   │ depends on
┌──────────────────▼───────────────────────────────────┐
│                   Domain Layer                       │
│                 Sudoku.Domain                        │
│  Entities · Value Objects · Domain Events ·          │
│  Domain Exceptions · Repository Interfaces           │
└──────────────────────────────────────────────────────┘
                   ▲ implements interfaces defined in Application
┌──────────────────┴───────────────────────────────────┐
│               Infrastructure Layer                   │
│              Sudoku.Infrastructure                   │
│  CosmosDbGameRepository · InMemoryPuzzleRepository  │
│  AzureBlobGameRepository · Mappers · EventHandling  │
└──────────────────────────────────────────────────────┘
```

### Dependency Rule

**Dependencies point inward only.** Outer layers may depend on inner layers; inner layers must never depend on outer layers.

| Layer | Allowed dependencies |
|---|---|
| `Sudoku.Domain` | None — no project references |
| `Sudoku.Application` | `Sudoku.Domain` only |
| `Sudoku.Infrastructure` | `Sudoku.Application`, `Sudoku.Domain` |
| `Sudoku.Api` | `Sudoku.Application` only |
| `Sudoku.Blazor` / `Sudoku.React` | `Sudoku.Api` via HTTP (no direct project references to backend layers) |

### Layer Responsibilities

**Domain (`Sudoku.Domain`)**
- Aggregate roots (`SudokuGame`, `SudokuPuzzle`) with rich encapsulated business logic
- Value objects (`GameId`, `PlayerAlias`, `Cell`, `GameDifficulty`, `GameStatistics`)
- Domain events (`GameCreatedEvent`, `MoveMadeEvent`, `GameCompletedEvent`, etc.)
- Domain exceptions for all business rule violations
- Repository and service interfaces (`IGameRepository`, `IPuzzleRepository`, `IPuzzleGenerator`, `IPuzzleSolver`, `IPuzzleValidator`)

**Application (`Sudoku.Application`)**
- CQRS commands, queries, and their handlers
- Application service interfaces (`IGameApplicationService`, `IPlayerApplicationService`)
- DTOs used as the public contract across the API boundary
- Specifications for structured repository queries
- No business rules — orchestrates domain objects only

**Infrastructure (`Sudoku.Infrastructure`)**
- Implements repository interfaces from the Application layer
- `CosmosDbGameRepository` — primary game persistence
- `InMemoryPuzzleRepository` — scoped to puzzle generation; no I/O latency
- `AzureBlobGameRepository` — retained for future repurposing; not the active `IGameRepository` implementation
- Domain event dispatching, mappers, and external service integrations

**API (`Sudoku.Api`)**
- REST controllers calling Application layer services
- Request/response model binding and HTTP status code mapping
- CORS configuration, Swagger, and middleware registration
- No business logic; no direct domain model exposure

**Frontend (`Sudoku.Blazor`, `Sudoku.React`)**
- Communicate with the backend exclusively through `Sudoku.Api` HTTP endpoints
- No direct references to any backend project assembly

---

## Consequences

### Positive

- **Testability**: Domain and Application logic can be unit-tested with no infrastructure dependencies. Mocks target interfaces, not concrete implementations.
- **Replaceability**: Persistence backends (Blob → Cosmos DB) can be swapped without touching domain or application logic.
- **Multiple UIs**: Blazor and React/Vite share the same API surface without duplicating business logic.
- **Clear contribution boundaries**: Each layer has an explicit ownership contract — contributors know where new code belongs.
- **Domain focus**: Sudoku rules, state transitions, and invariants live exclusively in `Sudoku.Domain`, making them easy to find, review, and evolve.

### Tradeoffs

- **Indirection**: Simple operations require passing through multiple layers (controller → handler → domain → repository). This is an intentional tradeoff for testability and maintainability.
- **Mapping overhead**: DTOs require explicit mapping to/from domain objects. `SudokuGameMapper` in `Sudoku.Infrastructure` owns this translation.
- **Boilerplate**: Adding a new feature typically requires touching multiple projects (command, handler, DTO, controller). This is offset by the structure that prevents logic from accumulating in the wrong layer.

### Rules Enforced by This Decision

1. **Never reference `Sudoku.Infrastructure` or `Sudoku.Domain` directly from `Sudoku.Api`.** The API calls application service interfaces only.
2. **Never add business logic to handlers or controllers.** All invariants live on domain entities.
3. **Never reference a specific repository implementation from Application or Domain.** Depend on the interface.
4. **Domain events are raised by the domain, handled in Application or Infrastructure.** The domain has no knowledge of handlers.

---

## Related ADRs

- [ADR-002 — CQRS in the Application Layer](ADR-002-cqrs.md)
- [ADR-004 — Azure Cosmos DB as the Primary Game Persistence Backend](ADR-004-cosmosdb.md)
- [ADR-005 — In-Memory Repository Scoped to Puzzle Generation](ADR-005-in-memory-puzzle-repository.md)
