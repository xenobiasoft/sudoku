param location string
param environment string
param appServicePlanName string
param webAppName string
param apiAppName string
param appServicePlanSku string = 'B1'
param customDomainName string = 'sudoku.xenobiasoft.com'
param enableCustomDomain bool = false

var corsAllowedOrigins = enableCustomDomain ? [
  'https://${customDomainName}'
  'https://${webAppName}.azurewebsites.net'
] : [
  'https://${webAppName}.azurewebsites.net'
]

var tags = {
  environment: environment
  project: 'XenobiaSoftSudoku'
}

var apiTags = union(tags, {
  component: 'api'
})

// ---------------------------------------------------------------------------
// App Service Plan
// ---------------------------------------------------------------------------

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  kind: 'linux'
  sku: {
    name: appServicePlanSku
    tier: 'Basic'
    size: appServicePlanSku
    family: 'B'
    capacity: 1
  }
  properties: {
    reserved: true
    elasticScaleEnabled: false
    isSpot: false
    zoneRedundant: false
    perSiteScaling: false
  }
}

// ---------------------------------------------------------------------------
// Web App (Blazor)
// ---------------------------------------------------------------------------

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: webAppName
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

resource webAppConfig 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: webApp
  name: 'web'
  properties: {
    linuxFxVersion: 'DOTNETCORE|10.0'
    appCommandLine: 'dotnet Sudoku.Blazor.dll'
    alwaysOn: true
    http20Enabled: true
    minTlsVersion: '1.2'
    scmMinTlsVersion: '1.2'
    ftpsState: 'FtpsOnly'
    healthCheckPath: '/health-check'
    defaultDocuments: [
      'Default.html'
      'index.html'
    ]
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
    webSocketsEnabled: false
    use32BitWorkerProcess: true
  }
}

// ---------------------------------------------------------------------------
// API App
// ---------------------------------------------------------------------------

resource apiApp 'Microsoft.Web/sites@2023-12-01' = {
  name: apiAppName
  location: location
  kind: 'app,linux'
  tags: apiTags
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

resource apiAppConfig 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: apiApp
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
    cors: {
      allowedOrigins: corsAllowedOrigins
      supportCredentials: false
    }
    remoteDebuggingEnabled: false
    webSocketsEnabled: false
    use32BitWorkerProcess: true
  }
}

// ---------------------------------------------------------------------------
// SSL Certificate (App Service Managed Certificate)
// Note: The custom domain DNS must point to the web app before this resource
// can be successfully provisioned.
// ---------------------------------------------------------------------------

resource certificate 'Microsoft.Web/certificates@2023-12-01' = if (enableCustomDomain) {
  name: '${customDomainName}-${webAppName}'
  location: location
  properties: {
    canonicalName: customDomainName
    hostNames: [
      customDomainName
    ]
    serverFarmId: appServicePlan.id
  }
}

// ---------------------------------------------------------------------------
// Custom Domain Binding
// ---------------------------------------------------------------------------

resource customDomainBinding 'Microsoft.Web/sites/hostNameBindings@2023-12-01' = if (enableCustomDomain) {
  parent: webApp
  name: customDomainName
  properties: {
    siteName: webAppName
    hostNameType: 'Verified'
    sslState: 'SniEnabled'
    thumbprint: certificate.properties.thumbprint
  }
}

output webAppUrl string = webApp.properties.defaultHostName
output apiAppUrl string = apiApp.properties.defaultHostName
output webAppPrincipalId string = webApp.identity.principalId
output apiAppPrincipalId string = apiApp.identity.principalId
