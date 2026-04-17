param location string
param environment string
param staticWebAppName string

@description('Custom domain name to bind to the SWA staging environment (e.g. sudoku-beta.xenobiasoft.com).')
param stagingCustomDomainName string = ''

@description('Whether to bind a custom domain to the SWA staging environment. Requires DNS to be configured first.')
param enableStagingCustomDomain bool = false

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

// The "staging" named deployment environment within this SWA.
// This environment is created automatically when the GitHub Action deploys
// with deployment_environment: staging.  We reference it here as "existing"
// only when we need to attach a custom domain to it; the resource must
// already exist (i.e. at least one staging deploy has run) before this
// Bicep module can bind the domain.
resource stagingBuild 'Microsoft.Web/staticSites/builds@2023-12-01' existing = if (enableStagingCustomDomain && !empty(stagingCustomDomainName)) {
  parent: staticWebApp
  name: 'staging'
}

// Azure SWA automatically provisions a free managed SSL certificate for
// custom domains — no separate cert or SSL module required.
// DNS prerequisite: create a CNAME record for sudoku-beta.xenobiasoft.com
// pointing to the staging environment hostname before running this deploy.
resource stagingEnvironmentCustomDomain 'Microsoft.Web/staticSites/builds/customDomains@2023-12-01' = if (enableStagingCustomDomain && !empty(stagingCustomDomainName)) {
  parent: stagingBuild
  name: stagingCustomDomainName
  properties: {}
}

output staticWebAppUrl string = staticWebApp.properties.defaultHostname
output staticWebAppId string = staticWebApp.id
