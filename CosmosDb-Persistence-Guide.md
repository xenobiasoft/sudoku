# Using CosmosDb for Persistence in the Infrastructure Project

## 1. Current Persistence Overview

- **Game Persistence**: Handled by `AzureBlobGameRepository` (implements `IGameRepository`), using `IAzureStorageService` for Azure Blob Storage.
- **Puzzle Persistence**: Handled by `InMemoryPuzzleRepository` (implements `IPuzzleRepository`).
- Dependency injection is configured in `InfrastructureServiceCollectionExtensions.cs`.

## 2. Migrating to CosmosDb

### a. Repository Implementation

- Create `CosmosDbGameRepository : IGameRepository`.
- Implement all required methods from `IGameRepository`.

### b. CosmosDb Service Abstraction

- Create an interface (e.g., `ICosmosDbService`) for CosmosDb operations.
- Implement this service using the Azure CosmosDb SDK.

### c. Dependency Injection Registration

- Register the CosmosDb client and your new service/repository in `InfrastructureServiceCollectionExtensions.cs`.
- Register `IGameRepository` as `CosmosDbGameRepository`.

### d. Configuration

- Add CosmosDb configuration (endpoint, key, database, container) to your configuration files and bind in DI.

### e. Domain Model Mapping

- **CosmosDb Serialization Requirements:**
  - Use JSON serialization attributes (e.g., `[JsonPropertyName]`, `[JsonConstructor]`) on your domain entities to ensure correct mapping to/from CosmosDb documents.
  - If your entities use value objects or complex types, consider implementing custom JSON converters or mapping to DTOs for persistence.
  - Be aware that private setters or constructors (common in Clean Architecture) can prevent deserialization; use `[JsonConstructor]` or make setters public as needed.
  - See [System.Text.Json documentation](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-customize-properties?pivots=dotnet-7-0) for more details.

### f. Testing

- Implement tests for the new repository.

## 3. Aspire Project Configuration

### a. Add CosmosDb Resource to Aspire AppHost

In your `Sudoku.AppHost/Program.cs`, add CosmosDb as a resource:

```csharp
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
    .WithDatabase("SudokuDb")
    .WithContainer("Games", "/gameId"); // Use '/gameId' as the partition key for efficient game lookups

builder.AddProject<Projects.Sudoku_Api>("sudoku-api")
    .WithReference(cosmosDb)
    .WithReference(keyVault);

builder.AddProject<Projects.Sudoku_Web_Server>("sudoku-blazor")
    .WithReference(keyVault)
    .WithExternalHttpEndpoints();
```

### b. Add CosmosDb Connection to Configuration

In `appsettings.json` (or via environment variables/secrets):

```json
"ConnectionStrings": {
  "CosmosDb": "AccountEndpoint=https://<your-account>.documents.azure.com:443/;AccountKey=<your-account-key>;"
  // See: https://learn.microsoft.com/azure/cosmos-db/connect-to-account for details on obtaining your CosmosDb connection string
},
"CosmosDb": {
  "Database": "SudokuDb",
  "Container": "Games"
}
```

### c. Update DI Setup

In `InfrastructureServiceCollectionExtensions.cs`, register the CosmosDb client and your service:

```csharp
services.AddSingleton<CosmosClient>(sp =>
    new CosmosClient(configuration.GetConnectionString("CosmosDb")));
services.AddScoped<ICosmosDbService, CosmosDbService>();
services.AddScoped<IGameRepository, CosmosDbGameRepository>();
```

### d. Local Development

- Aspire can provision a local CosmosDb emulator for development.
- Ensure your AppHost project references the CosmosDb resource and passes the connection string to your API and server projects.

## 4. Key Files to Update or Add

- `Sudoku.Infrastructure/Repositories/CosmosDbGameRepository.cs` (new)
- `Sudoku.Infrastructure/Services/ICosmosDbService.cs` (new)
- `Sudoku.Infrastructure/Services/CosmosDbService.cs` (new)
- `Sudoku.Infrastructure/Configuration/InfrastructureServiceCollectionExtensions.cs` (update)
- `Sudoku.AppHost/Program.cs` (update)
- Configuration files (e.g., `appsettings.json`) (update)

---

**Summary:**  
To use CosmosDb for persistence, implement a new repository and service layer, update DI, and configure CosmosDb as a resource in your Aspire AppHost. Update configuration files to provide the necessary connection details.
