// ---------------------------------------------------------------------------
// Azure Functions App
//
// Hosts the puzzle-pool background functions:
//   PuzzlePoolSeedFunction   — TimerTrigger, nightly top-up of the pool
//   PuzzleReplenishFunction  — EventGridTrigger, replenishes one puzzle each
//                              time a puzzle blob is deleted from the pool
//
// Runs on the shared App Service Plan (B1, Linux) alongside the API and MCP
// apps. Reuses the existing storage account for the Functions runtime
// (AzureWebJobsStorage).
//
// Auth model
//   Outbound — SystemAssigned Managed Identity → Key Vault / Cosmos / Blob.
//   The identity must be granted the same data-plane roles as the API app
//   (Key Vault Secrets User, Storage Blob Data Contributor, and the Cosmos
//   SQL data role). Those grants are managed outside this template, matching
//   the existing API setup.
// ---------------------------------------------------------------------------

param location string
param environment string
param appServicePlanName string
param functionAppName string

@description('Name of the existing storage account reused for the Functions runtime and puzzle blobs.')
param storageAccountName string

@description('Name of the existing Key Vault (for the Key Vault Secrets User role assignment).')
param keyVaultName string

@description('Name of the existing Cosmos DB account (for the SQL data-plane role assignment).')
param cosmosDbAccountName string

param appInsightsConnectionString string
param keyVaultUri string
param cosmosDbEndpoint string

var tags = {
  environment: environment
  project: 'XenobiaSoftSudoku'
  component: 'functions'
}

// ---------------------------------------------------------------------------
// Reference shared / existing resources (read-only)
// ---------------------------------------------------------------------------

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' existing = {
  name: appServicePlanName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' existing = {
  name: cosmosDbAccountName
}

var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${az.environment().suffixes.storage}'

// ---------------------------------------------------------------------------
// Function App
// ---------------------------------------------------------------------------

resource functionApp 'Microsoft.Web/sites@2023-12-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp,linux'
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: false
    reserved: true
    keyVaultReferenceIdentity: 'SystemAssigned'
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|10.0'
      alwaysOn: true
      http20Enabled: true
      minTlsVersion: '1.2'
      scmMinTlsVersion: '1.2'
      ftpsState: 'FtpsOnly'
      numberOfWorkers: 1
      use32BitWorkerProcess: false
    }
  }
}

resource functionAppSettings 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: functionApp
  name: 'appsettings'
  properties: {
    // Functions runtime
    FUNCTIONS_EXTENSION_VERSION: '~4'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet-isolated'
    AzureWebJobsStorage: storageConnectionString

    // Deploy the pre-built publish artifact via zip-deploy to a writable
    // wwwroot. WEBSITE_RUN_FROM_PACKAGE=1 must NOT be set: it makes wwwroot a
    // read-only mount, so the Linux zip-deploy can't write the package and the
    // host indexes zero functions. Disable Oryx/remote build since the artifact
    // is already published.
    SCM_DO_BUILD_DURING_DEPLOYMENT: 'false'
    ENABLE_ORYX_BUILD: 'false'

    // Observability
    APPLICATIONINSIGHTS_CONNECTION_STRING: appInsightsConnectionString

    // Application configuration (mirrors the API app)
    ConnectionStrings__AzureKeyVault: keyVaultUri
    CosmosDb__UseManagedIdentity: 'true'
    CosmosDb__AccountEndpoint: cosmosDbEndpoint
  }
}

// NOTE: The Event Grid subscription (BlobDeleted → PuzzleReplenishFunction) is
// NOT created here. Event Grid validates that the target function exists when
// the subscription is created, but infra is deployed before the function code,
// so on a fresh Function App the function doesn't exist yet and validation
// fails with NotFound. The subscription is instead created in the deploy-apps
// pipeline step, after the function package is published.

// ---------------------------------------------------------------------------
// RBAC — grant the Function App's managed identity the same data-plane access
// the API app uses.
//
//   Storage Blob Data Contributor  (ba92f5b4-2d11-453d-a403-e96b0029c9fe)
//     Read/write puzzle blobs in the sudoku-puzzles container.
//   Key Vault Secrets User         (4633458b-17de-408a-b874-0445c86b69e6)
//     Resolve Key Vault references / configuration secrets.
//   Cosmos DB Built-in Data Contributor (00000000-...-000000000002)
//     Cosmos data-plane access (SQL role assignment, not Azure RBAC).
// ---------------------------------------------------------------------------

var storageBlobDataContributorRoleId = 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
var keyVaultSecretsUserRoleId        = '4633458b-17de-408a-b874-0445c86b69e6'
var cosmosDataContributorRoleId      = '00000000-0000-0000-0000-000000000002'

resource blobDataContributorAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: storageAccount
  name: guid(storageAccount.id, functionApp.id, storageBlobDataContributorRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageBlobDataContributorRoleId)
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource keyVaultSecretsUserAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyVault
  name: guid(keyVault.id, functionApp.id, keyVaultSecretsUserRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', keyVaultSecretsUserRoleId)
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource cosmosDataContributorAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2024-05-15' = {
  parent: cosmosDbAccount
  name: guid(cosmosDbAccount.id, functionApp.id, cosmosDataContributorRoleId)
  properties: {
    roleDefinitionId: '${cosmosDbAccount.id}/sqlRoleDefinitions/${cosmosDataContributorRoleId}'
    principalId: functionApp.identity.principalId
    scope: cosmosDbAccount.id
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

output functionAppName string = functionApp.name
output functionAppUrl string = functionApp.properties.defaultHostName
output functionAppPrincipalId string = functionApp.identity.principalId
