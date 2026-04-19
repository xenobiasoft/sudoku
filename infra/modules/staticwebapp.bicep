param location string
param environment string
param staticWebAppName string

@description('Custom domain name to bind to the SWA production environment (e.g. sudoku-beta.xenobiasoft.com).')
param customDomainName string = ''

@description('Whether to bind a custom domain to the SWA. Requires DNS to be configured first.')
param enableCustomDomain bool = false

@description('Resource ID of the API App Service to link as a backend. SWA will proxy /api/* requests to it.')
param apiAppResourceId string

var tags = {
  environment: environment
  project: 'XenobiaSoftSudoku'
}

resource staticWebApp 'Microsoft.Web/staticSites@2023-12-01' = {
  name: staticWebAppName
  location: location
  tags: tags
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {}
}

resource staticWebAppLinkedBackend 'Microsoft.Web/staticSites/linkedBackends@2023-12-01' = {
  parent: staticWebApp
  name: 'backend'
  properties: {
    backendResourceId: apiAppResourceId
    region: location
  }
}

// Binds the custom domain to the SWA production environment.
// Azure SWA automatically provisions a free managed SSL certificate —
// no separate cert or SSL module required.
// DNS prerequisite: create a CNAME pointing customDomainName to the SWA's
// default hostname before enabling this.
resource swaCustomDomain 'Microsoft.Web/staticSites/customDomains@2023-12-01' = if (enableCustomDomain && !empty(customDomainName)) {
  parent: staticWebApp
  name: customDomainName
  properties: {}
}

output staticWebAppUrl string = staticWebApp.properties.defaultHostname
output staticWebAppId string = staticWebApp.id
