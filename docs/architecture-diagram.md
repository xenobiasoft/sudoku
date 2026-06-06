# Current Sudoku Solution Architecture

## Solution Overview

The solution is a multi-layered application following Clean Architecture with DDD and CQRS. It consists of 9 main projects:

```
Sudoku.sln
├── src/backend/
│   ├── Sudoku.Domain/                # Core Domain Library (Clean Architecture)
│   ├── Sudoku.Application/           # Application Layer (Clean Architecture)
│   ├── Sudoku.Infrastructure/        # Infrastructure Layer (Clean Architecture)
│   ├── Sudoku.Api/                   # REST API (.NET 10.0)
│   ├── Sudoku.AppHost/               # Application Orchestration (.NET 10.0)
│   ├── Sudoku.ServiceDefaults/       # Shared Service Configuration (.NET 10.0)
│   ├── Sudoku.McpServer/             # MCP Server for AI tooling (.NET 10.0)
│   ├── Sudoku.Functions/             # Azure Functions — puzzle pool seed & replenishment (.NET 8)
│   └── Tests/                        # Unit & Integration Tests (.NET 10.0)
├── src/frontend/
│   └── Sudoku.React/                 # React/Vite SPA (TypeScript)
└── Tests/E2E/                        # Playwright E2E Tests
```

## Architectural Diagram

