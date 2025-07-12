# Sudoku Domain Layer

This project contains the core domain logic for the Sudoku application, following Domain-Driven Design principles and Clean Architecture patterns.

## Structure

```
Sudoku.Domain/
├── Common/
│   └── AggregateRoot.cs          # Base class for domain entities
├── Entities/
│   ├── SudokuGame.cs             # Main game entity with business logic
│   └── SudokuPuzzle.cs           # Puzzle entity
├── Enums/
│   └── GameStatus.cs             # Game state enumeration
├── Events/
│   ├── DomainEvent.cs            # Base domain event
│   └── GameEvents.cs             # Game-related domain events
├── Exceptions/
│   ├── DomainException.cs        # Base domain exception
│   ├── GameNotInProgressException.cs
│   ├── InvalidMoveException.cs
│   ├── CellIsFixedException.cs
│   └── ...                       # Other domain-specific exceptions
├── Repositories/
│   ├── IGameRepository.cs        # Game persistence interface
│   └── IPuzzleRepository.cs      # Puzzle persistence interface
├── Services/
│   ├── IPuzzleValidator.cs       # Puzzle validation interface
│   └── PuzzleValidator.cs        # Puzzle validation implementation
├── ValueObjects/
│   ├── GameId.cs                 # Game identifier
│   ├── PlayerAlias.cs            # Player alias with validation
│   ├── GameDifficulty.cs         # Difficulty levels
│   ├── Cell.cs                   # Individual cell
│   └── GameStatistics.cs         # Game performance metrics
├── GlobalUsings.cs               # Global using statements
└── README.md                     # This file
```

## Key Components

### Entities

#### SudokuGame

The main aggregate root representing a complete Sudoku game:

- Manages game state transitions
- Validates moves according to Sudoku rules
- Tracks game statistics
- Raises domain events for state changes

#### SudokuPuzzle

Represents a Sudoku puzzle:

- Validates puzzle structure
- Checks for unique solutions
- Provides puzzle metadata

### Value Objects

#### GameId

Immutable game identifier with validation and conversion operators.

#### PlayerAlias

Player alias with business rules:

- 2-50 characters
- Alphanumeric and spaces only
- Trimmed and validated

#### GameDifficulty

Predefined difficulty levels (Easy, Medium, Hard, Expert) with validation.

#### Cell

Individual Sudoku cell with position and value validation:

- Row/column validation (0-8)
- Value validation (1-9)
- Fixed cell protection

#### GameStatistics

Game performance tracking:

- Move counting (total, valid, invalid)
- Play duration
- Accuracy percentage

### Domain Events

Events raised by domain entities for loose coupling:

- `GameCreatedEvent`
- `GameStartedEvent`
- `MoveMadeEvent`
- `GamePausedEvent`
- `GameResumedEvent`
- `GameCompletedEvent`
- `GameAbandonedEvent`

### Exceptions

Domain-specific exceptions for business rule violations:

- `GameNotInProgressException`
- `InvalidMoveException`
- `CellIsFixedException`
- `InvalidPlayerAliasException`
- And more...

## Usage Examples

### Creating a New Game

```csharp
var playerAlias = PlayerAlias.Create("JohnDoe");
var difficulty = GameDifficulty.Medium;
var initialCells = GeneratePuzzleCells(difficulty);

var game = SudokuGame.Create(playerAlias, difficulty, initialCells);
game.StartGame();
```

### Making a Move

```csharp
try
{
    game.MakeMove(0, 0, 5);
}
catch (InvalidMoveException ex)
{
    // Handle invalid move
}
catch (CellIsFixedException ex)
{
    // Handle attempt to modify fixed cell
}
```

### Handling Domain Events

```csharp
foreach (var domainEvent in game.DomainEvents)
{
    await _eventDispatcher.DispatchAsync(domainEvent);
}
game.ClearDomainEvents();
```

## Business Rules

1. **Game State Transitions**: Games follow strict state transitions (NotStarted → InProgress → Paused/Completed/Abandoned)
2. **Move Validation**: All moves must follow Sudoku rules (no duplicates in rows, columns, or 3x3 boxes)
3. **Fixed Cells**: Fixed cells cannot be modified
4. **Player Alias**: Must be 2-50 characters, alphanumeric and spaces only
5. **Puzzle Validation**: Puzzles must have exactly 81 cells and follow Sudoku rules

## Dependencies

This domain layer has no external dependencies and is completely self-contained. It can be used by any application layer without coupling to infrastructure concerns.
