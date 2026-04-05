param location string
param environment string
param storageAccountName string
param cosmosDbAccountName string

@description('Enable the Cosmos DB free tier. Only one account per subscription may use this. Set to false if another account in the subscription already uses it.')
param cosmosDbEnableFreeTier bool = true

var tags = {
  environment: environment
  project: 'XenobiaSoftSudoku'
}

// ---------------------------------------------------------------------------
// Storage Account
// ---------------------------------------------------------------------------

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  tags: tags
  sku: {
    name: 'Standard_RAGRS'
  }
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowCrossTenantReplication: false
    allowSharedKeyAccess: true
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    encryption: {
      keySource: 'Microsoft.Storage'
      requireInfrastructureEncryption: false
    }
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    deleteRetentionPolicy: {
      allowPermanentDelete: false
      enabled: true
      days: 7
    }
    containerDeleteRetentionPolicy: {
      enabled: true
      days: 7
    }
    cors: {
      corsRules: []
    }
  }
}

resource sudokuPuzzlesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: 'sudoku-puzzles'
  properties: {
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
}

resource fileService 'Microsoft.Storage/storageAccounts/fileServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    shareDeleteRetentionPolicy: {
      enabled: true
      days: 7
    }
    cors: {
      corsRules: []
    }
  }
}

resource adminFileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2023-01-01' = {
  parent: fileService
  name: 'admin-fileshare'
  properties: {
    accessTier: 'TransactionOptimized'
    enabledProtocols: 'SMB'
    shareQuota: 6
  }
}

resource queueService 'Microsoft.Storage/storageAccounts/queueServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource tableService 'Microsoft.Storage/storageAccounts/tableServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

// ---------------------------------------------------------------------------
// Cosmos DB
// ---------------------------------------------------------------------------

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' = {
  name: cosmosDbAccountName
  location: 'CentralUS'
  kind: 'GlobalDocumentDB'
  tags: union(tags, {
    defaultExperience: 'Core (SQL)'
    'hidden-workload-type': 'Production'
  })
  properties: {
    databaseAccountOfferType: 'Standard'
    analyticalStorageConfiguration: {
        schemaType: 'WellDefined'
    }
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: 'centralus'
        failoverPriority: 0
        isZoneRedundant: true
      }
    ]
    backupPolicy: {
      type: 'Periodic'
      periodicModeProperties: {
        backupIntervalInMinutes: 240
        backupRetentionIntervalInHours: 8
        backupStorageRedundancy: 'Geo'
      }
    }
    capabilities: []
    capacity: {
      totalThroughputLimit: 1000
    }
    defaultIdentity: 'FirstPartyIdentity'
    enableFreeTier: cosmosDbEnableFreeTier
    enableAutomaticFailover: true
    enableMultipleWriteLocations: false
    enableAnalyticalStorage: false
    enableBurstCapacity: false
    enablePartitionMerge: false
    disableLocalAuth: false
    disableKeyBasedMetadataWriteAccess: false
    minimalTlsVersion: 'Tls12'
    networkAclBypass: 'None'
    publicNetworkAccess: 'Enabled'
    virtualNetworkRules: []
    ipRules: [
      {
        ipAddressOrRange: '76.154.207.26'
      }
      {
        ipAddressOrRange: '20.75.146.211'
      }
      {
        ipAddressOrRange: '20.75.146.221'
      }
      {
        ipAddressOrRange: '20.75.146.228'
      }
      {
        ipAddressOrRange: '20.75.146.229'
      }
      {
        ipAddressOrRange: '20.75.146.254'
      }
      {
        ipAddressOrRange: '20.75.146.255'
      }
      {
        ipAddressOrRange: '40.88.199.185'
      }
      {
        ipAddressOrRange: '20.75.146.16'
      }
      {
        ipAddressOrRange: '20.75.146.17'
      }
      {
        ipAddressOrRange: '20.75.146.24'
      }
      {
        ipAddressOrRange: '20.75.146.25'
      }
      {
        ipAddressOrRange: '20.75.146.30'
      }
      {
        ipAddressOrRange: '20.75.146.31'
      }
      {
        ipAddressOrRange: '20.75.146.32'
      }
      {
        ipAddressOrRange: '20.75.146.33'
      }
      {
        ipAddressOrRange: '20.75.146.40'
      }
      {
        ipAddressOrRange: '20.75.146.64'
      }
      {
        ipAddressOrRange: '20.75.146.65'
      }
      {
        ipAddressOrRange: '20.75.149.122'
      }
      {
        ipAddressOrRange: '20.75.146.74'
      }
      {
        ipAddressOrRange: '40.88.194.183'
      }
      {
        ipAddressOrRange: '20.75.146.166'
      }
      {
        ipAddressOrRange: '20.75.146.194'
      }
      {
        ipAddressOrRange: '20.75.146.195'
      }
      {
        ipAddressOrRange: '20.75.147.4'
      }
      {
        ipAddressOrRange: '20.75.147.5'
      }
      {
        ipAddressOrRange: '20.75.147.18'
      }
      {
        ipAddressOrRange: '20.75.147.19'
      }
      {
        ipAddressOrRange: '20.75.147.37'
      }
      {
        ipAddressOrRange: '20.75.147.65'
      }
      {
        ipAddressOrRange: '20.119.8.46'
      }
      {
        ipAddressOrRange: '4.210.172.107'
      }
      {
        ipAddressOrRange: '13.88.56.148'
      }
      {
        ipAddressOrRange: '13.91.105.215'
      }
      {
        ipAddressOrRange: '40.91.218.243'
      }
    ]
    cors: []
  }
}

resource sudokuDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-05-15' = {
  parent: cosmosDbAccount
  name: 'sudoku'
  properties: {
    resource: {
      id: 'sudoku'
    }
  }
}

resource gamesContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15' = {
    parent: sudokuDatabase
    name: 'games'
    properties: {
        resource: {
            id: 'games'
            partitionKey: {
                paths: [
                    '/gameId'
                ]
                kind: 'Hash'
                version: 2
            }
            indexingPolicy: {
                indexingMode: 'consistent'
                automatic: true
                includedPaths: [
                    {
                        path: '/*'
                    }
                ]
                excludedPaths: [
                    {
                        path: '/"_etag"/?'
                    }
                ]
            }
            conflictResolutionPolicy: {
                mode: 'LastWriterWins'
                conflictResolutionPath: '/_ts'
            }
            uniqueKeyPolicy: {
                uniqueKeys: []
            }
        }
    }
}

resource gamesThroughput 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/throughputSettings@2024-05-15' = {
  parent: gamesContainer
  name: 'default'
  properties: {
    resource: {
      throughput: 400
    }
  }
}

output storageAccountId string = storageAccount.id
output storageAccountName string = storageAccount.name
output cosmosDbAccountId string = cosmosDbAccount.id
output cosmosDbEndpoint string = cosmosDbAccount.properties.documentEndpoint