```mermaid
graph TB
    %% External
    Azure[Azure Cloud Services]
    CosmosDB[Azure CosmosDB]
    BlobStorage[Azure Blob Storage]
    EventGrid[Azure Event Grid]
    KeyVault[Azure Key Vault]
    Browser[Web Browser]

    %% Orchestration
    subgraph "Sudoku.AppHost (Orchestration)"
        AppHost[Application Host]
        ServiceDefaults[Service Defaults]
    end

    %% Frontends
    subgraph "Sudoku.React (React/Vite SPA)"
        ReactApp[React Application]
        ReactComponents[React Components]
        ReactHooks[Custom Hooks]
    end

    %% API
    subgraph "Sudoku.Api (REST API)"
        GamesController[GamesController]
        GameActionsController[GameActionsController]
        GameStatusController[GameStatusController]
        PossibleValuesController[PossibleValuesController]
        ProfilesController[ProfilesController]
        Swagger[Swagger Documentation]
    end

    %% MCP Server
    subgraph "Sudoku.McpServer (MCP Server)"
        McpTools[ApplicationInsights Tools]
    end

    %% Azure Functions
    subgraph "Sudoku.Functions (Azure Functions)"
        SeedFunction[PuzzlePoolSeedFunction - Timer]
        ReplenishFunction[PuzzleReplenishFunction - Event Grid]
    end

    %% Domain Layer
    subgraph "Sudoku.Domain (Domain Layer)"
        DomainEntities[Entities: SudokuGame, SudokuPuzzle, UserProfile]
        DomainValueObjects[Value Objects]
        DomainEvents[Domain Events]
        DomainServices[Domain Services]
        DomainExceptions[Domain Exceptions]
        DomainRepositories[Repository Interfaces]
    end

    %% Application Layer
    subgraph "Sudoku.Application (Application Layer)"
        ApplicationCommands[Commands]
        ApplicationQueries[Queries]
        ApplicationHandlers[Command/Query Handlers]
        ApplicationServices[Application Services]
        ApplicationDTOs[DTOs]
        PuzzlePoolService[IPuzzlePoolService]
    end

    %% Infrastructure Layer
    subgraph "Sudoku.Infrastructure (Infrastructure Layer)"
        CosmosDbGameRepo[CosmosDbGameRepository]
        CosmosDbProfileRepo[CosmosDbUserProfileRepository]
        AzureBlobRepo[AzureBlobGameRepository]
        AzureBlobPuzzleRepo[AzureBlobPuzzleRepository]
        InMemoryPuzzleRepo[InMemoryPuzzleRepository]
        PuzzlePoolSvc[PuzzlePoolService]
        InfraServices[External Services]
        EventHandlers[Domain Event Handlers]
    end

    %% Tests
    subgraph "Tests"
        UnitTests[Unit & Integration Tests]
        E2ETests[E2E Tests - Playwright]
    end

    %% Dependencies
    AppHost --> GamesController
    AppHost --> ReactApp
    AppHost --> KeyVault
    AppHost --> SeedFunction

    Browser --> ReactApp

    ReactApp --> GamesController

    GamesController --> ApplicationServices
    GameActionsController --> ApplicationServices
    GameStatusController --> ApplicationServices
    PossibleValuesController --> ApplicationServices
    ProfilesController --> ApplicationServices

    ApplicationServices --> DomainEntities
    ApplicationHandlers --> DomainEntities
    ApplicationHandlers --> CosmosDbGameRepo
    ApplicationHandlers --> CosmosDbProfileRepo
    ApplicationHandlers --> PuzzlePoolService

    PuzzlePoolService --> PuzzlePoolSvc
    PuzzlePoolSvc --> AzureBlobPuzzleRepo

    DomainEntities --> DomainValueObjects
    DomainEntities --> DomainEvents
    DomainEntities --> DomainServices
    DomainEntities --> DomainExceptions

    CosmosDbGameRepo --> DomainRepositories
    CosmosDbGameRepo --> CosmosDB
    CosmosDbProfileRepo --> DomainRepositories
    CosmosDbProfileRepo --> CosmosDB
    AzureBlobRepo --> DomainRepositories
    AzureBlobRepo --> BlobStorage
    AzureBlobPuzzleRepo --> BlobStorage
    InMemoryPuzzleRepo --> DomainRepositories

    InfraServices --> Azure
    EventHandlers --> DomainEvents

    BlobStorage -->|BlobDeleted| EventGrid
    EventGrid --> ReplenishFunction
    ReplenishFunction --> PuzzlePoolSvc
    SeedFunction --> PuzzlePoolSvc

    UnitTests --> DomainEntities
    UnitTests --> ApplicationServices
    UnitTests --> GamesController
    E2ETests --> ReactApp

    %% Styling
    classDef domain fill:#28A745,stroke:#333,stroke-width:2px,color:#fff
    classDef application fill:#007BFF,stroke:#333,stroke-width:2px,color:#fff
    classDef infrastructure fill:#FFC107,stroke:#333,stroke-width:2px,color:#000
    classDef presentation fill:#17A2B8,stroke:#333,stroke-width:2px,color:#fff
    classDef orchestration fill:#6F42C1,stroke:#333,stroke-width:2px,color:#fff
    classDef test fill:#6C757D,stroke:#333,stroke-width:2px,color:#fff
    classDef external fill:#343A40,stroke:#333,stroke-width:2px,color:#fff
    classDef functions fill:#E83E8C,stroke:#333,stroke-width:2px,color:#fff

    class DomainEntities,DomainValueObjects,DomainEvents,DomainServices,DomainExceptions,DomainRepositories domain
    class ApplicationCommands,ApplicationQueries,ApplicationHandlers,ApplicationServices,ApplicationDTOs,PuzzlePoolService application
    class CosmosDbGameRepo,CosmosDbProfileRepo,AzureBlobRepo,AzureBlobPuzzleRepo,InMemoryPuzzleRepo,PuzzlePoolSvc,InfraServices,EventHandlers infrastructure
    class GamesController,GameActionsController,GameStatusController,PossibleValuesController,ProfilesController,Swagger,ReactApp,ReactComponents,ReactHooks presentation
    class AppHost,ServiceDefaults,McpTools orchestration
    class SeedFunction,ReplenishFunction functions
    class UnitTests,E2ETests test
    class Azure,Browser,CosmosDB,BlobStorage,EventGrid,KeyVault external
```

## Key Components

### **Sudoku.Domain (Domain Layer)**

- **Purpose**: Core business logic and domain models following DDD principles
- **Entities**: `SudokuGame`, `SudokuPuzzle`, `UserProfile`
- **Value Objects**: `GameId`, `PlayerAlias`, `GameDifficulty`, `Cell`, `GameStatistics`, `ProfileId`
- **Domain Events**: `GameCreatedEvent`, `MoveMadeEvent`, `GameCompletedEvent`, `ProfileEvents`
- **Domain Services**: `PuzzleValidator`
- **Repository Interfaces**: `IGameRepository`, `IPuzzleRepository`, `IUserProfileRepository`

### **Sudoku.Application (Application Layer)**

- **Purpose**: Application use cases and orchestration via CQRS
- **Commands**: 16 command types (e.g. `CreateGameCommand`, `MakeMoveCommand`, `UndoLastMoveCommand`)
- **Queries**: 5 query types (e.g. `GetGameQuery`, `GetPlayerGamesQuery`)
- **Handlers**: 22+ command/query handlers
- **DTOs**: Request/response models

