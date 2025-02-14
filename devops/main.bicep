param siteName string = 'XenobiaSoftSudoku'
param location string = resourceGroup().location

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: 'XenobiaSoftServicePlan'
  location: location
  properties: {
    reserved: true
  }
  sku: {
    name: 'D1'
    tier: 'Shared'
    size: 'D1'
    family: 'D'
    capacity: 1
  }
  kind: 'app,linux'
}

resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: siteName
  location: location
  kind: 'app'
  properties: {
    enabled: true
    linuxFxVersion: 'DOTNETCORE|8.0'
    serverFarmId: appServicePlan.id
  }
}

resource appServiceConfig 'Microsoft.Web/sites/config@2022-09-01' = {
  parent: appService
  name: 'web'
  location: location
  properties: {
    defaultDocuments: [
      'Default.html'
      'index.html'      
    ]
    httpsOnly: true
    linuxFxVersion: 'DOTNETCORE|8.0'
    netFrameworkVersion: 'v4.0'
    managedPipelineMode: 'Integrated'
    virtualApplications: [
      {
        virtualPath: '/'
        physicalPath: 'site\\wwwroot'
        preloadEnabled: false
      }
    ]
    minTlsVersion: '1.2'    
  }
}
