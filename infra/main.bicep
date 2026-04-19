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

@description('Name of the Blazor web app.')
param webAppName string

@description('Name of the API app.')
param apiAppName string

@description('App Service Plan SKU.')
param appServicePlanSku string = 'B1'

@description('Custom domain name bound to the web app.')
param customDomainName string = 'sudoku.xenobiasoft.com'

@description('Whether to bind a custom domain and SSL certificate to the web app. Set to false for non-prod environments.')
param enableCustomDomain bool = false

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
    enableCustomDomain: enableCustomDomain
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    keyVaultUri: keyvault.outputs.keyVaultUri
    cosmosDbEndpoint: storage.outputs.cosmosDbEndpoint
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
  }
}

// Step 1: Bind the custom hostname to the web app (no SSL yet).
// The managed certificate cannot be created until the hostname is bound.
module hostname 'modules/hostname.bicep' = if (enableCustomDomain) {
  name: 'hostname'
  dependsOn: [compute]
  params: {
    webAppName: webAppName
    customDomainName: customDomainName
  }
}

// Step 2: Create the managed certificate and enable SNI SSL on the binding.
module ssl 'modules/ssl.bicep' = if (enableCustomDomain) {
  name: 'ssl'
  dependsOn: [hostname]
  params: {
    location: location
    webAppName: webAppName
    appServicePlanName: appServicePlanName
    customDomainName: customDomainName
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

output staticWebAppUrl string = staticwebapp.outputs.staticWebAppUrl
output resourceGroupName string = resourceGroup().name
output webAppUrl string = compute.outputs.webAppUrl
output apiAppUrl string = compute.outputs.apiAppUrl
output appInsightsConnectionString string = monitoring.outputs.appInsightsConnectionString
output cosmosDbEndpoint string = storage.outputs.cosmosDbEndpoint
output keyVaultUri string = keyvault.outputs.keyVaultUri
output appConfigEndpoint string = appconfig.outputs.appConfigEndpoint
