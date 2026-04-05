// ---------------------------------------------------------------------------
// Step 1 of custom-domain setup: bind the hostname to the web app WITHOUT SSL.
// The managed certificate (step 2) requires the hostname to already be bound
// before it can be provisioned, so this must run first.
// ---------------------------------------------------------------------------

param webAppName string
param customDomainName string

resource webApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: webAppName
}

resource hostnameBinding 'Microsoft.Web/sites/hostNameBindings@2023-12-01' = {
  parent: webApp
  name: customDomainName
  properties: {
    siteName: webAppName
    hostNameType: 'Verified'
    sslState: 'Disabled'
  }
}
