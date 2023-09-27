param siteName string = 'XenobiaSoftSudoku'
param location string = resourceGroup().location

resource sites_XenobiaSoftSudoku_resource 'Microsoft.Web/sites@2022-09-01' = {
  name: siteName
  location: location
  kind: 'app,linux'
  properties: {
    enabled: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|7.0'      
    }    
  }
}

resource sites_XenobiaSoftSudoku_web 'Microsoft.Web/sites/config@2022-09-01' = {
  parent: sites_XenobiaSoftSudoku_resource
  name: 'web'
  location: location
  properties: {
    defaultDocuments: [
      'Default.html'
      'index.html'      
    ]
    netFrameworkVersion: 'v4.0'
    linuxFxVersion: 'DOTNETCORE|7.0'
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
