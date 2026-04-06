// ---------------------------------------------------------------------------
// Step 2 of custom-domain setup: create the managed certificate and update
// the hostname binding to enable SNI SSL.
// This module must run AFTER hostname.bicep so that the hostname is already
// bound to the App Service when the certificate is provisioned.
// ---------------------------------------------------------------------------

param location string
param webAppName string
param appServicePlanName string
param customDomainName string

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' existing = {
  name: appServicePlanName
}

resource webApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: webAppName
}

resource certificate 'Microsoft.Web/certificates@2023-12-01' = {
  name: '${customDomainName}-${webAppName}'
  location: location
  kind: 'Managed'
  properties: {
    canonicalName: customDomainName
    hostNames: [
      customDomainName
    ]
    serverFarmId: appServicePlan.id
  }
}

// Updates the existing hostname binding (created without SSL in hostname.bicep)
// to enable SNI SSL using the managed certificate. ARM is idempotent: deploying
// a hostNameBindings resource with the same name replaces the prior binding.
resource sslBinding 'Microsoft.Web/sites/hostNameBindings@2023-12-01' = {
  parent: webApp
  name: customDomainName
  dependsOn: [
    certificate
  ]
  properties: {
    siteName: webAppName
    hostNameType: 'Verified'
    sslState: 'SniEnabled'
    thumbprint: certificate.properties.thumbprint
  }
}
