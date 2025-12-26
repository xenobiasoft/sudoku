param siteName string = 'XenobiaSoftSudoku'
param apiSiteName string = 'XenobiasoftSudokuApi'
param location string = resourceGroup().location
param appServicePlanSku string = 'B1'

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
    hostNameSslStates: [
      {
        name: 'sudoku.xenobiasoft.com'
        sslState: 'SniEnabled'
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
    httpsOnly: true
    clientAffinityEnabled: false
  }
  tags: {
    environment: 'production'
    project: 'XenobiaSoftSudoku'
  }
}

// API App Service
resource sudokuApiApp 'Microsoft.Web/sites@2023-12-01' = {
  name: apiSiteName
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
    component: 'api'
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
    ftpsState: 'FtpsOnly'
    alwaysOn: true
  }
}

// API App Service Configuration
resource apiServiceConfig 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: sudokuApiApp
  name: 'web'
  properties: {
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
    healthCheckPath: '/health'
    ftpsState: 'FtpsOnly'
    alwaysOn: true
    cors: {
      allowedOrigins: [
        'https://sudoku.xenobiasoft.com'
        'https://${siteName}.azurewebsites.net'
      ]
      supportCredentials: false
    }
  }
}

// Reference existing Azure Managed Certificate
// resource managedCert 'Microsoft.Web/certificates@2023-12-01' existing = {
//   name: 'sudoku.xenobiasoft.com'
// }

// // Custom Domain Binding with Managed Certificate
// resource customDomainBinding 'Microsoft.Web/sites/hostNameBindings@2023-12-01' = {
//   parent: sudokuApp
//   name: 'sudoku.xenobiasoft.com'
//   properties: {
//     siteName: siteName
//     hostNameType: 'Verified'
//     sslState: 'SniEnabled'
//     thumbprint: managedCert.properties.thumbprint
//   }
// }

output webAppUrl string = sudokuApp.properties.defaultHostName
output apiAppUrl string = sudokuApiApp.properties.defaultHostName
