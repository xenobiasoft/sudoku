# Migrating Blazor Sudoku to Use the API

## Overview

This document outlines the steps required to migrate the Blazor Sudoku game from using the local Sudoku project to using the Sudoku API. This migration will decouple the frontend from the backend logic, making it easier to maintain and scale.

## Current State

- The Blazor project (`Sudoku.Web.Server`) directly references the `Sudoku` project via a project reference in its `.csproj` file
- Game logic, state management, and puzzle validation are handled locally using classes and interfaces from the `Sudoku` project
- Services like `IGameStateManager`, `IGameSessionManager`, and others operate on in-memory or local representations of the game state

## Target State

- The Blazor frontend will interact with a backend API (`Sudoku.Api`) for all game logic, state management, and validation
- The API exposes endpoints for creating games, making moves, validating puzzles, and managing player sessions

## Migration Steps

### 1. Remove Direct Project Reference

**File:** `Sudoku.Web.Server/Sudoku.Web.Server.csproj`

**Action:** Remove the following line from the `<ItemGroup>` section:

```xml
<ProjectReference Include="..\Sudoku\Sudoku.csproj" />
```

**Additional:** Remove any other direct references to `Sudoku.Domain`, `Sudoku.Application`, etc., unless needed for DTOs only.

### 2. Refactor Game State Management

Replace direct instantiation and manipulation of `SudokuPuzzle`, `GameStateMemory`, etc., with API calls.

**Key Services to Refactor:**

#### `IGameStateManager`

- `LoadGameAsync` → `GET /api/players/{alias}/games/{gameId}`
- `SaveGameAsync` → `PUT /api/players/{alias}/games/{gameId}` (if needed)
- `DeleteGameAsync` → `DELETE /api/players/{alias}/games/{gameId}`
- `ResetGameAsync` → `POST /api/players/{alias}/games/{gameId}/reset`
- `UndoGameAsync` → `POST /api/players/{alias}/games/{gameId}/undo`

#### `IGameSessionManager`

- Session management should be handled server-side via API calls
- Timer and session state should be synchronized with the API

### 3. Update UI Event Handlers

All user actions that modify the game should trigger API requests:

#### Making a Move

```csharp
// Current
Puzzle.SetCell(row, column, value);

// New
await httpClient.PutAsync($"/api/players/{alias}/games/{gameId}",
    JsonContent.Create(new { Row = row, Column = column, Value = value }));
```

#### Adding/Removing Possible Values

```csharp
// Add possible value
await httpClient.PostAsync($"/api/players/{alias}/games/{gameId}/possiblevalues",
    JsonContent.Create(new { Row = row, Column = column, Value = value }));

// Remove possible value
await httpClient.DeleteAsync($"/api/players/{alias}/games/{gameId}/possiblevalues",
    JsonContent.Create(new { Row = row, Column = column, Value = value }));
```

#### Validating the Game

```csharp
// Current
var isValid = Puzzle.IsValid();

// New
var response = await httpClient.PostAsync($"/api/players/{alias}/games/{gameId}/validate", null);
var validationResult = await response.Content.ReadFromJsonAsync<ValidationResultDto>();
```

### 4. Synchronize Game State

After each API call, update the local UI state with the latest game state returned from the API:

```csharp
private async Task UpdateGameStateFromApi()
{
    var response = await httpClient.GetAsync($"/api/players/{alias}/games/{gameId}");
    var gameDto = await response.Content.ReadFromJsonAsync<GameDto>();

    // Update local UI state with gameDto data
    UpdatePuzzleDisplay(gameDto);
    StateHasChanged();
}
```

### 5. Authentication/Player Management

Ensure the Blazor app manages and sends the appropriate player identifiers:

```csharp
// Get or create player alias
var alias = await AliasService.GetAliasAsync();

// Include alias in all API calls
var response = await httpClient.GetAsync($"/api/players/{alias}/games");
```

### 6. Testing and Validation

- Thoroughly test all game flows (new game, move, undo, reset, validate, win/lose) to ensure they work via the API
- Remove or refactor any unit/integration tests that depended on local game logic
- Add new tests for API integration

