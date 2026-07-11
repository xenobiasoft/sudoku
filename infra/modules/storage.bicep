param location string
param environment string
param storageAccountName string
param cosmosDbAccountName string

@description('Enable the Cosmos DB free tier. Only one account per subscription may use this. Set to false if another account in the subscription already uses it.')
param cosmosDbEnableFreeTier bool = true

@description('Deploy the Cosmos DB account in Serverless capacity mode instead of manual provisioned throughput. Cannot be combined with free tier.')
param cosmosDbServerless bool = false

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

// Serverless accounts reject an explicit throughput cap, so `capacity` is
// omitted entirely rather than passed as an empty object.
var cosmosDbCapacity = cosmosDbServerless ? {} : {
  capacity: {
    totalThroughputLimit: 1000
  }
}

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' = {
  name: cosmosDbAccountName
  location: location
  kind: 'GlobalDocumentDB'
  tags: union(tags, {
    defaultExperience: 'Core (SQL)'
    'hidden-workload-type': 'Production'
  })
  properties: union({
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
        // Serverless accounts are single-region and do not support availability zones.
        isZoneRedundant: !cosmosDbServerless
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
    capabilities: cosmosDbServerless ? [
      {
        name: 'EnableServerless'
      }
    ] : []
    defaultIdentity: 'FirstPartyIdentity'
    // Free tier requires provisioned throughput, so it cannot coexist with
    // serverless. Forced off rather than left to fail the deployment, since
    // cosmosDbEnableFreeTier defaults to true.
    enableFreeTier: cosmosDbEnableFreeTier && !cosmosDbServerless
    enableAutomaticFailover: true
    enableMultipleWriteLocations: false
    enableAnalyticalStorage: false
    enableBurstCapacity: false
    enablePartitionMerge: false
    // Must stay false: the API authenticates with the account key embedded in
    // the ConnectionStrings--CosmosDb Key Vault secret, not managed identity.
    disableLocalAuth: false
    disableKeyBasedMetadataWriteAccess: false
    minimalTlsVersion: 'Tls12'
    networkAclBypass: 'None'
    publicNetworkAccess: 'Enabled'
    virtualNetworkRules: []
    ipRules: []
    cors: []
  }, cosmosDbCapacity)
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

resource gamesThroughput 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/throughputSettings@2024-05-15' = if (!cosmosDbServerless) {
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

resource profilesThroughput 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/throughputSettings@2024-05-15' = if (!cosmosDbServerless) {
  parent: profilesContainer
  name: 'default'
  properties: {
    resource: {
      throughput: 400
    }
  }
}

// Slim, append-only record of every won game (id = gameId, so writes are idempotent).
// Outlives the full game document, which the client deletes on solve.
resource gameCompletionsContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15' = {
  parent: sudokuDatabase
  name: 'game-completions'
  properties: {
    resource: {
      id: 'game-completions'
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

resource gameCompletionsThroughput 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/throughputSettings@2024-05-15' = if (!cosmosDbServerless) {
  parent: gameCompletionsContainer
  name: 'default'
  properties: {
    resource: {
      throughput: 400
    }
  }
}

// ---------------------------------------------------------------------------
// Event Grid system topic — source of BlobDeleted events on this account.
// The eventSubscription that routes BlobDeleted → PuzzleReplenishFunction
// lives in the functions module (it needs the Function App resource id).
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

output storageAccountId string = storageAccount.id
output storageAccountName string = storageAccount.name
output cosmosDbAccountId string = cosmosDbAccount.id
output cosmosDbEndpoint string = cosmosDbAccount.properties.documentEndpoint
output eventGridTopicName string = puzzlePoolEventGridTopic.name
