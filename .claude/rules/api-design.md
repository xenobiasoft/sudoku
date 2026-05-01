---
paths:
  - "src/backend/Sudoku.Api/**/*.cs"
---

# API Design Guidelines

## CQRS Command vs Query Separation

**Command endpoints** (`POST`, `PUT`, `PATCH`, `DELETE`) mutate state and **must not return domain data**:
- `201 Created` with a `Location` header for creates
- `204 No Content` for updates and deletes

**Query endpoints** (`GET`) read state and return data.

If a client needs the updated resource after a command, it issues a separate GET. Mixing the two breaks CQRS separation.

## REST Controller Examples

```csharp
// COMMAND endpoint — mutates state, returns no data
[HttpPost("{alias}/games/{difficulty}")]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult> CreateGame(string alias, string difficulty)
{
    var result = await _gameService.CreateGameAsync(alias, difficulty);
    if (!result.IsSuccess)
        return BadRequest(result.Error);

    return Created($"/api/players/{alias}/games/{result.Value.Id}", null);
}

// QUERY endpoint — reads state, returns data
[HttpGet("{alias}/games/{gameId}")]
[ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<GameDto>> GetGame(string alias, string gameId)
{
    var result = await _gameService.GetGameAsync(gameId);
    if (!result.IsSuccess)
        return NotFound();

    return Ok(result.Value);
}
```

## Rules
- Validate all user inputs at the controller boundary
- Use `Result<T>` from the application layer; map to HTTP status codes in the controller
- Include XML documentation for public API endpoints
- Never return domain data from command endpoints
