# Azure App Configuration Setup Guide

This API is configured to use Azure App Configuration with **Managed Identity** for secure, secret-free configuration management.

## Production Setup (Managed Identity - Recommended)

### 1. Create Azure App Configuration Resource
```bash
# Create resource group (if not exists)
az group create --name <resource-group-name> --location <location>

# Create App Configuration
az appconfig create --name <your-appconfig-name> --resource-group <resource-group-name> --location <location> --sku Standard
```

### 2. Enable Managed Identity on Azure App Service
```bash
# Enable system-assigned managed identity
az webapp identity assign --name <your-app-service-name> --resource-group <resource-group-name>
```

### 3. Grant App Configuration Access to Managed Identity
```bash
# Get the App Service's managed identity principal ID
$principalId = az webapp identity show --name <your-app-service-name> --resource-group <resource-group-name> --query principalId -o tsv

# Get the App Configuration resource ID
$appConfigId = az appconfig show --name <your-appconfig-name> --resource-group <resource-group-name> --query id -o tsv

# Assign the "App Configuration Data Reader" role
az role assignment create --assignee $principalId --role "App Configuration Data Reader" --scope $appConfigId
```

### 4. Update Production Configuration
In your Azure App Service, set the application setting or update `appsettings.Production.json`:

```json
{
    "appconfig": {
        "Endpoint": "https://<your-appconfig-name>.azconfig.io",
        "ManagedIdentityEnabled": true
    }
}
```

**Note:** Replace `<your-appconfig-name>` with your actual Azure App Configuration name.

## Local Development Setup

### Option 1: Use User Secrets (Recommended for Local Dev)
```bash
# Initialize user secrets
dotnet user-secrets init --project Sudoku.Api

# Set the connection string
dotnet user-secrets set "ConnectionStrings:appconfig" "Endpoint=https://<your-appconfig-name>.azconfig.io;Id=xxx;Secret=xxx" --project Sudoku.Api
```

### Option 2: Store Connection String in Azure Key Vault
Add a secret in your Key Vault with name: `ConnectionStrings--appconfig` (note the double dash)

The app will automatically load it since Key Vault is already configured.

### Option 3: Skip App Configuration Locally
If you don't provide any connection string or endpoint, the app will skip Azure App Configuration and use only local `appsettings.json` files.

## Configuration Structure in Azure App Configuration

### Keys to Add
Add your configuration keys in Azure App Configuration with appropriate labels:

- **Label: Development** - for local/dev environment
- **Label: Production** - for production environment

Example keys:
```
UseCosmosDb
CosmosDb:DatabaseName
CosmosDb:ContainerName
Cors:AllowedOrigins:0
Cors:AllowedOrigins:1
```

### Feature Flags (Optional)
If you enable feature flags, add them in the Feature Management section of Azure App Configuration.

## Environment-Specific Configuration

- **Development**: Uses label "Development" from App Configuration
- **Production**: Uses label "Production" from App Configuration
- **Refresh Interval**: 30 seconds (configurable in appsettings)

## Verification

After deployment, check the logs to verify App Configuration is connected:
- Look for: "Azure App Configuration successfully configured"
- If skipped, you'll see: "Azure App Configuration skipped - no connection string or endpoint provided"

## Troubleshooting

### Error: Failed to connect to App Configuration
**Solution:** Verify:
1. Managed Identity is enabled on the App Service
2. The identity has "App Configuration Data Reader" role
3. The endpoint URL is correct in configuration

### Configuration not refreshing
**Solution:** 
- Verify `RefreshInterval` is set in appsettings
- Check that the keys are registered for refresh in the extension code

### Local development not connecting
**Solution:**
- Use user secrets or Key Vault for the connection string
- Or simply don't configure it and use local appsettings only

## Security Best Practices

? **DO:**
- Use Managed Identity in production
- Store connection strings in Key Vault or user secrets for development
- Use environment-specific labels (Development, Production)
- Regularly rotate access keys if using connection strings

? **DON'T:**
- Commit connection strings to source control
- Use the same label for all environments
- Store secrets in appsettings.json files

## Additional Resources

- [Azure App Configuration Documentation](https://docs.microsoft.com/azure/azure-app-configuration/)
- [Managed Identity Documentation](https://docs.microsoft.com/azure/active-directory/managed-identities-azure-resources/)
- [Azure CLI App Configuration Commands](https://docs.microsoft.com/cli/azure/appconfig)