### **Sudoku.Infrastructure (Infrastructure Layer)**

- **Purpose**: External concerns and data persistence
- **Repositories**:
  - `CosmosDbGameRepository` — primary game store
  - `CosmosDbUserProfileRepository` — primary profile store
  - `AzureBlobGameRepository` — legacy blob snapshot store
  - `AzureBlobPuzzleRepository` — pre-generated puzzle pool store (`sudoku-puzzles` container)
  - `InMemoryPuzzleRepository` — transient puzzle state during generation (not persisted)
- **Services**: `CosmosDbService`, `AzureStorageService`, `PuzzleGenerator`, `StrategyBasedPuzzleSolver` (12+ strategies), `PuzzlePoolService`
- **Event Handling**: `IDomainEventDispatcher` dispatches domain events after persistence

### **Sudoku.Functions (Azure Functions)**

- **Purpose**: Background puzzle pool maintenance via event-driven and timer-based triggers
- **Functions**:
  - `PuzzlePoolSeedFunction` — Timer trigger (`0 0 2 * * *`); fills each difficulty's pool to 10 pre-generated puzzles nightly
  - `PuzzleReplenishFunction` — Event Grid trigger (`BlobDeleted`); replaces each consumed puzzle one-for-one in real time

### **Sudoku.Api (REST API)**

- **Purpose**: REST API presentation layer
- **Controllers**: `GamesController`, `GameActionsController`, `GameStatusController`, `PossibleValuesController`, `ProfilesController`
- **Swagger**: Full API documentation

### **Sudoku.React (React/Vite SPA)**

- **Purpose**: React/TypeScript single-page application
- **Components**: `GameBoard`, `CellInput`, `GameControls`, `GameStats`, `GameThumbnail`, `VictoryDisplay`
- **Pages**: `HomePage`, `NewGamePage`, `GamePage`, `ProfilePage`, `CreateProfilePage`
- **Hooks**: `useGameService`, `usePlayerService`

### **Sudoku.McpServer (MCP Server)**

- **Purpose**: MCP server exposing Azure Application Insights tools for AI tooling integrations

### **Sudoku.AppHost (Orchestration)**

- **Purpose**: .NET Aspire application host — orchestrates all services, CosmosDB emulator, and Azure Key Vault

## Data Flow

### API Request Flow

1. **Client** (React) → `Sudoku.Api` controllers
2. **Controllers** → Application services / MediatR handlers
3. **Handlers** → Domain aggregates (business rules enforced)
4. **Handlers** → Infrastructure repositories (persistence)
5. **Repositories** → CosmosDB (primary store)
6. **Domain events** → dispatched post-persistence via `IDomainEventDispatcher`

## Technology Stack

- **.NET 10.0**: All backend projects
- **React/TypeScript + Vite**: SPA frontend
- **Azure CosmosDB**: Primary persistent store
- **Azure Blob Storage**: Legacy game snapshot store
- **Azure Key Vault**: Secure configuration management
- **Azure App Configuration**: Feature flags and runtime config
- **.NET Aspire**: Service orchestration and developer dashboard
- **MediatR**: CQRS command/query dispatching
- **Playwright**: E2E testing
- **Swagger**: API documentation

## Status

- ✅ **Domain Layer**: Complete — `SudokuGame`, `SudokuPuzzle`, `UserProfile` aggregates
- ✅ **Application Layer**: Complete — 16 commands, 5 queries, 22+ handlers
- ✅ **Infrastructure Layer**: Complete — CosmosDB primary, blob as secondary
- ✅ **REST API**: Complete — 5 controllers, full Swagger docs
- ✅ **Application Orchestration**: Complete — Aspire AppHost with CosmosDB emulator
- ✅ **Storage Migration**: Complete — CosmosDB is primary; `AzureBlobGameRepository` retained as secondary
- ✅ **React Frontend**: Complete — sole frontend, full SPA with E2E test coverage
- ✅ **MCP Server**: Complete — ApplicationInsights tooling exposed
- ✅ **Blazor Retirement**: Complete — archived to `archive/Sudoku.Blazor`; React is the canonical UI
- ✅ **Pre-Generated Puzzle Pool**: Complete — `AzureBlobPuzzleRepository`, `PuzzlePoolService`, `Sudoku.Functions` with event-driven (Event Grid) and nightly timer replenishment