## API Endpoints Reference

| Endpoint                                                   | Method | Description                 |
| ---------------------------------------------------------- | ------ | --------------------------- |
| `/api/players/{alias}/games`                               | GET    | List all games for a player |
| `/api/players/{alias}/games/{gameId}`                      | GET    | Get a specific game         |
| `/api/players/{alias}/games/{difficulty}`                  | POST   | Create a new game           |
| `/api/players/{alias}/games/{gameId}`                      | PUT    | Make a move                 |
| `/api/players/{alias}/games/{gameId}/possiblevalues`       | POST   | Add possible value          |
| `/api/players/{alias}/games/{gameId}/possiblevalues`       | DELETE | Remove possible value       |
| `/api/players/{alias}/games/{gameId}/possiblevalues/clear` | DELETE | Clear all possible values   |
| `/api/players/{alias}/games/{gameId}/validate`             | POST   | Validate the game           |
| `/api/players/{alias}/games/{gameId}/undo`                 | POST   | Undo last move              |
| `/api/players/{alias}/games/{gameId}/reset`                | POST   | Reset game to initial state |
| `/api/players/{alias}/games/{gameId}`                      | DELETE | Delete a game               |

## Migration Summary Table

| Current Usage (Local)         | New Usage (API)                                     |
| ----------------------------- | --------------------------------------------------- |
| `new SudokuPuzzle()`          | `GET /api/players/{alias}/games/{gameId}`           |
| `puzzle.SetCell(row, col, v)` | `PUT /api/players/{alias}/games/{gameId}`           |
| `puzzle.IsValid()`            | `POST /api/players/{alias}/games/{gameId}/validate` |
| `GameStateManager.*`          | API client methods calling above endpoints          |
| `GameSessionManager.*`        | API client methods calling above endpoints          |

## Additional Considerations

### DTO Mapping

Ensure the Blazor app uses the same DTOs as the API for game state, moves, and validation results:

```csharp
public record GameDto
{
    public string Id { get; init; } = string.Empty;
    public string PlayerAlias { get; init; } = string.Empty;
    public string Difficulty { get; init; } = string.Empty;
    public GameStatus Status { get; init; }
    public List<CellDto> Board { get; init; } = new();
    public TimeSpan PlayDuration { get; init; }
    public int TotalMoves { get; init; }
}

public record MoveRequest(int Row, int Column, int? Value);
public record PossibleValueRequest(int Row, int Column, int Value);
public record CellRequest(int Row, int Column);
public record ValidationResultDto(bool IsValid, List<string> Errors);
```

### Performance Considerations

- Consider caching or local storage for offline/optimistic UI updates if needed
- Implement loading indicators for API calls
- Handle network latency gracefully

### Error Handling

Implement robust error handling for API failures, timeouts, and invalid responses:

```csharp
private async Task<GameDto?> GetGameAsync(string alias, string gameId)
{
    try
    {
        var response = await httpClient.GetAsync($"/api/players/{alias}/games/{gameId}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<GameDto>();
        }

        // Handle different error status codes
        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                // Handle game not found
                break;
            case HttpStatusCode.BadRequest:
                // Handle invalid request
                break;
            default:
                // Handle other errors
                break;
        }
    }
    catch (HttpRequestException ex)
    {
        // Handle network errors
        Console.WriteLine($"Network error: {ex.Message}");
    }

    return null;
}
```

### HTTP Client Configuration

Add HTTP client configuration to the Blazor app:

```csharp
// In Program.cs or service configuration
builder.Services.AddHttpClient("SudokuApi", client =>
{
    client.BaseAddress = new Uri("https://your-api-url.com/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
```

## Conclusion

This migration will:

- Decouple the frontend from backend logic
- Enable better scalability and maintenance
- Allow for easier testing and deployment
- Provide a cleaner separation of concerns

The key is to replace all local game logic with API calls while maintaining the same user experience. Careful attention should be paid to error handling, loading states, and data synchronization to ensure a smooth user experience.
