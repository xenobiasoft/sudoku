# Quick Setup Commands for Azure App Configuration

## Prerequisites
Replace these placeholders with your actual values:
- `<resource-group>`: Your Azure resource group name
- `<app-service>`: Your Azure App Service name  
- `<appconfig-name>`: Your App Configuration resource name (must be globally unique)
- `<location>`: Azure region (e.g., eastus, westus2, westeurope)

## 1. Create App Configuration
```powershell
az appconfig create `
  --name <appconfig-name> `
  --resource-group <resource-group> `
  --location <location> `
  --sku Standard
```

## 2. Enable Managed Identity on App Service
```powershell
az webapp identity assign `
  --name <app-service> `
  --resource-group <resource-group>
```

## 3. Grant Access
```powershell
# Get the principal ID
$principalId = az webapp identity show `
  --name <app-service> `
  --resource-group <resource-group> `
  --query principalId -o tsv

# Get App Config ID
$appConfigId = az appconfig show `
  --name <appconfig-name> `
  --resource-group <resource-group> `
  --query id -o tsv

# Assign role
az role assignment create `
  --assignee $principalId `
  --role "App Configuration Data Reader" `
  --scope $appConfigId
```

## 4. Get the Endpoint URL
```powershell
az appconfig show `
  --name <appconfig-name> `
  --resource-group <resource-group> `
  --query endpoint -o tsv
```

## 5. Update App Service Configuration
```powershell
az webapp config appsettings set `
  --name <app-service> `
  --resource-group <resource-group> `
  --settings appconfig__Endpoint="https://<appconfig-name>.azconfig.io" appconfig__ManagedIdentityEnabled="true"
```

## 6. Add Configuration Values to App Configuration
```powershell
# Example: Add a configuration key-value
az appconfig kv set `
  --name <appconfig-name> `
  --key "UseCosmosDb" `
  --value "true" `
  --label "Production" `
  --yes

# Add another key
az appconfig kv set `
  --name <appconfig-name> `
  --key "CosmosDb:DatabaseName" `
  --value "sudoku" `
  --label "Production" `
  --yes
```

## Verification
```powershell
# List all configurations
az appconfig kv list `
  --name <appconfig-name> `
  --label "Production"

# Check App Service managed identity
az webapp identity show `
  --name <app-service> `
  --resource-group <resource-group>

# Check role assignments
az role assignment list `
  --assignee $principalId `
  --scope $appConfigId
```

## Local Development (User Secrets)
```powershell
# Set connection string for local development
dotnet user-secrets set "ConnectionStrings:appconfig" "Endpoint=https://<appconfig-name>.azconfig.io;Id=xxx;Secret=xxx" --project Sudoku.Api
```

To get the connection string from Azure:
```powershell
az appconfig credential list `
  --name <appconfig-name> `
  --resource-group <resource-group>
```
