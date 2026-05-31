param location string
param environment string
param storageAccountName string
param cosmosDbAccountName string

@description('URL of the PuzzleReplenishFunction endpoint (e.g. https://<funcapp>.azurewebsites.net/runtime/webhooks/eventgrid?functionName=PuzzleReplenishFunction&code=<key>). Leave empty to skip Event Grid subscription creation.')
param puzzleReplenishFunctionUrl string = ''

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
  location: location
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
        locationName: location
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
    ipRules: []
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

resource profilesContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15' = {
  parent: sudokuDatabase
  name: 'profiles'
  properties: {
    resource: {
      id: 'profiles'
      partitionKey: {
        paths: ['/profileId']
        kind: 'Hash'
        version: 2
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [{ path: '/*' }]
        excludedPaths: [{ path: '/"_etag"/?' }]
      }
      uniqueKeyPolicy: {
        uniqueKeys: []
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
    }
  }
}

resource profilesThroughput 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/throughputSettings@2024-05-15' = {
  parent: profilesContainer
  name: 'default'
  properties: {
    resource: {
      throughput: 400
    }
  }
}

// ---------------------------------------------------------------------------
// Event Grid — BlobDeleted on sudoku-puzzles container → PuzzleReplenishFunction
// ---------------------------------------------------------------------------

resource puzzlePoolEventGridTopic 'Microsoft.EventGrid/systemTopics@2022-06-15' = {
  name: 'sudoku-puzzle-pool-${environment}'
  location: location
  tags: tags
  properties: {
    source: storageAccount.id
    topicType: 'Microsoft.Storage.StorageAccounts'
  }
}

resource puzzleReplenishSubscription 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2022-06-15' = if (!empty(puzzleReplenishFunctionUrl)) {
  name: 'puzzle-replenish-sub'
  parent: puzzlePoolEventGridTopic
  properties: {
    destination: {
      endpointType: 'WebHook'
      properties: {
        endpointUrl: puzzleReplenishFunctionUrl
      }
    }
    filter: {
      includedEventTypes: [
        'Microsoft.Storage.BlobDeleted'
      ]
      subjectBeginsWith: '/blobServices/default/containers/sudoku-puzzles/'
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
      maxDeliveryAttempts: 30
      eventTimeToLiveInMinutes: 1440
    }
  }
}

output storageAccountId string = storageAccount.id
output storageAccountName string = storageAccount.name
output cosmosDbAccountId string = cosmosDbAccount.id
output cosmosDbEndpoint string = cosmosDbAccount.properties.documentEndpoint
