targetScope = 'resourceGroup'

// ---------------------------------------------------------------------------
// Global parameters
// ---------------------------------------------------------------------------

@description('Azure region for all resources.')
param location string = 'westus2'

@description('Deployment environment name used for tagging and naming (e.g. prod, staging).')
param environment string = 'prod'

// ---------------------------------------------------------------------------
// Compute parameters
// ---------------------------------------------------------------------------

@description('Name of the App Service Plan.')
param appServicePlanName string

@description('Name of the API app.')
param apiAppName string

@description('Name of the MCP server app.')
param mcpAppName string

@description('Name of the Azure Functions app (puzzle-pool background jobs).')
param functionAppName string

@description('Name of the Flex Consumption plan that hosts the Azure Functions app.')
param functionPlanName string

@description('App Service Plan SKU.')
param appServicePlanSku string = 'B1'

// ---------------------------------------------------------------------------
// Storage parameters
// ---------------------------------------------------------------------------

@description('Name of the Storage Account (3–24 lowercase alphanumeric).')
param storageAccountName string

@description('Name of the Cosmos DB account.')
param cosmosDbAccountName string

@description('Enable the Cosmos DB free tier. Only one account per subscription may use this.')
param cosmosDbEnableFreeTier bool = true

@description('Deploy the Cosmos DB account in Serverless capacity mode instead of manual provisioned throughput. Cannot be combined with free tier.')
param cosmosDbServerless bool = false

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
// Static Web App parameters
// ---------------------------------------------------------------------------

@description('Name of the Static Web App.')
param staticWebAppName string

@description('Custom domain name to bind to the SWA production environment (e.g. sudoku-beta.xenobiasoft.com).')
param swaCustomDomainName string = ''

@description('Whether to bind a custom domain to the SWA production environment. Requires DNS to be configured first.')
param enableSwaCustomDomain bool = false

// ---------------------------------------------------------------------------
// Monitoring parameters
// ---------------------------------------------------------------------------

@description('Name of the Log Analytics workspace.')
param logAnalyticsWorkspaceName string

@description('Name of the Application Insights component.')
param appInsightsName string

@description('Name of the Application Insights Smart Detection action group.')
param actionGroupName string

@description('Name of the failure anomalies smart detector alert rule.')
param alertRuleName string

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
    actionGroupName: actionGroupName
    alertRuleName: alertRuleName
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
    cosmosDbServerless: cosmosDbServerless
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
    apiAppName: apiAppName
    appServicePlanSku: appServicePlanSku
    swaCustomDomainName: swaCustomDomainName
    enableSwaCustomDomain: enableSwaCustomDomain
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    keyVaultUri: keyvault.outputs.keyVaultUri
  }
}

module mcp 'modules/mcp.bicep' = {
  name: 'mcp'
  params: {
    location: location
    environment: environment
    appServicePlanName: appServicePlanName
    mcpAppName: mcpAppName
    logAnalyticsWorkspaceCustomerId: monitoring.outputs.logAnalyticsWorkspaceCustomerId
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    keyVaultUri: keyvault.outputs.keyVaultUri
  }
}

module functions 'modules/functions.bicep' = {
  name: 'functions'
  params: {
    location: location
    environment: environment
    functionPlanName: functionPlanName
    functionAppName: functionAppName
    storageAccountName: storage.outputs.storageAccountName
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    keyVaultUri: keyvault.outputs.keyVaultUri
  }
}

module staticwebapp 'modules/staticwebapp.bicep' = {
  name: 'staticwebapp'
  params: {
    location: location
    environment: environment
    staticWebAppName: staticWebAppName
    customDomainName: swaCustomDomainName
    enableCustomDomain: enableSwaCustomDomain
    apiAppResourceId: compute.outputs.apiAppId
    apiAppName: apiAppName
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

output staticWebAppUrl string = staticwebapp.outputs.staticWebAppUrl
output resourceGroupName string = resourceGroup().name
output apiAppUrl string = compute.outputs.apiAppUrl
output mcpAppUrl string = mcp.outputs.mcpAppUrl
output functionAppUrl string = functions.outputs.functionAppUrl
output appInsightsConnectionString string = monitoring.outputs.appInsightsConnectionString
output cosmosDbEndpoint string = storage.outputs.cosmosDbEndpoint
output keyVaultUri string = keyvault.outputs.keyVaultUri
output appConfigEndpoint string = appconfig.outputs.appConfigEndpoint
