param location string
param environment string
param staticWebAppName string

@description('Custom domain name to bind to the SWA production environment (e.g. sudoku-beta.xenobiasoft.com).')
param customDomainName string = ''

@description('Whether to bind a custom domain to the SWA. Requires DNS to be configured first.')
param enableCustomDomain bool = false

@description('Resource ID of the API App Service to link as a backend. SWA will proxy /api/* requests to it.')
param apiAppResourceId string

@description('Name of the API App Service. Used to reset Easy Auth after SWA linking re-enables it.')
param apiAppName string

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

// SWA's linkedBackend creation automatically re-enables Easy Auth on the API App Service
// and configures it to only accept SWA-issued tokens. Reset it here (after linking) so
// direct callers (e.g. the Blazor App Service) are not blocked by the injected auth layer.
resource existingApiApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: apiAppName
}

resource apiAuthDisabledAfterLinking 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: existingApiApp
  name: 'authsettingsV2'
  dependsOn: [staticWebAppLinkedBackend]
  properties: {
    globalValidation: {
      requireAuthentication: false
    }
    platform: {
      enabled: false
    }
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
