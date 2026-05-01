---
paths:
  - "src/backend/Sudoku.Infrastructure/**/*.cs"
---

# Infrastructure Layer Guidelines

## Repository Implementations
```csharp
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

## Rules
- Implement interfaces defined in the Application layer — never the reverse
- `CosmosDbGameRepository` is the primary persistent store
- `InMemoryPuzzleRepository` is for puzzle generation only (performance optimization, not persisted)
- `IDomainEventDispatcher` dispatches domain events after persistence
- Use `async/await` consistently throughout
- Dispose of resources properly
