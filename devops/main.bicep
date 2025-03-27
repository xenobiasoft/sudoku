param siteName string = 'XenobiaSoftSudoku'
param location string = resourceGroup().location
param appServicePlanSku string = 'B1' // Parameterize the SKU
param sslThumbprint string

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
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
    capacity: 3
  }
  kind: 'app,linux'
}

// Web App
resource sudokuApp 'Microsoft.Web/sites@2022-09-01' = {
  name: siteName
  location: location
  kind: 'app,linux'
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: 'sudoku.xenobiasoft.com'
        sslState: 'SniEnabled'
        thumbprint: sslThumbprint
        hostType: 'Standard'
      }
      {
        name: 'xenobiasoftsudoku.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: 'xenobiasoftsudoku.scm.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    serverFarmId: appServicePlan.id
  }
  tags: {
    environment: 'production'
    project: 'XenobiaSoftSudoku'
  }
}

// App Service Configuration
resource appServiceConfig 'Microsoft.Web/sites/config@2022-09-01' = {
  parent: sudokuApp
  name: 'web'
  properties: {
    defaultDocuments: [
      'Default.html'
      'index.html'      
    ]
    linuxFxVersion: 'DOTNETCORE:9.0'
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
  }
}

// Custom Domain Binding
resource customDomainBinding 'Microsoft.Web/sites/hostNameBindings@2024-04-01' = {
  parent: sudokuApp
  name: 'sudoku.xenobiasoft.com'
  properties: {
    siteName: siteName
    hostNameType: 'Verified'
    sslState: 'SniEnabled'
    thumbprint: sslThumbprint
  }
}

// Basic Binding
// resource basicBinding 'Microsoft.Web/sites/hostNameBindings@2024-04-01' = {
//   parent: sudokuApp
//   name: '${siteName}.azurewebsites.net'
//   properties: {
//     siteName: siteName
//     hostNameType: 'Verified'
//   }
// }