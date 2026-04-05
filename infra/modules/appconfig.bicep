param location string
param environment string
param appConfigName string

var tags = {
  environment: environment
  project: 'XenobiaSoftSudoku'
}

resource appConfigStore 'Microsoft.AppConfiguration/configurationStores@2023-03-01' = {
  name: appConfigName
  location: location
  tags: tags
  sku: {
    name: 'developer'
  }
  properties: {
    disableLocalAuth: false
    enablePurgeProtection: false
    softDeleteRetentionInDays: 0
    publicNetworkAccess: 'Enabled'
  }
}

output appConfigId string = appConfigStore.id
output appConfigEndpoint string = appConfigStore.properties.endpoint
output appConfigName string = appConfigStore.name
