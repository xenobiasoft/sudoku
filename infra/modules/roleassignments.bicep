param webAppPrincipalId string
param apiAppPrincipalId string
param keyVaultName string
param cosmosDbAccountName string
param appConfigName string

// ---------------------------------------------------------------------------
// Role definition IDs (built-in)
// ---------------------------------------------------------------------------

var keyVaultSecretsUserRoleId = 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'
var appConfigDataReaderRoleId = '516239f1-63e1-4d78-a4de-a74fb236a071'

// Cosmos DB Built-in Data Contributor is a Cosmos DB SQL role, not a standard
// Azure RBAC role — its ID is a well-known constant.
var cosmosBuiltinDataContributorRoleId = '00000000-0000-0000-0000-000000000002'

// ---------------------------------------------------------------------------
// Existing resource references
// ---------------------------------------------------------------------------

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' existing = {
  name: cosmosDbAccountName
}

resource appConfigStore 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: appConfigName
}

// ---------------------------------------------------------------------------
// Key Vault — Secrets User for both apps
// ---------------------------------------------------------------------------

resource webAppKeyVaultRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(webAppPrincipalId, keyVaultSecretsUserRoleId, keyVault.id)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', keyVaultSecretsUserRoleId)
    principalId: webAppPrincipalId
    principalType: 'ServicePrincipal'
  }
}

resource apiAppKeyVaultRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(apiAppPrincipalId, keyVaultSecretsUserRoleId, keyVault.id)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', keyVaultSecretsUserRoleId)
    principalId: apiAppPrincipalId
    principalType: 'ServicePrincipal'
  }
}

// ---------------------------------------------------------------------------
// App Configuration — Data Reader for both apps
// ---------------------------------------------------------------------------

resource webAppConfigRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(webAppPrincipalId, appConfigDataReaderRoleId, appConfigStore.id)
  scope: appConfigStore
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', appConfigDataReaderRoleId)
    principalId: webAppPrincipalId
    principalType: 'ServicePrincipal'
  }
}

resource apiAppConfigRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(apiAppPrincipalId, appConfigDataReaderRoleId, appConfigStore.id)
  scope: appConfigStore
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', appConfigDataReaderRoleId)
    principalId: apiAppPrincipalId
    principalType: 'ServicePrincipal'
  }
}

// ---------------------------------------------------------------------------
// Cosmos DB — Built-in Data Contributor for API only
// (Blazor is a frontend and talks to the API via HTTP, not Cosmos DB directly)
// ---------------------------------------------------------------------------

resource apiCosmosRole 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2021-10-15' = {
  parent: cosmosDbAccount
  name: guid(apiAppPrincipalId, cosmosDbAccount.id, cosmosBuiltinDataContributorRoleId)
  properties: {
    roleDefinitionId: '${cosmosDbAccount.id}/sqlRoleDefinitions/${cosmosBuiltinDataContributorRoleId}'
    principalId: apiAppPrincipalId
    scope: '/'
  }
}
