// ---------------------------------------------------------------------------
// MCP Server App Service
//
// Hosts the Model Context Protocol server that exposes Application Insights
// analytics tools to AI agents (e.g. Claude Desktop).
//
// Auth model
//   Outbound  — SystemAssigned Managed Identity → Azure Monitor / Log Analytics
//   Inbound   — Enable Easy Auth (Azure AD) via the portal or a separate Bicep
//               module once you have an Entra ID app registration.
// ---------------------------------------------------------------------------

param location string
param environment string
param appServicePlanName string
param mcpAppName string

@description('Log Analytics workspace ARM resource ID (for the role assignment scope).')
param logAnalyticsWorkspaceId string

@description('Log Analytics workspace GUID (customerId) — passed to the app as AppInsights__WorkspaceId.')
param logAnalyticsWorkspaceCustomerId string

param appInsightsConnectionString string
param keyVaultUri string

var tags = {
  environment: environment
  project: 'XenobiaSoftSudoku'
  component: 'mcp'
}

// ---------------------------------------------------------------------------
// Reference the shared App Service Plan (read-only)
// ---------------------------------------------------------------------------

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' existing = {
  name: appServicePlanName
}

// ---------------------------------------------------------------------------
// MCP App Service
// ---------------------------------------------------------------------------

resource mcpApp 'Microsoft.Web/sites@2023-12-01' = {
  name: mcpAppName
  location: location
  kind: 'app,linux'
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
      linuxFxVersion: 'DOTNETCORE|10.0'
      alwaysOn: true
      http20Enabled: true
      numberOfWorkers: 1
    }
  }
}

resource mcpAppConfig 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: mcpApp
  name: 'web'
  properties: {
    linuxFxVersion: 'DOTNETCORE|10.0'
    alwaysOn: true
    http20Enabled: true
    minTlsVersion: '1.2'
    scmMinTlsVersion: '1.2'
    ftpsState: 'FtpsOnly'
    healthCheckPath: '/health-check'
    managedPipelineMode: 'Integrated'
    loadBalancing: 'LeastRequests'
    virtualApplications: [
      {
        virtualPath: '/'
        physicalPath: 'site\\wwwroot'
        preloadEnabled: true
      }
    ]
    remoteDebuggingEnabled: false
    webSocketsEnabled: true // required for SSE keep-alive
    use32BitWorkerProcess: true
  }
}

resource mcpAppSettings 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: mcpApp
  name: 'appsettings'
  properties: {
    APPLICATIONINSIGHTS_CONNECTION_STRING: appInsightsConnectionString
    ApplicationInsightsAgent_EXTENSION_VERSION: '~3'
    ConnectionStrings__AzureKeyVault: keyVaultUri
    // Workspace GUID consumed by LogsQueryClient in ApplicationInsightsTools
    AppInsights__WorkspaceId: logAnalyticsWorkspaceCustomerId
  }
}

// ---------------------------------------------------------------------------
// RBAC — grant Managed Identity read access to Log Analytics
//
// Log Analytics Reader  (73c42c96-874c-492b-b04d-ab87d138a893)
//   Allows QueryWorkspace calls against the workspace.
//
// Monitoring Reader     (43d0d8ad-25c7-4714-9337-8ba259a9fe05)
//   Allows reading metrics from the Application Insights resource.
// ---------------------------------------------------------------------------

var logAnalyticsReaderRoleId = '73c42c96-874c-492b-b04d-ab87d138a893'
var monitoringReaderRoleId   = '43d0d8ad-25c7-4714-9337-8ba259a9fe05'

resource logAnalyticsReaderAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  // Scope to the workspace so the identity can only query this workspace
  scope: resourceGroup()
  name: guid(logAnalyticsWorkspaceId, mcpApp.id, logAnalyticsReaderRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', logAnalyticsReaderRoleId)
    principalId: mcpApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource monitoringReaderAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: resourceGroup()
  name: guid(logAnalyticsWorkspaceId, mcpApp.id, monitoringReaderRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', monitoringReaderRoleId)
    principalId: mcpApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

output mcpAppUrl string = mcpApp.properties.defaultHostName
output mcpAppId string = mcpApp.id
output mcpAppPrincipalId string = mcpApp.identity.principalId
