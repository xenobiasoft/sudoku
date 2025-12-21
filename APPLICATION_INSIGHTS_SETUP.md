# Application Insights Logging Configuration Guide

## Changes Made

I've fixed several issues with your Application Insights configuration:

### 1. Fixed Program.cs
- **Removed `ClearProviders()`**: This was preventing Application Insights logger from being registered
- **Improved connection string handling**: Now checks both `APPLICATIONINSIGHTS_CONNECTION_STRING` (standard) and `AppInsightsConnectionString`
- **Conditional configuration**: Only configures Application Insights if connection string is provided
- **Better logging filters**: More specific filters to reduce noise while capturing your custom logs

### 2. Updated appsettings.json
- **Added Application Insights logging section**: Specific log level configuration for Application Insights

### 3. Enabled Azure Monitor in ServiceDefaults
- **Uncommented and configured UseAzureMonitor()**: This sends OpenTelemetry data to Application Insights

## Required Configuration

### Add Application Insights Connection String

You need to add your Application Insights connection string. Choose one of these methods:

#### Option 1: Environment Variable (Recommended for Production)
Set the environment variable:
```
APPLICATIONINSIGHTS_CONNECTION_STRING=InstrumentationKey=xxxxx;IngestionEndpoint=https://...
```

#### Option 2: User Secrets (Recommended for Development)
```bash
dotnet user-secrets set "APPLICATIONINSIGHTS_CONNECTION_STRING" "InstrumentationKey=xxxxx;IngestionEndpoint=https://..." --project Sudoku.Web.Server
```

#### Option 3: appsettings.Development.json (Not Recommended - for testing only)
```json
{
  "APPLICATIONINSIGHTS_CONNECTION_STRING": "InstrumentationKey=xxxxx;IngestionEndpoint=https://...",
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### Option 4: Azure Key Vault (Best for Production)
Store in your Key Vault as `APPLICATIONINSIGHTS-CONNECTION-STRING` and it will be automatically loaded.

## Required NuGet Packages

Make sure you have these packages installed:

### Sudoku.Web.Server
```xml
<PackageReference Include="Microsoft.ApplicationInsights.AspNetCore" Version="2.22.0" />
```

### Sudoku.ServiceDefaults
```xml
<PackageReference Include="Azure.Monitor.OpenTelemetry.AspNetCore" Version="1.2.0" />
```

## Verification Steps

1. **Run your application**
2. **Check console output** - You should see Application Insights initialization messages
3. **Wait 2-5 minutes** - Application Insights has a delay before data appears
4. **Check Azure Portal**:
   - Go to your Application Insights resource
   - Navigate to "Logs"
   - Run this query:
   ```kusto
   traces
   | where timestamp > ago(1h)
   | order by timestamp desc
   | take 50
   ```

## What Gets Logged

With these changes, you'll see:
- ? All your custom `_logger.LogInformation()` calls from GameApiClient
- ? All your custom `_logger.LogWarning()` calls
- ? All your custom `_logger.LogError()` calls
- ? OpenTelemetry traces and metrics
- ? HTTP request telemetry
- ? Dependency telemetry (outgoing HTTP calls)
- ? Microsoft.AspNetCore internal logs (filtered to Warning and above to reduce noise)

## Troubleshooting

### If logs still don't appear:

1. **Check connection string format**:
   ```
   InstrumentationKey=xxx-xxx-xxx;IngestionEndpoint=https://xxx.applicationinsights.azure.com/;LiveEndpoint=https://xxx.livediagnostics.monitor.azure.com/
   ```

2. **Enable debug logging temporarily**:
   ```json
   "Logging": {
     "LogLevel": {
       "Default": "Debug",
       "Microsoft.ApplicationInsights": "Debug"
     }
   }
   ```

3. **Verify Application Insights is initialized**:
   Check console output for: `"Application Insights Telemetry is collected successfully"`

4. **Test with a simple log**:
   Add this to a controller/page:
   ```csharp
   _logger.LogInformation("TEST LOG MESSAGE - Application Insights Integration");
   ```

5. **Check sampling**:
   The configuration now has `EnableAdaptiveSampling = false` to ensure all logs are captured during testing.

## Performance Considerations

For production:
- Re-enable adaptive sampling: `EnableAdaptiveSampling = true`
- Adjust log levels in appsettings.Production.json to reduce volume
- Consider using log level Warning or Error for high-traffic applications

## Alternative: If You Only Want Classic Application Insights (Without OpenTelemetry)

If you don't want OpenTelemetry integration, you can:
1. Remove the `UseAzureMonitor()` call from ServiceDefaults
2. Keep only the Application Insights logging configuration in Program.cs
3. This will still send logs but won't include OpenTelemetry traces
