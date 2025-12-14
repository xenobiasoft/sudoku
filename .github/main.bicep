param siteName string = 'XenobiaSoftSudoku'
param location string = resourceGroup().location
param appServicePlanSku string = 'B1'
param customDomain string = 'sudoku.xenobiasoft.com'
param enableCustomDomainSsl bool = true

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: 'XenobiaSoftServicePlan'
  location: location
  properties: {
    reserved: true
  }
  sku: {
    name: appServicePlanSku
    tier: 'Basic'
    size: appServicePlanSku
    family: 'B'
    capacity: 1
  }
  kind: 'app,linux'
}

// Web App
resource sudokuApp 'Microsoft.Web/sites@2023-12-01' = {
  name: siteName
  location: location
  kind: 'app,linux'
  properties: {
    enabled: true
    serverFarmId: appServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: false
  }
  tags: {
    environment: 'production'
    project: 'XenobiaSoftSudoku'
  }
}

// App Service Configuration
resource appServiceConfig 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: sudokuApp
  name: 'web'
  properties: {
    defaultDocuments: [
      'Default.html'
      'index.html'
    ]
    linuxFxVersion: 'DOTNETCORE|10.0'
    managedPipelineMode: 'Integrated'
    virtualApplications: [
      {
        virtualPath: '/'
        physicalPath: 'site\\wwwroot'
        preloadEnabled: false
      }
    ]
    http20Enabled: true
    minTlsVersion: '1.2'
    healthCheckPath: '/health-check'
    minimumElasticInstanceCount: 1
    ftpsState: 'FtpsOnly'
    alwaysOn: true
  }
}

// Reference existing Azure Managed Certificate (only if SSL is enabled)
resource managedCert 'Microsoft.Web/certificates@2023-12-01' existing = if (enableCustomDomainSsl) {
  name: customDomain
}

// Custom Domain Binding with Managed Certificate (only if SSL is enabled)
resource customDomainBinding 'Microsoft.Web/sites/hostNameBindings@2023-12-01' = if (enableCustomDomainSsl) {
  parent: sudokuApp
  name: customDomain
  properties: {
    siteName: siteName
    hostNameType: 'Verified'
    sslState: 'SniEnabled'
    thumbprint: managedCert.properties.thumbprint
  }
}
