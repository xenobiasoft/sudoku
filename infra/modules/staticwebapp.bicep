param location string
param environment string
param staticWebAppName string

@description('Custom domain name to bind to the Static Web App (e.g. beta-sudoku.xenobiasoft.com).')
param customDomainName string = ''

@description('Whether to bind a custom domain to the Static Web App. Requires DNS to be configured first.')
param enableCustomDomain bool = false

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

// Azure SWA custom domains get a free managed SSL certificate automatically.
// DNS must be configured before running this deployment (CNAME pointing to
// the SWA's default hostname, or a TXT validation record for apex domains).
resource swaCustomDomain 'Microsoft.Web/staticSites/customDomains@2023-12-01' = if (enableCustomDomain && !empty(customDomainName)) {
  parent: staticWebApp
  name: customDomainName
  properties: {}
}

output staticWebAppUrl string = staticWebApp.properties.defaultHostname
output staticWebAppId string = staticWebApp.id
