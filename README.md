# Sudoku Game

A modern, full-featured Sudoku game built with Blazor Server and C#, following Clean Architecture and Domain-Driven Design (DDD) principles. The solution is modular, testable, and cloud-ready, featuring a clean interactive UI, multiple difficulty levels, and robust backend logic.

## Solution Overview

This solution is organized into several projects:


```
Sudoku.sln
├── Sudoku.Api/              # REST API
├── Sudoku.AppHost/          # Application orchestration/hosting
├── Sudoku.Application/      # Application use cases and orchestration
├── Sudoku.Domain/           # Core domain logic and business rules
├── Sudoku.Infrastructure/   # Infrastructure (storage, external APIs)
├── Sudoku.Web.Server/       # Blazor Server web UI
└── Tests/                   # Unit and integration tests
```

### Key Technologies & Patterns

- **.NET 10.0** throughout
- **Blazor Server** for real-time, interactive UI
- **Domain-Driven Design (DDD)**: Rich domain models, value objects, domain events
- **Clean Architecture**: Clear separation of concerns between domain, application, infrastructure, and UI
- **CQRS Pattern**: Commands and queries are handled separately for scalability and maintainability
- **Azure Storage**: Cloud persistence for game state
- **Dependency Injection**: For all services and repositories
- **Comprehensive Testing**: Unit and integration tests for all layers

### Data Flow

1. **User Interaction**: Browser → Blazor Server → SignalR Hub
2. **Game Logic**: Web Services → Domain Layer
3. **State Management**: Game state stored in memory, local storage, or Azure
4. **Persistence**: Azure Blob Storage with optional caching

## Features

- Multiple difficulty levels (Easy, Medium, Hard)
- Real-time, interactive UI
- Game state persistence (local and cloud)
- Undo/redo, pencil mode, and validation
- Modular, extensible architecture

## Building the Project

**Prerequisites:**

- [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- (Optional) Azure account for cloud storage features

To build the solution:

```bash
dotnet build
```

## Running the Project

To run the Blazor Server app locally:

```bash
dotnet run --project Sudoku.Web.Server
```

The app will be available at `https://localhost:5001` (or as indicated in the console output).

## Testing

To run all tests:

```bash
dotnet test
```

## Architecture Diagram

For a detailed architecture diagram and explanation, see `current-architecture-diagram.md` in the root of the repository.
