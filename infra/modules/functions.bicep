// ---------------------------------------------------------------------------
// Azure Functions App — Flex Consumption (on-demand)
//
// Hosts the puzzle-pool background functions:
//   PuzzlePoolSeedFunction   — TimerTrigger, nightly top-up of the pool
//   PuzzleReplenishFunction  — EventGridTrigger, replenishes one puzzle each
//                              time a puzzle blob is deleted from the pool
//
// Hosting
//   Runs on its OWN Flex Consumption plan (FC1, Linux) rather than the shared
//   B1 App Service Plan. Flex Consumption supports the .NET 10 isolated runtime
//   (the Linux Dedicated plan does not), and on-demand billing keeps this very
//   low-volume workload (one daily timer + sporadic blob-delete events) inside
//   the monthly free grant — effectively $0/month.
//
// Cost note
//   scaleAndConcurrency.alwaysReady is intentionally EMPTY. Always-ready
//   instances bill even while idle ($0.000004/GB-s, no free grant). This
//   background workload tolerates cold starts, so we run purely on-demand.
//
// Runtime / deployment
//   For Flex Consumption the runtime stack and worker are declared in
//   functionAppConfig.runtime (NOT linuxFxVersion), and the deployment package
//   is delivered to a blob container (functionAppConfig.deployment.storage)
//   via OneDeploy — there is no Kudu/Oryx build, so SCM_DO_BUILD_DURING_*,
//   ENABLE_ORYX_BUILD, WEBSITE_RUN_FROM_PACKAGE and FUNCTIONS_EXTENSION_VERSION
//   are not used.
//
// Auth model
//   SystemAssigned Managed Identity for everything: host storage
//   (AzureWebJobsStorage), the deployment container, Key Vault, Cosmos and
//   puzzle blobs. The identity is granted the data-plane roles below.
// ---------------------------------------------------------------------------

param location string
param environment string

@description('Name of the Flex Consumption plan to create for the Function App.')
param functionPlanName string

param functionAppName string

@description('Name of the existing storage account reused for the Functions runtime, deployment package and puzzle blobs.')
param storageAccountName string

@description('Name of the existing Key Vault (for the Key Vault Secrets User role assignment).')
param keyVaultName string

@description('Name of the existing Cosmos DB account (for the SQL data-plane role assignment).')
param cosmosDbAccountName string

param appInsightsConnectionString string
param keyVaultUri string
param cosmosDbEndpoint string

@description('Memory allocated per Flex Consumption instance, in MB. 2048 or 4096.')
@allowed([
  2048
  4096
])
param instanceMemoryMB int = 2048

@description('Maximum number of instances the app may scale out to (40–1000).')
@minValue(40)
@maxValue(1000)
param maximumInstanceCount int = 40

var tags = {
  environment: environment
  project: 'XenobiaSoftSudoku'
  component: 'functions'
}

// Blob container that holds the deployed function package (OneDeploy target).
var deploymentContainerName = 'function-deployment'

// ---------------------------------------------------------------------------
// Reference shared / existing resources (read-only)
// ---------------------------------------------------------------------------

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' existing = {
  name: cosmosDbAccountName
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' existing = {
  parent: storageAccount
  name: 'default'
}

// Container for the deployment package.
resource deploymentContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: deploymentContainerName
  properties: {
    publicAccess: 'None'
  }
}

// ---------------------------------------------------------------------------
// Flex Consumption plan (FC1, Linux)
// ---------------------------------------------------------------------------

resource flexPlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: functionPlanName
  location: location
  tags: tags
  kind: 'functionapp'
  sku: {
    tier: 'FlexConsumption'
    name: 'FC1'
  }
  properties: {
    reserved: true // Linux
  }
}

// ---------------------------------------------------------------------------
// Function App (Flex Consumption)
// ---------------------------------------------------------------------------

resource functionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp,linux'
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: flexPlan.id
    httpsOnly: true
    keyVaultReferenceIdentity: 'SystemAssigned'
    functionAppConfig: {
      deployment: {
        storage: {
          type: 'blobContainer'
          value: '${storageAccount.properties.primaryEndpoints.blob}${deploymentContainerName}'
          authentication: {
            type: 'SystemAssignedIdentity'
          }
        }
      }
      runtime: {
        name: 'dotnet-isolated'
        version: '10.0'
      }
      scaleAndConcurrency: {
        // On-demand only — no always-ready instances, so the app bills nothing
        // while idle and stays within the monthly free grant for this workload.
        alwaysReady: []
        maximumInstanceCount: maximumInstanceCount
        instanceMemoryMB: instanceMemoryMB
      }
    }
    siteConfig: {
      minTlsVersion: '1.2'
      ftpsState: 'FtpsOnly'
    }
  }
}

resource functionAppSettings 'Microsoft.Web/sites/config@2024-04-01' = {
  parent: functionApp
  name: 'appsettings'
  properties: {
    // Host storage via managed identity (no account keys / connection strings).
    AzureWebJobsStorage__accountName: storageAccount.name

    // Observability
    APPLICATIONINSIGHTS_CONNECTION_STRING: appInsightsConnectionString

    // Application configuration (mirrors the API app)
    ConnectionStrings__AzureKeyVault: keyVaultUri
    CosmosDb__UseManagedIdentity: 'true'
    CosmosDb__AccountEndpoint: cosmosDbEndpoint
  }
}

// NOTE: The Event Grid subscription (BlobDeleted → PuzzleReplenishFunction) is
// still created in the deploy pipeline after the package is published, because
// the subscription's webhook target only resolves once the host has indexed
// the Event Grid-triggered function.

// ---------------------------------------------------------------------------
// RBAC — grant the Function App's managed identity the data-plane access it
// needs.
//
//   Storage Blob Data Owner        (b7e6dc6d-f1e8-4753-8033-0f276bb0955b)
//     Host storage (AzureWebJobsStorage), the deployment container, and
//     read/write of puzzle blobs in the sudoku-puzzles container. Owner (vs
//     Contributor) is used because the Flex host and timer-singleton leases
//     need full blob data-plane access.
//   Key Vault Secrets User         (4633458b-17de-408a-b874-0445c86b69e6)
//     Resolve Key Vault references / configuration secrets.
//   Cosmos DB Built-in Data Contributor (00000000-...-000000000002)
//     Cosmos data-plane access (SQL role assignment, not Azure RBAC).
// ---------------------------------------------------------------------------

var storageBlobDataOwnerRoleId = 'b7e6dc6d-f1e8-4753-8033-0f276bb0955b'
var keyVaultSecretsUserRoleId  = '4633458b-17de-408a-b874-0445c86b69e6'
var cosmosDataContributorRoleId = '00000000-0000-0000-0000-000000000002'

resource blobDataOwnerAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: storageAccount
  name: guid(storageAccount.id, functionApp.id, storageBlobDataOwnerRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageBlobDataOwnerRoleId)
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
