targetScope = 'resourceGroup'

// ---------------------------------------------------------------------------
// Global parameters
// ---------------------------------------------------------------------------

@description('Azure region for all resources.')
param location string = 'westus2'

@description('Deployment environment name used for tagging.')
param environment string = 'production'

// ---------------------------------------------------------------------------
// Compute parameters
// ---------------------------------------------------------------------------

@description('Name of the App Service Plan.')
param appServicePlanName string

@description('Name of the Blazor web app.')
param webAppName string

@description('Name of the API app.')
param apiAppName string

@description('App Service Plan SKU.')
param appServicePlanSku string = 'B1'

@description('Custom domain name bound to the web app.')
param customDomainName string = 'sudoku.xenobiasoft.com'

// ---------------------------------------------------------------------------
// Storage parameters
// ---------------------------------------------------------------------------

@description('Name of the Storage Account (3–24 lowercase alphanumeric).')
param storageAccountName string

@description('Name of the Cosmos DB account.')
param cosmosDbAccountName string

@description('Enable the Cosmos DB free tier. Only one account per subscription may use this.')
param cosmosDbEnableFreeTier bool = true

// ---------------------------------------------------------------------------
// Key Vault parameters
// ---------------------------------------------------------------------------

@description('Name of the Key Vault.')
param keyVaultName string

// ---------------------------------------------------------------------------
// App Configuration parameters
// ---------------------------------------------------------------------------

@description('Name of the App Configuration store.')
param appConfigName string

// ---------------------------------------------------------------------------
// Monitoring parameters
// ---------------------------------------------------------------------------

@description('Name of the Log Analytics workspace.')
param logAnalyticsWorkspaceName string

@description('Name of the Application Insights component.')
param appInsightsName string

// ---------------------------------------------------------------------------
// Modules
// ---------------------------------------------------------------------------

module monitoring 'modules/monitoring.bicep' = {
  name: 'monitoring'
  params: {
    location: location
    environment: environment
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    appInsightsName: appInsightsName
  }
}

module storage 'modules/storage.bicep' = {
  name: 'storage'
  params: {
    location: location
    environment: environment
    storageAccountName: storageAccountName
    cosmosDbAccountName: cosmosDbAccountName
    cosmosDbEnableFreeTier: cosmosDbEnableFreeTier
  }
}

module keyvault 'modules/keyvault.bicep' = {
  name: 'keyvault'
  params: {
    location: location
    environment: environment
    keyVaultName: keyVaultName
  }
}

module appconfig 'modules/appconfig.bicep' = {
  name: 'appconfig'
  params: {
    location: location
    environment: environment
    appConfigName: appConfigName
  }
}

module compute 'modules/compute.bicep' = {
  name: 'compute'
  params: {
    location: location
    environment: environment
    appServicePlanName: appServicePlanName
    webAppName: webAppName
    apiAppName: apiAppName
    appServicePlanSku: appServicePlanSku
    customDomainName: customDomainName
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

output resourceGroupName string = resourceGroup().name
output webAppUrl string = compute.outputs.webAppUrl
output apiAppUrl string = compute.outputs.apiAppUrl
output appInsightsConnectionString string = monitoring.outputs.appInsightsConnectionString
output cosmosDbEndpoint string = storage.outputs.cosmosDbEndpoint
output keyVaultUri string = keyvault.outputs.keyVaultUri
output appConfigEndpoint string = appconfig.outputs.appConfigEndpoint
