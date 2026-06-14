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
// RBAC NOTE: This app's managed identity needs Log Analytics Reader and
// Monitoring Reader (RG scope) to query Application Insights. Those grants are
// NOT declared here — all role assignments live in scripts/assign-roles.sh
// (see .claude/rules/rbac-role-assignments.md). Run that script after deploying.
// ---------------------------------------------------------------------------

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

output mcpAppUrl string = mcpApp.properties.defaultHostName
output mcpAppId string = mcpApp.id
output mcpAppPrincipalId string = mcpApp.identity.principalId
