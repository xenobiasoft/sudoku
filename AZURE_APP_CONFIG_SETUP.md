# Azure App Configuration Setup Guide

This document explains how to configure Azure App Configuration for the Sudoku application.

## Prerequisites

1. An Azure App Configuration resource created in your Azure subscription
2. The connection string for your Azure App Configuration resource
3. Proper permissions to read from the Azure App Configuration resource

## Connection String Configuration

### Option 1: Using AppHost appsettings.json (Recommended for Development)

Update your `Sudoku.AppHost/appsettings.json` file:

```json
{
  "ConnectionStrings": {
    "appconfig": "Endpoint=https://your-app-config-name.azconfig.io;Id=<your-id>;Secret=<your-secret>"
  }
}
```

### Option 2: Using Azure Key Vault (Recommended for Production)

Store the App Configuration connection string in Azure Key Vault with the key name:
`ConnectionStrings--appconfig`

### Option 3: Using User Secrets (Development only)

```bash
dotnet user-secrets set "ConnectionStrings:appconfig" "Endpoint=https://your-app-config-name.azconfig.io;Id=<your-id>;Secret=<your-secret>" --project Sudoku.AppHost
```

## Configuration Structure in Azure App Configuration

The following configuration keys should be created in your Azure App Configuration store:

### Core Sudoku Application Settings

```
Key: Sudoku:Game:DefaultDifficulty
Value: "Medium"
Label: <environment-name> (e.g., "Development", "Production")

Key: Sudoku:Game:MaxHintsPerGame
Value: 3
Label: <environment-name>

Key: Sudoku:Game:AutoSaveIntervalSeconds
Value: 30
Label: <environment-name>

Key: Sudoku:Game:EnableStatistics
Value: true
Label: <environment-name>

Key: Sudoku:UI:DefaultTheme
Value: "Light"
Label: <environment-name>

Key: Sudoku:UI:ShowTimer
Value: true
Label: <environment-name>

Key: Sudoku:UI:EnableAnimations
Value: true
Label: <environment-name>

Key: Sudoku:UI:CellSizePixels
Value: 40
Label: <environment-name>

Key: Sudoku:Performance:GameStateCacheTimeoutSeconds
Value: 300
Label: <environment-name>

Key: Sudoku:Performance:MaxConcurrentGamesPerUser
Value: 5
Label: <environment-name>

Key: Sudoku:Performance:EnableResponseCompression
Value: true
Label: <environment-name>
```

### Connection Strings (if not stored in Key Vault)

```
Key: ConnectionStrings:CosmosDb
Value: <your-cosmos-db-connection-string>
Label: <environment-name>

Key: ConnectionStrings:AzureStorage
Value: <your-azure-storage-connection-string>
Label: <environment-name>
```

### Application Insights Settings

```
Key: AppInsightsConnectionString
Value: <your-application-insights-connection-string>
Label: <environment-name>
```

## Feature Flags

You can also configure feature flags in Azure App Configuration:

### Example Feature Flags

```
Feature Flag: EnableNewGameUI
Description: Enable the new game user interface
Enabled: true (for specific environments/users)

Feature Flag: EnableAdvancedStatistics
Description: Enable advanced game statistics tracking
Enabled: false

Feature Flag: EnableMultiplayer
Description: Enable multiplayer game mode
Enabled: false
```

## Environment-Specific Configuration

The application is configured to use environment-specific labels. For example:
- `Development` label for development environment
- `Staging` label for staging environment  
- `Production` label for production environment

## Configuration Refresh

The application is configured to automatically refresh configuration values every 5 minutes (300 seconds) by default. You can adjust this in the `AzureAppConfiguration:RefreshInterval` setting.

## Demo and Testing

### Blazor Configuration Demo Page

Visit `/config-demo` in the Blazor web application to see a live demonstration of Azure App Configuration values. This page shows:

- Current game, UI, and performance settings
- Connection string status
- Real-time configuration values
- Instructions for setup

### API Configuration Endpoints

The API includes several endpoints for testing and debugging configuration:

- `GET /api/configuration/sudoku-settings` - Returns all Sudoku configuration settings
- `GET /api/configuration/value/{key}` - Returns a specific configuration value
- `GET /api/configuration/connections` - Shows connection string status
- `GET /api/configuration/refresh-test` - Test endpoint for configuration refresh

### Testing Configuration Updates

1. Update values in your Azure App Configuration store
2. Visit the demo page or call the API endpoints
3. Configuration will refresh automatically (may take up to 5 minutes)
4. For immediate testing, restart the application

## Security Considerations

1. **Use Managed Identity in Production**: Configure your Azure resources to use Managed Identity instead of connection strings
2. **Store Secrets in Key Vault**: Keep sensitive configuration like connection strings in Azure Key Vault
3. **Use Labels for Environment Isolation**: Use different labels for different environments
4. **Configure Access Policies**: Limit access to configuration values based on the principle of least privilege

## Example Usage in Code

### Dependency Injection

```csharp
// Inject the configured options in your services
public class GameService
{
    private readonly SudokuOptions _options;

    public GameService(IOptions<SudokuOptions> options)
    {
        _options = options.Value;
    }

    public void CreateGame()
    {
        var difficulty = _options.Game.DefaultDifficulty;
        var maxHints = _options.Game.MaxHintsPerGame;
        // ... use configuration values
    }
}
```

### Blazor Components

```razor
@inject IOptions<SudokuOptions> SudokuOptions

<div>
    <p>Default Difficulty: @SudokuOptions.Value.Game.DefaultDifficulty</p>
    <p>Show Timer: @SudokuOptions.Value.UI.ShowTimer</p>
</div>
```

### API Controllers

```csharp
[ApiController]
public class GameController : ControllerBase
{
    private readonly SudokuOptions _options;

    public GameController(IOptions<SudokuOptions> options)
    {
        _options = options.Value;
    }

    [HttpGet]
    public ActionResult GetGameSettings()
    {
        return Ok(_options);
    }
}
```

## Monitoring and Troubleshooting

1. Check the application logs for Azure App Configuration connection issues
2. Verify that the connection string is correctly configured
3. Ensure that the Azure App Configuration resource allows access from your application's IP address or Azure resources
4. Monitor configuration refresh operations in Azure App Configuration's monitoring blade
5. Use the demo pages and API endpoints to test configuration retrieval
6. Check the `/config-demo` page for real-time configuration status

## Advanced Configuration

### Custom Refresh Intervals

Update your `appsettings.json` in the AppHost:

```json
{
  "AzureAppConfiguration": {
    "RefreshInterval": 120,
    "FeatureFlags": {
      "Enabled": true,
      "RefreshInterval": 60
    }
  }
}
```

### Environment-Specific Labels

The application automatically uses the environment name as a label filter. To override:

```json
{
  "AzureAppConfiguration": {
    "LabelFilter": "MyCustomLabel"
  }
}