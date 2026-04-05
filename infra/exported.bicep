param actionGroups_Application_Insights_Smart_Detection_name string
param certificates_sudoku_xenobiasoft_com_XenobiaSoftSudoku_name string
param components_XenobiaSoftSudoku_name string
param configurationStores_appcs_xenobiasoft_com_name string
param databaseAccounts_cosmos_sudoku_prod_name string
param serverfarms_XenobiaSoftServicePlan_name string
param sites_XenobiaSoftSudokuApi_name string
param sites_XenobiaSoftSudoku_name string
param smartdetectoralertrules_failure_anomalies_xenobiasoftsudoku_name string
param staticSites_xenobiasoft_blog_name string
param storageAccounts_stxenobiasoft_name string
param vaults_kv_xenobiasoft_name string
param workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_externalid string
param workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name string

resource configurationStores_appcs_xenobiasoft_com_name_resource 'Microsoft.AppConfiguration/configurationStores@2025-06-01-preview' = {
  location: 'eastus'
  name: configurationStores_appcs_xenobiasoft_com_name
  properties: {
    azureFrontDoor: {}
    dataPlaneProxy: {
      authenticationMode: 'Pass-through'
      privateLinkDelegation: 'Disabled'
    }
    defaultKeyValueRevisionRetentionPeriodInSeconds: 604800
    disableLocalAuth: false
    enablePurgeProtection: false
    encryption: {}
    softDeleteRetentionInDays: 0
    telemetry: {}
  }
  sku: {
    name: 'developer'
  }
}

resource databaseAccounts_cosmos_sudoku_prod_name_resource 'Microsoft.DocumentDB/databaseAccounts@2025-11-01-preview' = {
  identity: {
    type: 'None'
  }
  kind: 'GlobalDocumentDB'
  location: 'Central US'
  name: databaseAccounts_cosmos_sudoku_prod_name
  properties: {
    analyticalStorageConfiguration: {
      schemaType: 'WellDefined'
    }
    backupPolicy: {
      periodicModeProperties: {
        backupIntervalInMinutes: 240
        backupRetentionIntervalInHours: 8
        backupStorageRedundancy: 'Geo'
      }
      type: 'Periodic'
    }
    capabilities: []
    capacity: {
      totalThroughputLimit: 1000
    }
    capacityMode: 'Provisioned'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
      maxIntervalInSeconds: 5
      maxStalenessPrefix: 100
    }
    cors: []
    databaseAccountOfferType: 'Standard'
    defaultIdentity: 'FirstPartyIdentity'
    defaultPriorityLevel: 'High'
    diagnosticLogSettings: {
      enableFullTextQuery: 'None'
    }
    disableKeyBasedMetadataWriteAccess: false
    disableLocalAuth: false
    enableAnalyticalStorage: false
    enableAutomaticFailover: true
    enableBurstCapacity: false
    enableFreeTier: true
    enableMaterializedViews: false
    enableMultipleWriteLocations: false
    enablePartitionMerge: false
    enablePerRegionPerPartitionAutoscale: true
    enablePriorityBasedExecution: false
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
    isVirtualNetworkFilterEnabled: false
    locations: [
      {
        failoverPriority: 0
        isZoneRedundant: true
        locationName: 'Central US'
      }
    ]
    minimalTlsVersion: 'Tls12'
    networkAclBypass: 'None'
    networkAclBypassResourceIds: []
    publicNetworkAccess: 'Enabled'
    virtualNetworkRules: []
  }
  tags: {
    defaultExperience: 'Core (SQL)'
    'hidden-cosmos-mmspecial': ''
    'hidden-workload-type': 'Production'
  }
}

resource actionGroups_Application_Insights_Smart_Detection_name_resource 'microsoft.insights/actionGroups@2024-10-01-preview' = {
  location: 'Global'
  name: actionGroups_Application_Insights_Smart_Detection_name
  properties: {
    armRoleReceivers: [
      {
        name: 'Monitoring Contributor'
        roleId: '749f88d5-cbae-40b8-bcfc-e573ddc772fa'
        useCommonAlertSchema: true
      }
      {
        name: 'Monitoring Reader'
        roleId: '43d0d8ad-25c7-4714-9337-8ba259a9fe05'
        useCommonAlertSchema: true
      }
    ]
    automationRunbookReceivers: []
    azureAppPushReceivers: []
    azureFunctionReceivers: []
    emailReceivers: []
    enabled: true
    eventHubReceivers: []
    groupShortName: 'SmartDetect'
    itsmReceivers: []
    logicAppReceivers: []
    smsReceivers: []
    voiceReceivers: []
    webhookReceivers: []
  }
}

resource components_XenobiaSoftSudoku_name_resource 'microsoft.insights/components@2020-02-02' = {
  kind: 'web'
  location: 'eastus'
  name: components_XenobiaSoftSudoku_name
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Redfield'
    IngestionMode: 'LogAnalytics'
    Request_Source: 'IbizaAIExtensionEnablementBlade'
    RetentionInDays: 90
    WorkspaceResourceId: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_externalid
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource vaults_kv_xenobiasoft_name_resource 'Microsoft.KeyVault/vaults@2025-05-01' = {
  location: 'centralus'
  name: vaults_kv_xenobiasoft_name
  properties: {
    accessPolicies: []
    enableRbacAuthorization: true
    enableSoftDelete: true
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: true
    provisioningState: 'Succeeded'
    publicNetworkAccess: 'Enabled'
    sku: {
      family: 'A'
      name: 'Standard'
    }
    softDeleteRetentionInDays: 90
    tenantId: '76534de5-00fb-428e-8334-355e822797dd'
    vaultUri: 'https://${vaults_kv_xenobiasoft_name}.vault.azure.net/'
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource 'Microsoft.OperationalInsights/workspaces@2025-07-01' = {
  location: 'eastus'
  name: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name
  properties: {
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
      legacy: 0
      searchVersion: 1
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    retentionInDays: 30
    sku: {
      name: 'PerGB2018'
    }
    workspaceCapping: {
      dailyQuotaGb: json('-1')
    }
  }
}

resource storageAccounts_stxenobiasoft_name_resource 'Microsoft.Storage/storageAccounts@2025-06-01' = {
  kind: 'StorageV2'
  location: 'eastus'
  name: storageAccounts_stxenobiasoft_name
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowCrossTenantReplication: false
    allowSharedKeyAccess: true
    defaultToOAuthAuthentication: false
    dnsEndpointType: 'Standard'
    encryption: {
      keySource: 'Microsoft.Storage'
      requireInfrastructureEncryption: false
      services: {
        blob: {
          enabled: true
          keyType: 'Account'
        }
        file: {
          enabled: true
          keyType: 'Account'
        }
      }
    }
    largeFileSharesState: 'Enabled'
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
      ipRules: []
      ipv6Rules: []
      virtualNetworkRules: []
    }
    publicNetworkAccess: 'Enabled'
    supportsHttpsTrafficOnly: true
  }
  sku: {
    name: 'Standard_RAGRS'
    tier: 'Standard'
  }
}

resource certificates_sudoku_xenobiasoft_com_XenobiaSoftSudoku_name_resource 'Microsoft.Web/certificates@2024-11-01' = {
  location: 'East US'
  name: certificates_sudoku_xenobiasoft_com_XenobiaSoftSudoku_name
  properties: {
    canonicalName: 'sudoku.xenobiasoft.com'
    hostNames: [
      'sudoku.xenobiasoft.com'
    ]
  }
}

resource serverfarms_XenobiaSoftServicePlan_name_resource 'Microsoft.Web/serverfarms@2024-11-01' = {
  kind: 'linux'
  location: 'East US'
  name: serverfarms_XenobiaSoftServicePlan_name
  properties: {
    asyncScalingEnabled: false
    elasticScaleEnabled: false
    hyperV: false
    isSpot: false
    isXenon: false
    maximumElasticWorkerCount: 1
    perSiteScaling: false
    reserved: true
    targetWorkerCount: 0
    targetWorkerSizeId: 0
    zoneRedundant: false
  }
  sku: {
    capacity: 1
    family: 'B'
    name: 'B1'
    size: 'B1'
    tier: 'Basic'
  }
}

resource staticSites_xenobiasoft_blog_name_resource 'Microsoft.Web/staticSites@2024-11-01' = {
  location: 'Central US'
  name: staticSites_xenobiasoft_blog_name
  properties: {
    allowConfigFileUpdates: true
    branch: 'main'
    enterpriseGradeCdnStatus: 'Disabled'
    provider: 'GitHub'
    repositoryUrl: 'https://github.com/xenobiasoft/${staticSites_xenobiasoft_blog_name}'
    stagingEnvironmentPolicy: 'Enabled'
  }
  sku: {
    name: 'Free'
    tier: 'Free'
  }
}

resource databaseAccounts_cosmos_sudoku_prod_name_00000000_0000_0000_0000_000000000003 'Microsoft.DocumentDB/databaseAccounts/cassandraRoleDefinitions@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_resource
  name: '00000000-0000-0000-0000-000000000003'
  properties: {
    assignableScopes: [
      databaseAccounts_cosmos_sudoku_prod_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/containers/executeQuery'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/containers/readChangeFeed'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/containers/entities/read'
        ]
        notDataActions: []
      }
    ]
    roleName: 'Cosmos DB Cassandra Built-in Data Reader'
    type: 'BuiltInRole'
  }
}

resource databaseAccounts_cosmos_sudoku_prod_name_00000000_0000_0000_0000_000000000004 'Microsoft.DocumentDB/databaseAccounts/cassandraRoleDefinitions@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_resource
  name: '00000000-0000-0000-0000-000000000004'
  properties: {
    assignableScopes: [
      databaseAccounts_cosmos_sudoku_prod_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/write'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/*'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/write'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/delete'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/cassandra/containers/entities/*'
        ]
        notDataActions: []
      }
    ]
    roleName: 'Cosmos DB Cassandra Built-in Data Contributor'
    type: 'BuiltInRole'
  }
}

resource Microsoft_DocumentDB_databaseAccounts_gremlinRoleDefinitions_databaseAccounts_cosmos_sudoku_prod_name_00000000_0000_0000_0000_000000000003 'Microsoft.DocumentDB/databaseAccounts/gremlinRoleDefinitions@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_resource
  name: '00000000-0000-0000-0000-000000000003'
  properties: {
    assignableScopes: [
      databaseAccounts_cosmos_sudoku_prod_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/containers/executeQuery'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/containers/readChangeFeed'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/containers/entities/read'
        ]
        notDataActions: []
      }
    ]
    roleName: 'Cosmos DB Gremlin Built-in Data Reader'
    type: 'BuiltInRole'
  }
}

resource Microsoft_DocumentDB_databaseAccounts_gremlinRoleDefinitions_databaseAccounts_cosmos_sudoku_prod_name_00000000_0000_0000_0000_000000000004 'Microsoft.DocumentDB/databaseAccounts/gremlinRoleDefinitions@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_resource
  name: '00000000-0000-0000-0000-000000000004'
  properties: {
    assignableScopes: [
      databaseAccounts_cosmos_sudoku_prod_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/write'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/*'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/write'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/delete'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/gremlin/containers/entities/*'
        ]
        notDataActions: []
      }
    ]
    roleName: 'Cosmos DB Gremlin Built-in Data Contributor'
    type: 'BuiltInRole'
  }
}

resource Microsoft_DocumentDB_databaseAccounts_mongoMIRoleDefinitions_databaseAccounts_cosmos_sudoku_prod_name_00000000_0000_0000_0000_000000000003 'Microsoft.DocumentDB/databaseAccounts/mongoMIRoleDefinitions@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_resource
  name: '00000000-0000-0000-0000-000000000003'
  properties: {
    assignableScopes: [
      databaseAccounts_cosmos_sudoku_prod_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/containers/executeQuery'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/containers/readChangeFeed'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/containers/entities/read'
        ]
        notDataActions: []
      }
    ]
    roleName: 'Cosmos DB Mongo Built-in Data Reader'
    type: 'BuiltInRole'
  }
}

resource Microsoft_DocumentDB_databaseAccounts_mongoMIRoleDefinitions_databaseAccounts_cosmos_sudoku_prod_name_00000000_0000_0000_0000_000000000004 'Microsoft.DocumentDB/databaseAccounts/mongoMIRoleDefinitions@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_resource
  name: '00000000-0000-0000-0000-000000000004'
  properties: {
    assignableScopes: [
      databaseAccounts_cosmos_sudoku_prod_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/read'
          'Microsoft.DocumentDB/databaseAccounts/throughputSettings/write'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/*'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/write'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/delete'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/mongoMI/containers/entities/*'
        ]
        notDataActions: []
      }
    ]
    roleName: 'Cosmos DB Mongo Built-in Data Contributor'
    type: 'BuiltInRole'
  }
}

resource databaseAccounts_cosmos_sudoku_prod_name_sudoku 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_resource
  name: 'sudoku'
  properties: {
    resource: {
      id: 'sudoku'
    }
  }
}

resource databaseAccounts_cosmos_sudoku_prod_name_00000000_0000_0000_0000_000000000001 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_resource
  name: '00000000-0000-0000-0000-000000000001'
  properties: {
    assignableScopes: [
      databaseAccounts_cosmos_sudoku_prod_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/executeQuery'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/readChangeFeed'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/read'
        ]
        notDataActions: []
      }
    ]
    roleName: 'Cosmos DB Built-in Data Reader'
    type: 'BuiltInRole'
  }
}

resource databaseAccounts_cosmos_sudoku_prod_name_00000000_0000_0000_0000_000000000002 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_resource
  name: '00000000-0000-0000-0000-000000000002'
  properties: {
    assignableScopes: [
      databaseAccounts_cosmos_sudoku_prod_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*'
        ]
        notDataActions: []
      }
    ]
    roleName: 'Cosmos DB Built-in Data Contributor'
    type: 'BuiltInRole'
  }
}

resource Microsoft_DocumentDB_databaseAccounts_tableRoleDefinitions_databaseAccounts_cosmos_sudoku_prod_name_00000000_0000_0000_0000_000000000001 'Microsoft.DocumentDB/databaseAccounts/tableRoleDefinitions@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_resource
  name: '00000000-0000-0000-0000-000000000001'
  properties: {
    assignableScopes: [
      databaseAccounts_cosmos_sudoku_prod_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/tables/containers/executeQuery'
          'Microsoft.DocumentDB/databaseAccounts/tables/containers/readChangeFeed'
          'Microsoft.DocumentDB/databaseAccounts/tables/containers/entities/read'
        ]
        notDataActions: []
      }
    ]
    roleName: 'Cosmos DB Built-in Data Reader'
    type: 'BuiltInRole'
  }
}

resource Microsoft_DocumentDB_databaseAccounts_tableRoleDefinitions_databaseAccounts_cosmos_sudoku_prod_name_00000000_0000_0000_0000_000000000002 'Microsoft.DocumentDB/databaseAccounts/tableRoleDefinitions@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_resource
  name: '00000000-0000-0000-0000-000000000002'
  properties: {
    assignableScopes: [
      databaseAccounts_cosmos_sudoku_prod_name_resource.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/tables/*'
          'Microsoft.DocumentDB/databaseAccounts/tables/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/tables/containers/entities/*'
        ]
        notDataActions: []
      }
    ]
    roleName: 'Cosmos DB Built-in Data Contributor'
    type: 'BuiltInRole'
  }
}

resource components_XenobiaSoftSudoku_name_degradationindependencyduration 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'degradationindependencyduration'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'Smart Detection rules notify you of performance anomaly issues.'
      DisplayName: 'Degradation in dependency duration'
      HelpUrl: 'https://docs.microsoft.com/en-us/azure/application-insights/app-insights-proactive-performance-diagnostics'
      IsEnabledByDefault: true
      IsHidden: false
      IsInPreview: false
      Name: 'degradationindependencyduration'
      SupportsEmailNotifications: true
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_degradationinserverresponsetime 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'degradationinserverresponsetime'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'Smart Detection rules notify you of performance anomaly issues.'
      DisplayName: 'Degradation in server response time'
      HelpUrl: 'https://docs.microsoft.com/en-us/azure/application-insights/app-insights-proactive-performance-diagnostics'
      IsEnabledByDefault: true
      IsHidden: false
      IsInPreview: false
      Name: 'degradationinserverresponsetime'
      SupportsEmailNotifications: true
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_digestMailConfiguration 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'digestMailConfiguration'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'This rule describes the digest mail preferences'
      DisplayName: 'Digest Mail Configuration'
      HelpUrl: 'www.homail.com'
      IsEnabledByDefault: true
      IsHidden: true
      IsInPreview: false
      Name: 'digestMailConfiguration'
      SupportsEmailNotifications: true
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_extension_billingdatavolumedailyspikeextension 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'extension_billingdatavolumedailyspikeextension'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'This detection rule automatically analyzes the billing data generated by your application, and can warn you about an unusual increase in your application\'s billing costs'
      DisplayName: 'Abnormal rise in daily data volume (preview)'
      HelpUrl: 'https://github.com/Microsoft/ApplicationInsights-Home/tree/master/SmartDetection/billing-data-volume-daily-spike.md'
      IsEnabledByDefault: true
      IsHidden: false
      IsInPreview: true
      Name: 'extension_billingdatavolumedailyspikeextension'
      SupportsEmailNotifications: false
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_extension_canaryextension 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'extension_canaryextension'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'Canary extension'
      DisplayName: 'Canary extension'
      HelpUrl: 'https://github.com/Microsoft/ApplicationInsights-Home/blob/master/SmartDetection/'
      IsEnabledByDefault: true
      IsHidden: true
      IsInPreview: true
      Name: 'extension_canaryextension'
      SupportsEmailNotifications: false
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_extension_exceptionchangeextension 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'extension_exceptionchangeextension'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'This detection rule automatically analyzes the exceptions thrown in your application, and can warn you about unusual patterns in your exception telemetry.'
      DisplayName: 'Abnormal rise in exception volume (preview)'
      HelpUrl: 'https://github.com/Microsoft/ApplicationInsights-Home/blob/master/SmartDetection/abnormal-rise-in-exception-volume.md'
      IsEnabledByDefault: true
      IsHidden: false
      IsInPreview: true
      Name: 'extension_exceptionchangeextension'
      SupportsEmailNotifications: false
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_extension_memoryleakextension 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'extension_memoryleakextension'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'This detection rule automatically analyzes the memory consumption of each process in your application, and can warn you about potential memory leaks or increased memory consumption.'
      DisplayName: 'Potential memory leak detected (preview)'
      HelpUrl: 'https://github.com/Microsoft/ApplicationInsights-Home/tree/master/SmartDetection/memory-leak.md'
      IsEnabledByDefault: true
      IsHidden: false
      IsInPreview: true
      Name: 'extension_memoryleakextension'
      SupportsEmailNotifications: false
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_extension_securityextensionspackage 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'extension_securityextensionspackage'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'This detection rule automatically analyzes the telemetry generated by your application and detects potential security issues.'
      DisplayName: 'Potential security issue detected (preview)'
      HelpUrl: 'https://github.com/Microsoft/ApplicationInsights-Home/blob/master/SmartDetection/application-security-detection-pack.md'
      IsEnabledByDefault: true
      IsHidden: false
      IsInPreview: true
      Name: 'extension_securityextensionspackage'
      SupportsEmailNotifications: false
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_extension_traceseveritydetector 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'extension_traceseveritydetector'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'This detection rule automatically analyzes the trace logs emitted from your application, and can warn you about unusual patterns in the severity of your trace telemetry.'
      DisplayName: 'Degradation in trace severity ratio (preview)'
      HelpUrl: 'https://github.com/Microsoft/ApplicationInsights-Home/blob/master/SmartDetection/degradation-in-trace-severity-ratio.md'
      IsEnabledByDefault: true
      IsHidden: false
      IsInPreview: true
      Name: 'extension_traceseveritydetector'
      SupportsEmailNotifications: false
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_longdependencyduration 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'longdependencyduration'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'Smart Detection rules notify you of performance anomaly issues.'
      DisplayName: 'Long dependency duration'
      HelpUrl: 'https://docs.microsoft.com/en-us/azure/application-insights/app-insights-proactive-performance-diagnostics'
      IsEnabledByDefault: true
      IsHidden: false
      IsInPreview: false
      Name: 'longdependencyduration'
      SupportsEmailNotifications: true
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_migrationToAlertRulesCompleted 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'migrationToAlertRulesCompleted'
  properties: {
    customEmails: []
    enabled: false
    ruleDefinitions: {
      Description: 'A configuration that controls the migration state of Smart Detection to Smart Alerts'
      DisplayName: 'Migration To Alert Rules Completed'
      HelpUrl: 'https://docs.microsoft.com/en-us/azure/application-insights/app-insights-proactive-performance-diagnostics'
      IsEnabledByDefault: false
      IsHidden: true
      IsInPreview: true
      Name: 'migrationToAlertRulesCompleted'
      SupportsEmailNotifications: false
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_slowpageloadtime 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'slowpageloadtime'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'Smart Detection rules notify you of performance anomaly issues.'
      DisplayName: 'Slow page load time'
      HelpUrl: 'https://docs.microsoft.com/en-us/azure/application-insights/app-insights-proactive-performance-diagnostics'
      IsEnabledByDefault: true
      IsHidden: false
      IsInPreview: false
      Name: 'slowpageloadtime'
      SupportsEmailNotifications: true
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource components_XenobiaSoftSudoku_name_slowserverresponsetime 'microsoft.insights/components/ProactiveDetectionConfigs@2018-05-01-preview' = {
  parent: components_XenobiaSoftSudoku_name_resource
  location: 'eastus'
  name: 'slowserverresponsetime'
  properties: {
    customEmails: []
    enabled: true
    ruleDefinitions: {
      Description: 'Smart Detection rules notify you of performance anomaly issues.'
      DisplayName: 'Slow server response time'
      HelpUrl: 'https://docs.microsoft.com/en-us/azure/application-insights/app-insights-proactive-performance-diagnostics'
      IsEnabledByDefault: true
      IsHidden: false
      IsInPreview: false
      Name: 'slowserverresponsetime'
      SupportsEmailNotifications: true
    }
    sendEmailsToSubscriptionOwners: true
  }
}

resource vaults_kv_xenobiasoft_name_AppInsightsConnectionString 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: vaults_kv_xenobiasoft_name_resource
  location: 'centralus'
  name: 'AppInsightsConnectionString'
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource vaults_kv_xenobiasoft_name_AppInsightsKey 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: vaults_kv_xenobiasoft_name_resource
  location: 'centralus'
  name: 'AppInsightsKey'
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource vaults_kv_xenobiasoft_name_APPLICATIONINSIGHTS_CONNECTION_STRING 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: vaults_kv_xenobiasoft_name_resource
  location: 'centralus'
  name: 'APPLICATIONINSIGHTS-CONNECTION-STRING'
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource vaults_kv_xenobiasoft_name_AzureStorageConnection 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: vaults_kv_xenobiasoft_name_resource
  location: 'centralus'
  name: 'AzureStorageConnection'
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource vaults_kv_xenobiasoft_name_BugFixerAgentPrivKey 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: vaults_kv_xenobiasoft_name_resource
  location: 'centralus'
  name: 'BugFixerAgentPrivKey'
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource vaults_kv_xenobiasoft_name_BugFixerAgentSecret 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: vaults_kv_xenobiasoft_name_resource
  location: 'centralus'
  name: 'BugFixerAgentSecret'
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource vaults_kv_xenobiasoft_name_CosmosDbConnection 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: vaults_kv_xenobiasoft_name_resource
  location: 'centralus'
  name: 'CosmosDbConnection'
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource vaults_kv_xenobiasoft_name_OpenAIApiKey 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: vaults_kv_xenobiasoft_name_resource
  location: 'centralus'
  name: 'OpenAIApiKey'
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_General_AlphabeticallySortedComputers 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_General|AlphabeticallySortedComputers'
  properties: {
    category: 'General Exploration'
    displayName: 'All Computers with their most recent data'
    query: 'search not(ObjectName == "Advisor Metrics" or ObjectName == "ManagedSpace") | summarize AggregatedValue = max(TimeGenerated) by Computer | limit 500000 | sort by Computer asc\r\n// Oql: NOT(ObjectName="Advisor Metrics" OR ObjectName=ManagedSpace) | measure max(TimeGenerated) by Computer | top 500000 | Sort Computer // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_General_dataPointsPerManagementGroup 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_General|dataPointsPerManagementGroup'
  properties: {
    category: 'General Exploration'
    displayName: 'Which Management Group is generating the most data points?'
    query: 'search * | summarize AggregatedValue = count() by ManagementGroupName\r\n// Oql: * | Measure count() by ManagementGroupName // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_General_dataTypeDistribution 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_General|dataTypeDistribution'
  properties: {
    category: 'General Exploration'
    displayName: 'Distribution of data Types'
    query: 'search * | extend Type = $table | summarize AggregatedValue = count() by Type\r\n// Oql: * | Measure count() by Type // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_General_StaleComputers 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_General|StaleComputers'
  properties: {
    category: 'General Exploration'
    displayName: 'Stale Computers (data older than 24 hours)'
    query: 'search not(ObjectName == "Advisor Metrics" or ObjectName == "ManagedSpace") | summarize lastdata = max(TimeGenerated) by Computer | limit 500000 | where lastdata < ago(24h)\r\n// Oql: NOT(ObjectName="Advisor Metrics" OR ObjectName=ManagedSpace) | measure max(TimeGenerated) as lastdata by Computer | top 500000 | where lastdata < NOW-24HOURS // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_AllEvents 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|AllEvents'
  properties: {
    category: 'Log Management'
    displayName: 'All Events'
    query: 'Event | sort by TimeGenerated desc\r\n// Oql: Type=Event // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_AllSyslog 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|AllSyslog'
  properties: {
    category: 'Log Management'
    displayName: 'All Syslogs'
    query: 'Syslog | sort by TimeGenerated desc\r\n// Oql: Type=Syslog // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_AllSyslogByFacility 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|AllSyslogByFacility'
  properties: {
    category: 'Log Management'
    displayName: 'All Syslog Records grouped by Facility'
    query: 'Syslog | summarize AggregatedValue = count() by Facility\r\n// Oql: Type=Syslog | Measure count() by Facility // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_AllSyslogByProcess 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|AllSyslogByProcessName'
  properties: {
    category: 'Log Management'
    displayName: 'All Syslog Records grouped by ProcessName'
    query: 'Syslog | summarize AggregatedValue = count() by ProcessName\r\n// Oql: Type=Syslog | Measure count() by ProcessName // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_AllSyslogsWithErrors 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|AllSyslogsWithErrors'
  properties: {
    category: 'Log Management'
    displayName: 'All Syslog Records with Errors'
    query: 'Syslog | where SeverityLevel == "error" | sort by TimeGenerated desc\r\n// Oql: Type=Syslog SeverityLevel=error // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_AverageHTTPRequestTimeByClientIPAddress 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|AverageHTTPRequestTimeByClientIPAddress'
  properties: {
    category: 'Log Management'
    displayName: 'Average HTTP Request time by Client IP Address'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = avg(TimeTaken) by cIP\r\n// Oql: Type=W3CIISLog | Measure Avg(TimeTaken) by cIP // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_AverageHTTPRequestTimeHTTPMethod 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|AverageHTTPRequestTimeHTTPMethod'
  properties: {
    category: 'Log Management'
    displayName: 'Average HTTP Request time by HTTP Method'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = avg(TimeTaken) by csMethod\r\n// Oql: Type=W3CIISLog | Measure Avg(TimeTaken) by csMethod // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_CountIISLogEntriesClientIPAddress 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|CountIISLogEntriesClientIPAddress'
  properties: {
    category: 'Log Management'
    displayName: 'Count of IIS Log Entries by Client IP Address'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by cIP\r\n// Oql: Type=W3CIISLog | Measure count() by cIP // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_CountIISLogEntriesHTTPRequestMethod 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|CountIISLogEntriesHTTPRequestMethod'
  properties: {
    category: 'Log Management'
    displayName: 'Count of IIS Log Entries by HTTP Request Method'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csMethod\r\n// Oql: Type=W3CIISLog | Measure count() by csMethod // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_CountIISLogEntriesHTTPUserAgent 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|CountIISLogEntriesHTTPUserAgent'
  properties: {
    category: 'Log Management'
    displayName: 'Count of IIS Log Entries by HTTP User Agent'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csUserAgent\r\n// Oql: Type=W3CIISLog | Measure count() by csUserAgent // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_CountOfIISLogEntriesByHostRequestedByClient 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|CountOfIISLogEntriesByHostRequestedByClient'
  properties: {
    category: 'Log Management'
    displayName: 'Count of IIS Log Entries by Host requested by client'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csHost\r\n// Oql: Type=W3CIISLog | Measure count() by csHost // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_CountOfIISLogEntriesByURLForHost 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|CountOfIISLogEntriesByURLForHost'
  properties: {
    category: 'Log Management'
    displayName: 'Count of IIS Log Entries by URL for the host "www.contoso.com" (replace with your own)'
    query: 'search csHost == "www.contoso.com" | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csUriStem\r\n// Oql: Type=W3CIISLog csHost="www.contoso.com" | Measure count() by csUriStem // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_CountOfIISLogEntriesByURLRequestedByClient 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|CountOfIISLogEntriesByURLRequestedByClient'
  properties: {
    category: 'Log Management'
    displayName: 'Count of IIS Log Entries by URL requested by client (without query strings)'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csUriStem\r\n// Oql: Type=W3CIISLog | Measure count() by csUriStem // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_CountOfWarningEvents 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|CountOfWarningEvents'
  properties: {
    category: 'Log Management'
    displayName: 'Count of Events with level "Warning" grouped by Event ID'
    query: 'Event | where EventLevelName == "warning" | summarize AggregatedValue = count() by EventID\r\n// Oql: Type=Event EventLevelName=warning | Measure count() by EventID // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_DisplayBreakdownRespondCodes 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|DisplayBreakdownRespondCodes'
  properties: {
    category: 'Log Management'
    displayName: 'Shows breakdown of response codes'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by scStatus\r\n// Oql: Type=W3CIISLog | Measure count() by scStatus // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_EventsByEventLog 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|EventsByEventLog'
  properties: {
    category: 'Log Management'
    displayName: 'Count of Events grouped by Event Log'
    query: 'Event | summarize AggregatedValue = count() by EventLog\r\n// Oql: Type=Event | Measure count() by EventLog // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_EventsByEventsID 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|EventsByEventsID'
  properties: {
    category: 'Log Management'
    displayName: 'Count of Events grouped by Event ID'
    query: 'Event | summarize AggregatedValue = count() by EventID\r\n// Oql: Type=Event | Measure count() by EventID // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_EventsByEventSource 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|EventsByEventSource'
  properties: {
    category: 'Log Management'
    displayName: 'Count of Events grouped by Event Source'
    query: 'Event | summarize AggregatedValue = count() by Source\r\n// Oql: Type=Event | Measure count() by Source // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_EventsInOMBetween2000to3000 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|EventsInOMBetween2000to3000'
  properties: {
    category: 'Log Management'
    displayName: 'Events in the Operations Manager Event Log whose Event ID is in the range between 2000 and 3000'
    query: 'Event | where EventLog == "Operations Manager" and EventID >= 2000 and EventID <= 3000 | sort by TimeGenerated desc\r\n// Oql: Type=Event EventLog="Operations Manager" EventID:[2000..3000] // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_EventsWithStartedinEventID 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|EventsWithStartedinEventID'
  properties: {
    category: 'Log Management'
    displayName: 'Count of Events containing the word "started" grouped by EventID'
    query: 'search in (Event) "started" | summarize AggregatedValue = count() by EventID\r\n// Oql: Type=Event "started" | Measure count() by EventID // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_FindMaximumTimeTakenForEachPage 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|FindMaximumTimeTakenForEachPage'
  properties: {
    category: 'Log Management'
    displayName: 'Find the maximum time taken for each page'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = max(TimeTaken) by csUriStem\r\n// Oql: Type=W3CIISLog | Measure Max(TimeTaken) by csUriStem // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_IISLogEntriesForClientIP 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|IISLogEntriesForClientIP'
  properties: {
    category: 'Log Management'
    displayName: 'IIS Log Entries for a specific client IP Address (replace with your own)'
    query: 'search cIP == "192.168.0.1" | extend Type = $table | where Type == W3CIISLog | sort by TimeGenerated desc | project csUriStem, scBytes, csBytes, TimeTaken, scStatus\r\n// Oql: Type=W3CIISLog cIP="192.168.0.1" | Select csUriStem,scBytes,csBytes,TimeTaken,scStatus // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_ListAllIISLogEntries 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|ListAllIISLogEntries'
  properties: {
    category: 'Log Management'
    displayName: 'All IIS Log Entries'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | sort by TimeGenerated desc\r\n// Oql: Type=W3CIISLog // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_NoOfConnectionsToOMSDKService 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|NoOfConnectionsToOMSDKService'
  properties: {
    category: 'Log Management'
    displayName: 'How many connections to Operations Manager\'s SDK service by day'
    query: 'Event | where EventID == 26328 and EventLog == "Operations Manager" | summarize AggregatedValue = count() by bin(TimeGenerated, 1d) | sort by TimeGenerated desc\r\n// Oql: Type=Event EventID=26328 EventLog="Operations Manager" | Measure count() interval 1DAY // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_ServerRestartTime 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|ServerRestartTime'
  properties: {
    category: 'Log Management'
    displayName: 'When did my servers initiate restart?'
    query: 'search in (Event) "shutdown" and EventLog == "System" and Source == "User32" and EventID == 1074 | sort by TimeGenerated desc | project TimeGenerated, Computer\r\n// Oql: shutdown Type=Event EventLog=System Source=User32 EventID=1074 | Select TimeGenerated,Computer // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_Show404PagesList 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|Show404PagesList'
  properties: {
    category: 'Log Management'
    displayName: 'Shows which pages people are getting a 404 for'
    query: 'search scStatus == 404 | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by csUriStem\r\n// Oql: Type=W3CIISLog scStatus=404 | Measure count() by csUriStem // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_ShowServersThrowingInternalServerError 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|ShowServersThrowingInternalServerError'
  properties: {
    category: 'Log Management'
    displayName: 'Shows servers that are throwing internal server error'
    query: 'search scStatus == 500 | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = count() by sComputerName\r\n// Oql: Type=W3CIISLog scStatus=500 | Measure count() by sComputerName // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_TotalBytesReceivedByEachAzureRoleInstance 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|TotalBytesReceivedByEachAzureRoleInstance'
  properties: {
    category: 'Log Management'
    displayName: 'Total Bytes received by each Azure Role Instance'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = sum(csBytes) by RoleInstance\r\n// Oql: Type=W3CIISLog | Measure Sum(csBytes) by RoleInstance // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_TotalBytesReceivedByEachIISComputer 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|TotalBytesReceivedByEachIISComputer'
  properties: {
    category: 'Log Management'
    displayName: 'Total Bytes received by each IIS Computer'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = sum(csBytes) by Computer | limit 500000\r\n// Oql: Type=W3CIISLog | Measure Sum(csBytes) by Computer | top 500000 // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_TotalBytesRespondedToClientsByClientIPAddress 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|TotalBytesRespondedToClientsByClientIPAddress'
  properties: {
    category: 'Log Management'
    displayName: 'Total Bytes responded back to clients by Client IP Address'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = sum(scBytes) by cIP\r\n// Oql: Type=W3CIISLog | Measure Sum(scBytes) by cIP // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_TotalBytesRespondedToClientsByEachIISServerIPAddress 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|TotalBytesRespondedToClientsByEachIISServerIPAddress'
  properties: {
    category: 'Log Management'
    displayName: 'Total Bytes responded back to clients by each IIS ServerIP Address'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = sum(scBytes) by sIP\r\n// Oql: Type=W3CIISLog | Measure Sum(scBytes) by sIP // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_TotalBytesSentByClientIPAddress 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|TotalBytesSentByClientIPAddress'
  properties: {
    category: 'Log Management'
    displayName: 'Total Bytes sent by Client IP Address'
    query: 'search * | extend Type = $table | where Type == W3CIISLog | summarize AggregatedValue = sum(csBytes) by cIP\r\n// Oql: Type=W3CIISLog | Measure Sum(csBytes) by cIP // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PEF: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_WarningEvents 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|WarningEvents'
  properties: {
    category: 'Log Management'
    displayName: 'All Events with level "Warning"'
    query: 'Event | where EventLevelName == "warning" | sort by TimeGenerated desc\r\n// Oql: Type=Event EventLevelName=warning // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_WindowsFireawallPolicySettingsChanged 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|WindowsFireawallPolicySettingsChanged'
  properties: {
    category: 'Log Management'
    displayName: 'Windows Firewall Policy settings have changed'
    query: 'Event | where EventLog == "Microsoft-Windows-Windows Firewall With Advanced Security/Firewall" and EventID == 2008 | sort by TimeGenerated desc\r\n// Oql: Type=Event EventLog="Microsoft-Windows-Windows Firewall With Advanced Security/Firewall" EventID=2008 // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogManagement_WindowsFireawallPolicySettingsChangedByMachines 'Microsoft.OperationalInsights/workspaces/savedSearches@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogManagement(${workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name})_LogManagement|WindowsFireawallPolicySettingsChangedByMachines'
  properties: {
    category: 'Log Management'
    displayName: 'On which machines and how many times have Windows Firewall Policy settings changed'
    query: 'Event | where EventLog == "Microsoft-Windows-Windows Firewall With Advanced Security/Firewall" and EventID == 2008 | summarize AggregatedValue = count() by Computer | limit 500000\r\n// Oql: Type=Event EventLog="Microsoft-Windows-Windows Firewall With Advanced Security/Firewall" EventID=2008 | measure count() by Computer | top 500000 // Args: {OQ: True; WorkspaceId: 00000000-0000-0000-0000-000000000000} // Settings: {PTT: True; SortI: True; SortF: True} // Version: 0.1.122'
    version: 2
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AACAudit 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AACAudit'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AACAudit'
      name: 'AACAudit'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AACHttpRequest 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AACHttpRequest'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AACHttpRequest'
      name: 'AACHttpRequest'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADB2CRequestLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADB2CRequestLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADB2CRequestLogs'
      name: 'AADB2CRequestLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADCustomSecurityAttributeAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADCustomSecurityAttributeAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADCustomSecurityAttributeAuditLogs'
      name: 'AADCustomSecurityAttributeAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADDomainServicesAccountLogon 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADDomainServicesAccountLogon'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADDomainServicesAccountLogon'
      name: 'AADDomainServicesAccountLogon'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADDomainServicesAccountManagement 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADDomainServicesAccountManagement'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADDomainServicesAccountManagement'
      name: 'AADDomainServicesAccountManagement'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADDomainServicesDirectoryServiceAccess 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADDomainServicesDirectoryServiceAccess'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADDomainServicesDirectoryServiceAccess'
      name: 'AADDomainServicesDirectoryServiceAccess'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADDomainServicesDNSAuditsDynamicUpdates 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADDomainServicesDNSAuditsDynamicUpdates'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADDomainServicesDNSAuditsDynamicUpdates'
      name: 'AADDomainServicesDNSAuditsDynamicUpdates'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADDomainServicesDNSAuditsGeneral 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADDomainServicesDNSAuditsGeneral'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADDomainServicesDNSAuditsGeneral'
      name: 'AADDomainServicesDNSAuditsGeneral'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADDomainServicesLogonLogoff 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADDomainServicesLogonLogoff'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADDomainServicesLogonLogoff'
      name: 'AADDomainServicesLogonLogoff'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADDomainServicesPolicyChange 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADDomainServicesPolicyChange'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADDomainServicesPolicyChange'
      name: 'AADDomainServicesPolicyChange'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADDomainServicesPrivilegeUse 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADDomainServicesPrivilegeUse'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADDomainServicesPrivilegeUse'
      name: 'AADDomainServicesPrivilegeUse'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADDomainServicesSystemSecurity 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADDomainServicesSystemSecurity'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADDomainServicesSystemSecurity'
      name: 'AADDomainServicesSystemSecurity'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADFirstPartyToFirstPartySignInLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADFirstPartyToFirstPartySignInLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADFirstPartyToFirstPartySignInLogs'
      name: 'AADFirstPartyToFirstPartySignInLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADGraphActivityLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADGraphActivityLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADGraphActivityLogs'
      name: 'AADGraphActivityLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADManagedIdentitySignInLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADManagedIdentitySignInLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADManagedIdentitySignInLogs'
      name: 'AADManagedIdentitySignInLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADNonInteractiveUserSignInLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADNonInteractiveUserSignInLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADNonInteractiveUserSignInLogs'
      name: 'AADNonInteractiveUserSignInLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADProvisioningLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADProvisioningLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADProvisioningLogs'
      name: 'AADProvisioningLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADRiskyServicePrincipals 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADRiskyServicePrincipals'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADRiskyServicePrincipals'
      name: 'AADRiskyServicePrincipals'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADRiskyUsers 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADRiskyUsers'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADRiskyUsers'
      name: 'AADRiskyUsers'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADServicePrincipalRiskEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADServicePrincipalRiskEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADServicePrincipalRiskEvents'
      name: 'AADServicePrincipalRiskEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADServicePrincipalSignInLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADServicePrincipalSignInLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADServicePrincipalSignInLogs'
      name: 'AADServicePrincipalSignInLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AADUserRiskEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AADUserRiskEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AADUserRiskEvents'
      name: 'AADUserRiskEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ABSBotRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ABSBotRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ABSBotRequests'
      name: 'ABSBotRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACICollaborationAudit 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACICollaborationAudit'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACICollaborationAudit'
      name: 'ACICollaborationAudit'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACLTransactionLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACLTransactionLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACLTransactionLogs'
      name: 'ACLTransactionLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACLUserDefinedLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACLUserDefinedLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACLUserDefinedLogs'
      name: 'ACLUserDefinedLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACRConnectedClientList 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACRConnectedClientList'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACRConnectedClientList'
      name: 'ACRConnectedClientList'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACREntraAuthenticationAuditLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACREntraAuthenticationAuditLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACREntraAuthenticationAuditLog'
      name: 'ACREntraAuthenticationAuditLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSAdvancedMessagingOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSAdvancedMessagingOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSAdvancedMessagingOperations'
      name: 'ACSAdvancedMessagingOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSAuthIncomingOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSAuthIncomingOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSAuthIncomingOperations'
      name: 'ACSAuthIncomingOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSBillingUsage 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSBillingUsage'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSBillingUsage'
      name: 'ACSBillingUsage'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallAutomationIncomingOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallAutomationIncomingOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallAutomationIncomingOperations'
      name: 'ACSCallAutomationIncomingOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallAutomationMediaSummary 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallAutomationMediaSummary'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallAutomationMediaSummary'
      name: 'ACSCallAutomationMediaSummary'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallAutomationStreamingUsage 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallAutomationStreamingUsage'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallAutomationStreamingUsage'
      name: 'ACSCallAutomationStreamingUsage'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallClientMediaStatsTimeSeries 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallClientMediaStatsTimeSeries'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallClientMediaStatsTimeSeries'
      name: 'ACSCallClientMediaStatsTimeSeries'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallClientOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallClientOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallClientOperations'
      name: 'ACSCallClientOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallClientServiceRequestAndOutcome 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallClientServiceRequestAndOutcome'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallClientServiceRequestAndOutcome'
      name: 'ACSCallClientServiceRequestAndOutcome'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallClosedCaptionsSummary 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallClosedCaptionsSummary'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallClosedCaptionsSummary'
      name: 'ACSCallClosedCaptionsSummary'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallDiagnostics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallDiagnostics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallDiagnostics'
      name: 'ACSCallDiagnostics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallDiagnosticsUpdates 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallDiagnosticsUpdates'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallDiagnosticsUpdates'
      name: 'ACSCallDiagnosticsUpdates'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallingMetrics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallingMetrics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallingMetrics'
      name: 'ACSCallingMetrics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallRecordingIncomingOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallRecordingIncomingOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallRecordingIncomingOperations'
      name: 'ACSCallRecordingIncomingOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallRecordingSummary 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallRecordingSummary'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallRecordingSummary'
      name: 'ACSCallRecordingSummary'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallSummary 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallSummary'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallSummary'
      name: 'ACSCallSummary'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallSummaryUpdates 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallSummaryUpdates'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallSummaryUpdates'
      name: 'ACSCallSummaryUpdates'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSCallSurvey 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSCallSurvey'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSCallSurvey'
      name: 'ACSCallSurvey'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSChatIncomingOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSChatIncomingOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSChatIncomingOperations'
      name: 'ACSChatIncomingOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSEmailSendMailOperational 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSEmailSendMailOperational'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSEmailSendMailOperational'
      name: 'ACSEmailSendMailOperational'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSEmailStatusUpdateOperational 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSEmailStatusUpdateOperational'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSEmailStatusUpdateOperational'
      name: 'ACSEmailStatusUpdateOperational'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSEmailUserEngagementOperational 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSEmailUserEngagementOperational'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSEmailUserEngagementOperational'
      name: 'ACSEmailUserEngagementOperational'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSJobRouterIncomingOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSJobRouterIncomingOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSJobRouterIncomingOperations'
      name: 'ACSJobRouterIncomingOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSOptOutManagementOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSOptOutManagementOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSOptOutManagementOperations'
      name: 'ACSOptOutManagementOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSRoomsIncomingOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSRoomsIncomingOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSRoomsIncomingOperations'
      name: 'ACSRoomsIncomingOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ACSSMSIncomingOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ACSSMSIncomingOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ACSSMSIncomingOperations'
      name: 'ACSSMSIncomingOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADAssessmentRecommendation'
      name: 'ADAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AddonAzureBackupAlerts 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AddonAzureBackupAlerts'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AddonAzureBackupAlerts'
      name: 'AddonAzureBackupAlerts'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AddonAzureBackupJobs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AddonAzureBackupJobs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AddonAzureBackupJobs'
      name: 'AddonAzureBackupJobs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AddonAzureBackupPolicy 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AddonAzureBackupPolicy'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AddonAzureBackupPolicy'
      name: 'AddonAzureBackupPolicy'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AddonAzureBackupProtectedInstance 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AddonAzureBackupProtectedInstance'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AddonAzureBackupProtectedInstance'
      name: 'AddonAzureBackupProtectedInstance'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AddonAzureBackupStorage 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AddonAzureBackupStorage'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AddonAzureBackupStorage'
      name: 'AddonAzureBackupStorage'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFActivityRun 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFActivityRun'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFActivityRun'
      name: 'ADFActivityRun'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFAirflowSchedulerLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFAirflowSchedulerLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFAirflowSchedulerLogs'
      name: 'ADFAirflowSchedulerLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFAirflowTaskLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFAirflowTaskLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFAirflowTaskLogs'
      name: 'ADFAirflowTaskLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFAirflowWebLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFAirflowWebLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFAirflowWebLogs'
      name: 'ADFAirflowWebLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFAirflowWorkerLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFAirflowWorkerLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFAirflowWorkerLogs'
      name: 'ADFAirflowWorkerLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFPipelineRun 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFPipelineRun'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFPipelineRun'
      name: 'ADFPipelineRun'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFSandboxActivityRun 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFSandboxActivityRun'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFSandboxActivityRun'
      name: 'ADFSandboxActivityRun'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFSandboxPipelineRun 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFSandboxPipelineRun'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFSandboxPipelineRun'
      name: 'ADFSandboxPipelineRun'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFSSignInLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFSSignInLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFSSignInLogs'
      name: 'ADFSSignInLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFSSISIntegrationRuntimeLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFSSISIntegrationRuntimeLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFSSISIntegrationRuntimeLogs'
      name: 'ADFSSISIntegrationRuntimeLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFSSISPackageEventMessageContext 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFSSISPackageEventMessageContext'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFSSISPackageEventMessageContext'
      name: 'ADFSSISPackageEventMessageContext'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFSSISPackageEventMessages 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFSSISPackageEventMessages'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFSSISPackageEventMessages'
      name: 'ADFSSISPackageEventMessages'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFSSISPackageExecutableStatistics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFSSISPackageExecutableStatistics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFSSISPackageExecutableStatistics'
      name: 'ADFSSISPackageExecutableStatistics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFSSISPackageExecutionComponentPhases 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFSSISPackageExecutionComponentPhases'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFSSISPackageExecutionComponentPhases'
      name: 'ADFSSISPackageExecutionComponentPhases'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFSSISPackageExecutionDataStatistics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFSSISPackageExecutionDataStatistics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFSSISPackageExecutionDataStatistics'
      name: 'ADFSSISPackageExecutionDataStatistics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADFTriggerRun 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADFTriggerRun'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADFTriggerRun'
      name: 'ADFTriggerRun'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADReplicationResult 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADReplicationResult'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADReplicationResult'
      name: 'ADReplicationResult'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADSecurityAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADSecurityAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADSecurityAssessmentRecommendation'
      name: 'ADSecurityAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADTDataHistoryOperation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADTDataHistoryOperation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADTDataHistoryOperation'
      name: 'ADTDataHistoryOperation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADTDigitalTwinsOperation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADTDigitalTwinsOperation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADTDigitalTwinsOperation'
      name: 'ADTDigitalTwinsOperation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADTEventRoutesOperation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADTEventRoutesOperation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADTEventRoutesOperation'
      name: 'ADTEventRoutesOperation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADTModelsOperation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADTModelsOperation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADTModelsOperation'
      name: 'ADTModelsOperation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADTQueryOperation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADTQueryOperation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADTQueryOperation'
      name: 'ADTQueryOperation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADXCommand 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADXCommand'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADXCommand'
      name: 'ADXCommand'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADXDataOperation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADXDataOperation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADXDataOperation'
      name: 'ADXDataOperation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADXIngestionBatching 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADXIngestionBatching'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADXIngestionBatching'
      name: 'ADXIngestionBatching'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADXJournal 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADXJournal'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADXJournal'
      name: 'ADXJournal'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADXQuery 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADXQuery'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADXQuery'
      name: 'ADXQuery'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADXTableDetails 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADXTableDetails'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADXTableDetails'
      name: 'ADXTableDetails'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ADXTableUsageStatistics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ADXTableUsageStatistics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ADXTableUsageStatistics'
      name: 'ADXTableUsageStatistics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AegDataPlaneRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AegDataPlaneRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AegDataPlaneRequests'
      name: 'AegDataPlaneRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AegDeliveryFailureLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AegDeliveryFailureLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AegDeliveryFailureLogs'
      name: 'AegDeliveryFailureLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AegPublishFailureLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AegPublishFailureLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AegPublishFailureLogs'
      name: 'AegPublishFailureLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AEWAssignmentBlobLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AEWAssignmentBlobLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AEWAssignmentBlobLogs'
      name: 'AEWAssignmentBlobLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AEWAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AEWAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AEWAuditLogs'
      name: 'AEWAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AEWComputePipelinesLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AEWComputePipelinesLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AEWComputePipelinesLogs'
      name: 'AEWComputePipelinesLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AEWExperimentAssignmentSummary 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AEWExperimentAssignmentSummary'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AEWExperimentAssignmentSummary'
      name: 'AEWExperimentAssignmentSummary'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AEWExperimentScorecardMetricPairs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AEWExperimentScorecardMetricPairs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AEWExperimentScorecardMetricPairs'
      name: 'AEWExperimentScorecardMetricPairs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AEWExperimentScorecards 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AEWExperimentScorecards'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AEWExperimentScorecards'
      name: 'AEWExperimentScorecards'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AFSAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AFSAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AFSAuditLogs'
      name: 'AFSAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AGCAccessLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AGCAccessLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AGCAccessLogs'
      name: 'AGCAccessLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AGCFirewallLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AGCFirewallLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AGCFirewallLogs'
      name: 'AGCFirewallLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AgriFoodApplicationAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AgriFoodApplicationAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AgriFoodApplicationAuditLogs'
      name: 'AgriFoodApplicationAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AgriFoodFarmManagementLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AgriFoodFarmManagementLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AgriFoodFarmManagementLogs'
      name: 'AgriFoodFarmManagementLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AgriFoodFarmOperationLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AgriFoodFarmOperationLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AgriFoodFarmOperationLogs'
      name: 'AgriFoodFarmOperationLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AgriFoodInsightLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AgriFoodInsightLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AgriFoodInsightLogs'
      name: 'AgriFoodInsightLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AgriFoodJobProcessedLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AgriFoodJobProcessedLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AgriFoodJobProcessedLogs'
      name: 'AgriFoodJobProcessedLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AgriFoodModelInferenceLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AgriFoodModelInferenceLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AgriFoodModelInferenceLogs'
      name: 'AgriFoodModelInferenceLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AgriFoodProviderAuthLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AgriFoodProviderAuthLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AgriFoodProviderAuthLogs'
      name: 'AgriFoodProviderAuthLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AgriFoodSatelliteLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AgriFoodSatelliteLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AgriFoodSatelliteLogs'
      name: 'AgriFoodSatelliteLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AgriFoodSensorManagementLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AgriFoodSensorManagementLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AgriFoodSensorManagementLogs'
      name: 'AgriFoodSensorManagementLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AgriFoodWeatherLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AgriFoodWeatherLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AgriFoodWeatherLogs'
      name: 'AgriFoodWeatherLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AGSGrafanaLoginEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AGSGrafanaLoginEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AGSGrafanaLoginEvents'
      name: 'AGSGrafanaLoginEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AGSGrafanaUsageInsightsEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AGSGrafanaUsageInsightsEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AGSGrafanaUsageInsightsEvents'
      name: 'AGSGrafanaUsageInsightsEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AGSUpdateEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AGSUpdateEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AGSUpdateEvents'
      name: 'AGSUpdateEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AGWAccessLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AGWAccessLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AGWAccessLogs'
      name: 'AGWAccessLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AGWFirewallLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AGWFirewallLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AGWFirewallLogs'
      name: 'AGWFirewallLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AGWPerformanceLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AGWPerformanceLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AGWPerformanceLogs'
      name: 'AGWPerformanceLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AHCIDiagnosticLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AHCIDiagnosticLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AHCIDiagnosticLogs'
      name: 'AHCIDiagnosticLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AHDSDeidAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AHDSDeidAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AHDSDeidAuditLogs'
      name: 'AHDSDeidAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AHDSDicomAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AHDSDicomAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AHDSDicomAuditLogs'
      name: 'AHDSDicomAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AHDSDicomDiagnosticLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AHDSDicomDiagnosticLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AHDSDicomDiagnosticLogs'
      name: 'AHDSDicomDiagnosticLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AHDSMedTechDiagnosticLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AHDSMedTechDiagnosticLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AHDSMedTechDiagnosticLogs'
      name: 'AHDSMedTechDiagnosticLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AirflowDagProcessingLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AirflowDagProcessingLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AirflowDagProcessingLogs'
      name: 'AirflowDagProcessingLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AKSAudit 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AKSAudit'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AKSAudit'
      name: 'AKSAudit'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AKSAuditAdmin 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AKSAuditAdmin'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AKSAuditAdmin'
      name: 'AKSAuditAdmin'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AKSControlPlane 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AKSControlPlane'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AKSControlPlane'
      name: 'AKSControlPlane'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ALBHealthEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ALBHealthEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ALBHealthEvent'
      name: 'ALBHealthEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_Alert 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'Alert'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'Alert'
      name: 'Alert'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlComputeClusterEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlComputeClusterEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlComputeClusterEvent'
      name: 'AmlComputeClusterEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlComputeClusterNodeEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlComputeClusterNodeEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlComputeClusterNodeEvent'
      name: 'AmlComputeClusterNodeEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlComputeCpuGpuUtilization 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlComputeCpuGpuUtilization'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlComputeCpuGpuUtilization'
      name: 'AmlComputeCpuGpuUtilization'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlComputeInstanceEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlComputeInstanceEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlComputeInstanceEvent'
      name: 'AmlComputeInstanceEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlComputeJobEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlComputeJobEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlComputeJobEvent'
      name: 'AmlComputeJobEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlDataLabelEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlDataLabelEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlDataLabelEvent'
      name: 'AmlDataLabelEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlDataSetEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlDataSetEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlDataSetEvent'
      name: 'AmlDataSetEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlDataStoreEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlDataStoreEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlDataStoreEvent'
      name: 'AmlDataStoreEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlDeploymentEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlDeploymentEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlDeploymentEvent'
      name: 'AmlDeploymentEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlEnvironmentEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlEnvironmentEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlEnvironmentEvent'
      name: 'AmlEnvironmentEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlInferencingEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlInferencingEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlInferencingEvent'
      name: 'AmlInferencingEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlModelsEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlModelsEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlModelsEvent'
      name: 'AmlModelsEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlOnlineEndpointConsoleLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlOnlineEndpointConsoleLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlOnlineEndpointConsoleLog'
      name: 'AmlOnlineEndpointConsoleLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlOnlineEndpointEventLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlOnlineEndpointEventLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlOnlineEndpointEventLog'
      name: 'AmlOnlineEndpointEventLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlOnlineEndpointTrafficLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlOnlineEndpointTrafficLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlOnlineEndpointTrafficLog'
      name: 'AmlOnlineEndpointTrafficLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlPipelineEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlPipelineEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlPipelineEvent'
      name: 'AmlPipelineEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlRegistryReadEventsLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlRegistryReadEventsLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlRegistryReadEventsLog'
      name: 'AmlRegistryReadEventsLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlRegistryWriteEventsLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlRegistryWriteEventsLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlRegistryWriteEventsLog'
      name: 'AmlRegistryWriteEventsLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlRunEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlRunEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlRunEvent'
      name: 'AmlRunEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AmlRunStatusChangedEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AmlRunStatusChangedEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AmlRunStatusChangedEvent'
      name: 'AmlRunStatusChangedEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AMSKeyDeliveryRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AMSKeyDeliveryRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AMSKeyDeliveryRequests'
      name: 'AMSKeyDeliveryRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AMSLiveEventOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AMSLiveEventOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AMSLiveEventOperations'
      name: 'AMSLiveEventOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AMSMediaAccountHealth 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AMSMediaAccountHealth'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AMSMediaAccountHealth'
      name: 'AMSMediaAccountHealth'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AMSStreamingEndpointRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AMSStreamingEndpointRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AMSStreamingEndpointRequests'
      name: 'AMSStreamingEndpointRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AMWMetricsUsageDetails 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AMWMetricsUsageDetails'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AMWMetricsUsageDetails'
      name: 'AMWMetricsUsageDetails'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ANFFileAccess 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ANFFileAccess'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ANFFileAccess'
      name: 'ANFFileAccess'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AOIDatabaseQuery 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AOIDatabaseQuery'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AOIDatabaseQuery'
      name: 'AOIDatabaseQuery'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AOIDigestion 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AOIDigestion'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AOIDigestion'
      name: 'AOIDigestion'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AOIStorage 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AOIStorage'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AOIStorage'
      name: 'AOIStorage'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ApiManagementGatewayLlmLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ApiManagementGatewayLlmLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ApiManagementGatewayLlmLog'
      name: 'ApiManagementGatewayLlmLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ApiManagementGatewayLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ApiManagementGatewayLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ApiManagementGatewayLogs'
      name: 'ApiManagementGatewayLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ApiManagementGatewayMCPLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ApiManagementGatewayMCPLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ApiManagementGatewayMCPLog'
      name: 'ApiManagementGatewayMCPLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ApiManagementWebSocketConnectionLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ApiManagementWebSocketConnectionLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ApiManagementWebSocketConnectionLogs'
      name: 'ApiManagementWebSocketConnectionLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_APIMDevPortalAuditDiagnosticLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'APIMDevPortalAuditDiagnosticLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'APIMDevPortalAuditDiagnosticLog'
      name: 'APIMDevPortalAuditDiagnosticLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppAvailabilityResults 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppAvailabilityResults'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AppAvailabilityResults'
      name: 'AppAvailabilityResults'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppBrowserTimings 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppBrowserTimings'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AppBrowserTimings'
      name: 'AppBrowserTimings'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppCenterError 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppCenterError'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppCenterError'
      name: 'AppCenterError'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppDependencies 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppDependencies'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AppDependencies'
      name: 'AppDependencies'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppEnvSessionConsoleLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppEnvSessionConsoleLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppEnvSessionConsoleLogs'
      name: 'AppEnvSessionConsoleLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppEnvSessionLifecycleLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppEnvSessionLifecycleLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppEnvSessionLifecycleLogs'
      name: 'AppEnvSessionLifecycleLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppEnvSessionPoolEventLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppEnvSessionPoolEventLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppEnvSessionPoolEventLogs'
      name: 'AppEnvSessionPoolEventLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppEnvSpringAppConsoleLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppEnvSpringAppConsoleLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppEnvSpringAppConsoleLogs'
      name: 'AppEnvSpringAppConsoleLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AppEvents'
      name: 'AppEvents'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppExceptions 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppExceptions'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AppExceptions'
      name: 'AppExceptions'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppGenAIContent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppGenAIContent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppGenAIContent'
      name: 'AppGenAIContent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppMetrics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppMetrics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AppMetrics'
      name: 'AppMetrics'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppPageViews 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppPageViews'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AppPageViews'
      name: 'AppPageViews'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppPerformanceCounters 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppPerformanceCounters'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AppPerformanceCounters'
      name: 'AppPerformanceCounters'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppPlatformBuildLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppPlatformBuildLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppPlatformBuildLogs'
      name: 'AppPlatformBuildLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppPlatformContainerEventLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppPlatformContainerEventLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppPlatformContainerEventLogs'
      name: 'AppPlatformContainerEventLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppPlatformIngressLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppPlatformIngressLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppPlatformIngressLogs'
      name: 'AppPlatformIngressLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppPlatformLogsforSpring 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppPlatformLogsforSpring'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppPlatformLogsforSpring'
      name: 'AppPlatformLogsforSpring'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppPlatformSystemLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppPlatformSystemLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppPlatformSystemLogs'
      name: 'AppPlatformSystemLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AppRequests'
      name: 'AppRequests'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppServiceAntivirusScanAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppServiceAntivirusScanAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppServiceAntivirusScanAuditLogs'
      name: 'AppServiceAntivirusScanAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppServiceAppLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppServiceAppLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppServiceAppLogs'
      name: 'AppServiceAppLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppServiceAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppServiceAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppServiceAuditLogs'
      name: 'AppServiceAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppServiceAuthenticationLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppServiceAuthenticationLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppServiceAuthenticationLogs'
      name: 'AppServiceAuthenticationLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppServiceConsoleLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppServiceConsoleLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppServiceConsoleLogs'
      name: 'AppServiceConsoleLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppServiceEnvironmentPlatformLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppServiceEnvironmentPlatformLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppServiceEnvironmentPlatformLogs'
      name: 'AppServiceEnvironmentPlatformLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppServiceFileAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppServiceFileAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppServiceFileAuditLogs'
      name: 'AppServiceFileAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppServiceHTTPLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppServiceHTTPLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppServiceHTTPLogs'
      name: 'AppServiceHTTPLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppServiceIPSecAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppServiceIPSecAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppServiceIPSecAuditLogs'
      name: 'AppServiceIPSecAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppServicePlatformLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppServicePlatformLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppServicePlatformLogs'
      name: 'AppServicePlatformLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppServiceServerlessSecurityPluginData 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppServiceServerlessSecurityPluginData'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AppServiceServerlessSecurityPluginData'
      name: 'AppServiceServerlessSecurityPluginData'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppSystemEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppSystemEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AppSystemEvents'
      name: 'AppSystemEvents'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AppTraces 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AppTraces'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AppTraces'
      name: 'AppTraces'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ArcK8sAudit 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ArcK8sAudit'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ArcK8sAudit'
      name: 'ArcK8sAudit'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ArcK8sAuditAdmin 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ArcK8sAuditAdmin'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ArcK8sAuditAdmin'
      name: 'ArcK8sAuditAdmin'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ArcK8sControlPlane 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ArcK8sControlPlane'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ArcK8sControlPlane'
      name: 'ArcK8sControlPlane'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ASCAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ASCAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ASCAuditLogs'
      name: 'ASCAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ASCDeviceEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ASCDeviceEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ASCDeviceEvents'
      name: 'ASCDeviceEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ASRJobs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ASRJobs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ASRJobs'
      name: 'ASRJobs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ASRReplicatedItems 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ASRReplicatedItems'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ASRReplicatedItems'
      name: 'ASRReplicatedItems'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ASRv2HealthEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ASRv2HealthEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ASRv2HealthEvents'
      name: 'ASRv2HealthEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ASRv2JobEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ASRv2JobEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ASRv2JobEvents'
      name: 'ASRv2JobEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ASRv2ProtectedItems 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ASRv2ProtectedItems'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ASRv2ProtectedItems'
      name: 'ASRv2ProtectedItems'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ASRv2ReplicationExtensions 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ASRv2ReplicationExtensions'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ASRv2ReplicationExtensions'
      name: 'ASRv2ReplicationExtensions'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ASRv2ReplicationPolicies 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ASRv2ReplicationPolicies'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ASRv2ReplicationPolicies'
      name: 'ASRv2ReplicationPolicies'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ASRv2ReplicationVaults 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ASRv2ReplicationVaults'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ASRv2ReplicationVaults'
      name: 'ASRv2ReplicationVaults'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ATCExpressRouteCircuitIpfix 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ATCExpressRouteCircuitIpfix'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ATCExpressRouteCircuitIpfix'
      name: 'ATCExpressRouteCircuitIpfix'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ATCMicrosoftPeeringMetadata 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ATCMicrosoftPeeringMetadata'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ATCMicrosoftPeeringMetadata'
      name: 'ATCMicrosoftPeeringMetadata'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ATCPrivatePeeringMetadata 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ATCPrivatePeeringMetadata'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ATCPrivatePeeringMetadata'
      name: 'ATCPrivatePeeringMetadata'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AuditLogs'
      name: 'AuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AutoscaleEvaluationsLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AutoscaleEvaluationsLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AutoscaleEvaluationsLog'
      name: 'AutoscaleEvaluationsLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AutoscaleScaleActionsLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AutoscaleScaleActionsLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AutoscaleScaleActionsLog'
      name: 'AutoscaleScaleActionsLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AVNMConnectivityConfigurationChange 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AVNMConnectivityConfigurationChange'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AVNMConnectivityConfigurationChange'
      name: 'AVNMConnectivityConfigurationChange'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AVNMIPAMPoolAllocationChange 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AVNMIPAMPoolAllocationChange'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AVNMIPAMPoolAllocationChange'
      name: 'AVNMIPAMPoolAllocationChange'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AVNMNetworkGroupMembershipChange 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AVNMNetworkGroupMembershipChange'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AVNMNetworkGroupMembershipChange'
      name: 'AVNMNetworkGroupMembershipChange'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AVNMRuleCollectionChange 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AVNMRuleCollectionChange'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AVNMRuleCollectionChange'
      name: 'AVNMRuleCollectionChange'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AVSEsxiFirewallSyslog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AVSEsxiFirewallSyslog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AVSEsxiFirewallSyslog'
      name: 'AVSEsxiFirewallSyslog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AVSEsxiSyslog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AVSEsxiSyslog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AVSEsxiSyslog'
      name: 'AVSEsxiSyslog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AVSNsxEdgeSyslog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AVSNsxEdgeSyslog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AVSNsxEdgeSyslog'
      name: 'AVSNsxEdgeSyslog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AVSNsxManagerSyslog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AVSNsxManagerSyslog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AVSNsxManagerSyslog'
      name: 'AVSNsxManagerSyslog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AVSSyslog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AVSSyslog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AVSSyslog'
      name: 'AVSSyslog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AVSVcSyslog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AVSVcSyslog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AVSVcSyslog'
      name: 'AVSVcSyslog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWApplicationRule 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWApplicationRule'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWApplicationRule'
      name: 'AZFWApplicationRule'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWApplicationRuleAggregation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWApplicationRuleAggregation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWApplicationRuleAggregation'
      name: 'AZFWApplicationRuleAggregation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWDnsFlowTrace 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWDnsFlowTrace'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWDnsFlowTrace'
      name: 'AZFWDnsFlowTrace'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWDnsQuery 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWDnsQuery'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWDnsQuery'
      name: 'AZFWDnsQuery'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWFatFlow 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWFatFlow'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWFatFlow'
      name: 'AZFWFatFlow'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWFlowTrace 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWFlowTrace'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWFlowTrace'
      name: 'AZFWFlowTrace'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWIdpsSignature 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWIdpsSignature'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWIdpsSignature'
      name: 'AZFWIdpsSignature'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWInternalFqdnResolutionFailure 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWInternalFqdnResolutionFailure'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWInternalFqdnResolutionFailure'
      name: 'AZFWInternalFqdnResolutionFailure'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWNatRule 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWNatRule'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWNatRule'
      name: 'AZFWNatRule'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWNatRuleAggregation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWNatRuleAggregation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWNatRuleAggregation'
      name: 'AZFWNatRuleAggregation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWNetworkRule 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWNetworkRule'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWNetworkRule'
      name: 'AZFWNetworkRule'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWNetworkRuleAggregation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWNetworkRuleAggregation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWNetworkRuleAggregation'
      name: 'AZFWNetworkRuleAggregation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZFWThreatIntel 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZFWThreatIntel'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZFWThreatIntel'
      name: 'AZFWThreatIntel'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZKVAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZKVAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZKVAuditLogs'
      name: 'AZKVAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZKVPolicyEvaluationDetailsLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZKVPolicyEvaluationDetailsLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZKVPolicyEvaluationDetailsLogs'
      name: 'AZKVPolicyEvaluationDetailsLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZMSApplicationMetricLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZMSApplicationMetricLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZMSApplicationMetricLogs'
      name: 'AZMSApplicationMetricLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZMSArchiveLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZMSArchiveLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZMSArchiveLogs'
      name: 'AZMSArchiveLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZMSAutoscaleLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZMSAutoscaleLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZMSAutoscaleLogs'
      name: 'AZMSAutoscaleLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZMSCustomerManagedKeyUserLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZMSCustomerManagedKeyUserLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZMSCustomerManagedKeyUserLogs'
      name: 'AZMSCustomerManagedKeyUserLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZMSDiagnosticErrorLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZMSDiagnosticErrorLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZMSDiagnosticErrorLogs'
      name: 'AZMSDiagnosticErrorLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZMSHybridConnectionsEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZMSHybridConnectionsEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZMSHybridConnectionsEvents'
      name: 'AZMSHybridConnectionsEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZMSKafkaCoordinatorLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZMSKafkaCoordinatorLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZMSKafkaCoordinatorLogs'
      name: 'AZMSKafkaCoordinatorLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZMSKafkaUserErrorLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZMSKafkaUserErrorLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZMSKafkaUserErrorLogs'
      name: 'AZMSKafkaUserErrorLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZMSOperationalLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZMSOperationalLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZMSOperationalLogs'
      name: 'AZMSOperationalLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZMSRunTimeAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZMSRunTimeAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZMSRunTimeAuditLogs'
      name: 'AZMSRunTimeAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AZMSVnetConnectionEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AZMSVnetConnectionEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AZMSVnetConnectionEvents'
      name: 'AZMSVnetConnectionEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AzureActivity 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AzureActivity'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'AzureActivity'
      name: 'AzureActivity'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AzureActivityV2 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AzureActivityV2'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AzureActivityV2'
      name: 'AzureActivityV2'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AzureAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AzureAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AzureAssessmentRecommendation'
      name: 'AzureAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AzureAttestationDiagnostics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AzureAttestationDiagnostics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AzureAttestationDiagnostics'
      name: 'AzureAttestationDiagnostics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AzureBackupOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AzureBackupOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AzureBackupOperations'
      name: 'AzureBackupOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AzureDevOpsAuditing 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AzureDevOpsAuditing'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AzureDevOpsAuditing'
      name: 'AzureDevOpsAuditing'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AzureLoadTestingOperation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AzureLoadTestingOperation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AzureLoadTestingOperation'
      name: 'AzureLoadTestingOperation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AzureMetrics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AzureMetrics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AzureMetrics'
      name: 'AzureMetrics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AzureMetricsV2 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AzureMetricsV2'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AzureMetricsV2'
      name: 'AzureMetricsV2'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_AzureMonitorPipelineLogErrors 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'AzureMonitorPipelineLogErrors'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'AzureMonitorPipelineLogErrors'
      name: 'AzureMonitorPipelineLogErrors'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_BehaviorEntities 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'BehaviorEntities'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'BehaviorEntities'
      name: 'BehaviorEntities'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_BehaviorInfo 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'BehaviorInfo'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'BehaviorInfo'
      name: 'BehaviorInfo'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_BlockchainApplicationLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'BlockchainApplicationLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'BlockchainApplicationLog'
      name: 'BlockchainApplicationLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_BlockchainProxyLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'BlockchainProxyLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'BlockchainProxyLog'
      name: 'BlockchainProxyLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CassandraAudit 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CassandraAudit'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CassandraAudit'
      name: 'CassandraAudit'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CassandraLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CassandraLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CassandraLogs'
      name: 'CassandraLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CCFApplicationLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CCFApplicationLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CCFApplicationLogs'
      name: 'CCFApplicationLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CDBCassandraRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CDBCassandraRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CDBCassandraRequests'
      name: 'CDBCassandraRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CDBControlPlaneRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CDBControlPlaneRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CDBControlPlaneRequests'
      name: 'CDBControlPlaneRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CDBDataPlaneRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CDBDataPlaneRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CDBDataPlaneRequests'
      name: 'CDBDataPlaneRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CDBDataPlaneRequests15M 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CDBDataPlaneRequests15M'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CDBDataPlaneRequests15M'
      name: 'CDBDataPlaneRequests15M'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CDBDataPlaneRequests5M 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CDBDataPlaneRequests5M'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CDBDataPlaneRequests5M'
      name: 'CDBDataPlaneRequests5M'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CDBGremlinRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CDBGremlinRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CDBGremlinRequests'
      name: 'CDBGremlinRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CDBMongoRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CDBMongoRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CDBMongoRequests'
      name: 'CDBMongoRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CDBPartitionKeyRUConsumption 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CDBPartitionKeyRUConsumption'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CDBPartitionKeyRUConsumption'
      name: 'CDBPartitionKeyRUConsumption'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CDBPartitionKeyStatistics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CDBPartitionKeyStatistics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CDBPartitionKeyStatistics'
      name: 'CDBPartitionKeyStatistics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CDBQueryRuntimeStatistics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CDBQueryRuntimeStatistics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CDBQueryRuntimeStatistics'
      name: 'CDBQueryRuntimeStatistics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CDBTableApiRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CDBTableApiRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CDBTableApiRequests'
      name: 'CDBTableApiRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ChaosStudioExperimentEventLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ChaosStudioExperimentEventLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ChaosStudioExperimentEventLogs'
      name: 'ChaosStudioExperimentEventLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CHSMServiceOperationAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CHSMServiceOperationAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CHSMServiceOperationAuditLogs'
      name: 'CHSMServiceOperationAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CIEventsAudit 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CIEventsAudit'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CIEventsAudit'
      name: 'CIEventsAudit'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CIEventsOperational 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CIEventsOperational'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CIEventsOperational'
      name: 'CIEventsOperational'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CloudHsmServiceOperationAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CloudHsmServiceOperationAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CloudHsmServiceOperationAuditLogs'
      name: 'CloudHsmServiceOperationAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ComputerGroup 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ComputerGroup'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ComputerGroup'
      name: 'ComputerGroup'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerAppConsoleLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerAppConsoleLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerAppConsoleLogs'
      name: 'ContainerAppConsoleLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerAppSystemLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerAppSystemLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerAppSystemLogs'
      name: 'ContainerAppSystemLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerEvent'
      name: 'ContainerEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerImageInventory 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerImageInventory'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerImageInventory'
      name: 'ContainerImageInventory'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerInstanceLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerInstanceLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerInstanceLog'
      name: 'ContainerInstanceLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerInventory 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerInventory'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerInventory'
      name: 'ContainerInventory'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerLog'
      name: 'ContainerLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerLogV2 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerLogV2'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerLogV2'
      name: 'ContainerLogV2'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerNetworkLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerNetworkLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerNetworkLogs'
      name: 'ContainerNetworkLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerNodeInventory 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerNodeInventory'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerNodeInventory'
      name: 'ContainerNodeInventory'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerRegistryLoginEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerRegistryLoginEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerRegistryLoginEvents'
      name: 'ContainerRegistryLoginEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerRegistryRepositoryEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerRegistryRepositoryEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerRegistryRepositoryEvents'
      name: 'ContainerRegistryRepositoryEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ContainerServiceLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ContainerServiceLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ContainerServiceLog'
      name: 'ContainerServiceLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_CoreAzureBackup 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'CoreAzureBackup'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'CoreAzureBackup'
      name: 'CoreAzureBackup'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksAccounts 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksAccounts'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksAccounts'
      name: 'DatabricksAccounts'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksApps 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksApps'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksApps'
      name: 'DatabricksApps'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksBrickStoreHttpGateway 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksBrickStoreHttpGateway'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksBrickStoreHttpGateway'
      name: 'DatabricksBrickStoreHttpGateway'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksBudgetPolicyCentral 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksBudgetPolicyCentral'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksBudgetPolicyCentral'
      name: 'DatabricksBudgetPolicyCentral'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksCapsule8Dataplane 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksCapsule8Dataplane'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksCapsule8Dataplane'
      name: 'DatabricksCapsule8Dataplane'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksClamAVScan 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksClamAVScan'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksClamAVScan'
      name: 'DatabricksClamAVScan'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksCloudStorageMetadata 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksCloudStorageMetadata'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksCloudStorageMetadata'
      name: 'DatabricksCloudStorageMetadata'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksClusterLibraries 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksClusterLibraries'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksClusterLibraries'
      name: 'DatabricksClusterLibraries'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksClusterPolicies 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksClusterPolicies'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksClusterPolicies'
      name: 'DatabricksClusterPolicies'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksClusters 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksClusters'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksClusters'
      name: 'DatabricksClusters'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksDashboards 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksDashboards'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksDashboards'
      name: 'DatabricksDashboards'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksDatabricksSQL 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksDatabricksSQL'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksDatabricksSQL'
      name: 'DatabricksDatabricksSQL'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksDataMonitoring 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksDataMonitoring'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksDataMonitoring'
      name: 'DatabricksDataMonitoring'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksDataRooms 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksDataRooms'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksDataRooms'
      name: 'DatabricksDataRooms'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksDBFS 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksDBFS'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksDBFS'
      name: 'DatabricksDBFS'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksDeltaPipelines 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksDeltaPipelines'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksDeltaPipelines'
      name: 'DatabricksDeltaPipelines'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksFeatureStore 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksFeatureStore'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksFeatureStore'
      name: 'DatabricksFeatureStore'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksFiles 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksFiles'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksFiles'
      name: 'DatabricksFiles'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksFilesystem 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksFilesystem'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksFilesystem'
      name: 'DatabricksFilesystem'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksGenie 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksGenie'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksGenie'
      name: 'DatabricksGenie'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksGitCredentials 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksGitCredentials'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksGitCredentials'
      name: 'DatabricksGitCredentials'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksGlobalInitScripts 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksGlobalInitScripts'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksGlobalInitScripts'
      name: 'DatabricksGlobalInitScripts'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksGroups 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksGroups'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksGroups'
      name: 'DatabricksGroups'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksIAMRole 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksIAMRole'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksIAMRole'
      name: 'DatabricksIAMRole'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksIngestion 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksIngestion'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksIngestion'
      name: 'DatabricksIngestion'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksInstancePools 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksInstancePools'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksInstancePools'
      name: 'DatabricksInstancePools'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksJobs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksJobs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksJobs'
      name: 'DatabricksJobs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksLakeviewConfig 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksLakeviewConfig'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksLakeviewConfig'
      name: 'DatabricksLakeviewConfig'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksLineageTracking 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksLineageTracking'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksLineageTracking'
      name: 'DatabricksLineageTracking'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksMarketplaceConsumer 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksMarketplaceConsumer'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksMarketplaceConsumer'
      name: 'DatabricksMarketplaceConsumer'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksMarketplaceProvider 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksMarketplaceProvider'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksMarketplaceProvider'
      name: 'DatabricksMarketplaceProvider'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksMLflowAcledArtifact 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksMLflowAcledArtifact'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksMLflowAcledArtifact'
      name: 'DatabricksMLflowAcledArtifact'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksMLflowExperiment 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksMLflowExperiment'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksMLflowExperiment'
      name: 'DatabricksMLflowExperiment'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksModelRegistry 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksModelRegistry'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksModelRegistry'
      name: 'DatabricksModelRegistry'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksNotebook 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksNotebook'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksNotebook'
      name: 'DatabricksNotebook'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksOnlineTables 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksOnlineTables'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksOnlineTables'
      name: 'DatabricksOnlineTables'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksPartnerHub 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksPartnerHub'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksPartnerHub'
      name: 'DatabricksPartnerHub'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksPredictiveOptimization 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksPredictiveOptimization'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksPredictiveOptimization'
      name: 'DatabricksPredictiveOptimization'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksRBAC 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksRBAC'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksRBAC'
      name: 'DatabricksRBAC'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksRemoteHistoryService 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksRemoteHistoryService'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksRemoteHistoryService'
      name: 'DatabricksRemoteHistoryService'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksRepos 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksRepos'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksRepos'
      name: 'DatabricksRepos'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksRFA 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksRFA'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksRFA'
      name: 'DatabricksRFA'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksSecrets 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksSecrets'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksSecrets'
      name: 'DatabricksSecrets'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksServerlessRealTimeInference 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksServerlessRealTimeInference'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksServerlessRealTimeInference'
      name: 'DatabricksServerlessRealTimeInference'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksSQL 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksSQL'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksSQL'
      name: 'DatabricksSQL'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksSQLPermissions 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksSQLPermissions'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksSQLPermissions'
      name: 'DatabricksSQLPermissions'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksSSH 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksSSH'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksSSH'
      name: 'DatabricksSSH'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksTables 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksTables'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksTables'
      name: 'DatabricksTables'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksUnityCatalog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksUnityCatalog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksUnityCatalog'
      name: 'DatabricksUnityCatalog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksVectorSearch 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksVectorSearch'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksVectorSearch'
      name: 'DatabricksVectorSearch'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksWebhookNotifications 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksWebhookNotifications'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksWebhookNotifications'
      name: 'DatabricksWebhookNotifications'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksWebTerminal 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksWebTerminal'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksWebTerminal'
      name: 'DatabricksWebTerminal'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksWorkspace 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksWorkspace'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksWorkspace'
      name: 'DatabricksWorkspace'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DatabricksWorkspaceFiles 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DatabricksWorkspaceFiles'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DatabricksWorkspaceFiles'
      name: 'DatabricksWorkspaceFiles'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DataSetOutput 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DataSetOutput'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DataSetOutput'
      name: 'DataSetOutput'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DataSetRuns 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DataSetRuns'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DataSetRuns'
      name: 'DataSetRuns'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DataTransferOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DataTransferOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DataTransferOperations'
      name: 'DataTransferOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DCRLogErrors 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DCRLogErrors'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DCRLogErrors'
      name: 'DCRLogErrors'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DCRLogTroubleshooting 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DCRLogTroubleshooting'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DCRLogTroubleshooting'
      name: 'DCRLogTroubleshooting'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DevCenterAgentHealthLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DevCenterAgentHealthLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DevCenterAgentHealthLogs'
      name: 'DevCenterAgentHealthLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DevCenterBillingEventLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DevCenterBillingEventLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DevCenterBillingEventLogs'
      name: 'DevCenterBillingEventLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DevCenterConnectionLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DevCenterConnectionLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DevCenterConnectionLogs'
      name: 'DevCenterConnectionLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DevCenterDiagnosticLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DevCenterDiagnosticLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DevCenterDiagnosticLogs'
      name: 'DevCenterDiagnosticLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DevCenterResourceOperationLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DevCenterResourceOperationLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DevCenterResourceOperationLogs'
      name: 'DevCenterResourceOperationLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DeviceBehaviorEntities 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DeviceBehaviorEntities'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DeviceBehaviorEntities'
      name: 'DeviceBehaviorEntities'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DeviceBehaviorInfo 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DeviceBehaviorInfo'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DeviceBehaviorInfo'
      name: 'DeviceBehaviorInfo'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DeviceCustomFileEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DeviceCustomFileEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DeviceCustomFileEvents'
      name: 'DeviceCustomFileEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DeviceCustomImageLoadEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DeviceCustomImageLoadEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DeviceCustomImageLoadEvents'
      name: 'DeviceCustomImageLoadEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DeviceCustomNetworkEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DeviceCustomNetworkEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DeviceCustomNetworkEvents'
      name: 'DeviceCustomNetworkEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DeviceCustomProcessEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DeviceCustomProcessEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DeviceCustomProcessEvents'
      name: 'DeviceCustomProcessEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DeviceCustomScriptEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DeviceCustomScriptEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DeviceCustomScriptEvents'
      name: 'DeviceCustomScriptEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DNSQueryLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DNSQueryLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DNSQueryLogs'
      name: 'DNSQueryLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DSMAzureBlobStorageLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DSMAzureBlobStorageLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DSMAzureBlobStorageLogs'
      name: 'DSMAzureBlobStorageLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DSMDataClassificationLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DSMDataClassificationLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DSMDataClassificationLogs'
      name: 'DSMDataClassificationLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DSMDataLabelingLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DSMDataLabelingLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DSMDataLabelingLogs'
      name: 'DSMDataLabelingLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_DurableTaskSchedulerLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'DurableTaskSchedulerLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'DurableTaskSchedulerLogs'
      name: 'DurableTaskSchedulerLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_EdgeActionConsoleLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'EdgeActionConsoleLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'EdgeActionConsoleLog'
      name: 'EdgeActionConsoleLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_EdgeActionServiceLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'EdgeActionServiceLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'EdgeActionServiceLog'
      name: 'EdgeActionServiceLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_EGNFailedHttpDataPlaneOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'EGNFailedHttpDataPlaneOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'EGNFailedHttpDataPlaneOperations'
      name: 'EGNFailedHttpDataPlaneOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_EGNFailedMqttConnections 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'EGNFailedMqttConnections'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'EGNFailedMqttConnections'
      name: 'EGNFailedMqttConnections'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_EGNFailedMqttPublishedMessages 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'EGNFailedMqttPublishedMessages'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'EGNFailedMqttPublishedMessages'
      name: 'EGNFailedMqttPublishedMessages'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_EGNFailedMqttSubscriptions 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'EGNFailedMqttSubscriptions'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'EGNFailedMqttSubscriptions'
      name: 'EGNFailedMqttSubscriptions'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_EGNMqttDisconnections 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'EGNMqttDisconnections'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'EGNMqttDisconnections'
      name: 'EGNMqttDisconnections'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_EGNSuccessfulHttpDataPlaneOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'EGNSuccessfulHttpDataPlaneOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'EGNSuccessfulHttpDataPlaneOperations'
      name: 'EGNSuccessfulHttpDataPlaneOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_EGNSuccessfulMqttConnections 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'EGNSuccessfulMqttConnections'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'EGNSuccessfulMqttConnections'
      name: 'EGNSuccessfulMqttConnections'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_EnrichedMicrosoft365AuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'EnrichedMicrosoft365AuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'EnrichedMicrosoft365AuditLogs'
      name: 'EnrichedMicrosoft365AuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ETWEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ETWEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ETWEvent'
      name: 'ETWEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_Event 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'Event'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'Event'
      name: 'Event'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ExchangeAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ExchangeAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ExchangeAssessmentRecommendation'
      name: 'ExchangeAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ExchangeOnlineAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ExchangeOnlineAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ExchangeOnlineAssessmentRecommendation'
      name: 'ExchangeOnlineAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_FailedIngestion 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'FailedIngestion'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'FailedIngestion'
      name: 'FailedIngestion'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_FunctionAppLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'FunctionAppLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'FunctionAppLogs'
      name: 'FunctionAppLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightAmbariClusterAlerts 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightAmbariClusterAlerts'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightAmbariClusterAlerts'
      name: 'HDInsightAmbariClusterAlerts'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightAmbariSystemMetrics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightAmbariSystemMetrics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightAmbariSystemMetrics'
      name: 'HDInsightAmbariSystemMetrics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightGatewayAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightGatewayAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightGatewayAuditLogs'
      name: 'HDInsightGatewayAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightHadoopAndYarnLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightHadoopAndYarnLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightHadoopAndYarnLogs'
      name: 'HDInsightHadoopAndYarnLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightHadoopAndYarnMetrics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightHadoopAndYarnMetrics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightHadoopAndYarnMetrics'
      name: 'HDInsightHadoopAndYarnMetrics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightHBaseLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightHBaseLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightHBaseLogs'
      name: 'HDInsightHBaseLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightHBaseMetrics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightHBaseMetrics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightHBaseMetrics'
      name: 'HDInsightHBaseMetrics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightHiveAndLLAPLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightHiveAndLLAPLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightHiveAndLLAPLogs'
      name: 'HDInsightHiveAndLLAPLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightHiveAndLLAPMetrics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightHiveAndLLAPMetrics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightHiveAndLLAPMetrics'
      name: 'HDInsightHiveAndLLAPMetrics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightHiveQueryAppStats 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightHiveQueryAppStats'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightHiveQueryAppStats'
      name: 'HDInsightHiveQueryAppStats'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightHiveTezAppStats 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightHiveTezAppStats'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightHiveTezAppStats'
      name: 'HDInsightHiveTezAppStats'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightJupyterNotebookEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightJupyterNotebookEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightJupyterNotebookEvents'
      name: 'HDInsightJupyterNotebookEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightKafkaLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightKafkaLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightKafkaLogs'
      name: 'HDInsightKafkaLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightKafkaMetrics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightKafkaMetrics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightKafkaMetrics'
      name: 'HDInsightKafkaMetrics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightKafkaServerLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightKafkaServerLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightKafkaServerLog'
      name: 'HDInsightKafkaServerLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightOozieLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightOozieLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightOozieLogs'
      name: 'HDInsightOozieLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightRangerAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightRangerAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightRangerAuditLogs'
      name: 'HDInsightRangerAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSecurityLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSecurityLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSecurityLogs'
      name: 'HDInsightSecurityLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSparkApplicationEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSparkApplicationEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSparkApplicationEvents'
      name: 'HDInsightSparkApplicationEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSparkBlockManagerEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSparkBlockManagerEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSparkBlockManagerEvents'
      name: 'HDInsightSparkBlockManagerEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSparkEnvironmentEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSparkEnvironmentEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSparkEnvironmentEvents'
      name: 'HDInsightSparkEnvironmentEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSparkExecutorEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSparkExecutorEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSparkExecutorEvents'
      name: 'HDInsightSparkExecutorEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSparkExtraEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSparkExtraEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSparkExtraEvents'
      name: 'HDInsightSparkExtraEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSparkJobEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSparkJobEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSparkJobEvents'
      name: 'HDInsightSparkJobEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSparkLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSparkLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSparkLogs'
      name: 'HDInsightSparkLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSparkSQLExecutionEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSparkSQLExecutionEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSparkSQLExecutionEvents'
      name: 'HDInsightSparkSQLExecutionEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSparkStageEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSparkStageEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSparkStageEvents'
      name: 'HDInsightSparkStageEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSparkStageTaskAccumulables 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSparkStageTaskAccumulables'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSparkStageTaskAccumulables'
      name: 'HDInsightSparkStageTaskAccumulables'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightSparkTaskEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightSparkTaskEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightSparkTaskEvents'
      name: 'HDInsightSparkTaskEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightStormLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightStormLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightStormLogs'
      name: 'HDInsightStormLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightStormMetrics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightStormMetrics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightStormMetrics'
      name: 'HDInsightStormMetrics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HDInsightStormTopologyMetrics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HDInsightStormTopologyMetrics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HDInsightStormTopologyMetrics'
      name: 'HDInsightStormTopologyMetrics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_HealthStateChangeEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'HealthStateChangeEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'HealthStateChangeEvent'
      name: 'HealthStateChangeEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_Heartbeat 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'Heartbeat'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'Heartbeat'
      name: 'Heartbeat'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_InsightsMetrics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'InsightsMetrics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'InsightsMetrics'
      name: 'InsightsMetrics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_IntuneAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'IntuneAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'IntuneAuditLogs'
      name: 'IntuneAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_IntuneDeviceComplianceOrg 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'IntuneDeviceComplianceOrg'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'IntuneDeviceComplianceOrg'
      name: 'IntuneDeviceComplianceOrg'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_IntuneDevices 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'IntuneDevices'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'IntuneDevices'
      name: 'IntuneDevices'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_IntuneOperationalLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'IntuneOperationalLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'IntuneOperationalLogs'
      name: 'IntuneOperationalLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_KubeEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'KubeEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'KubeEvents'
      name: 'KubeEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_KubeHealth 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'KubeHealth'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'KubeHealth'
      name: 'KubeHealth'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_KubeMonAgentEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'KubeMonAgentEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'KubeMonAgentEvents'
      name: 'KubeMonAgentEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_KubeNodeInventory 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'KubeNodeInventory'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'KubeNodeInventory'
      name: 'KubeNodeInventory'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_KubePodInventory 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'KubePodInventory'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'KubePodInventory'
      name: 'KubePodInventory'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_KubePVInventory 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'KubePVInventory'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'KubePVInventory'
      name: 'KubePVInventory'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_KubeServices 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'KubeServices'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'KubeServices'
      name: 'KubeServices'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LAJobLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LAJobLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'LAJobLogs'
      name: 'LAJobLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LAQueryLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LAQueryLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'LAQueryLogs'
      name: 'LAQueryLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LASummaryLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LASummaryLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'LASummaryLogs'
      name: 'LASummaryLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LIATrackingEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LIATrackingEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'LIATrackingEvents'
      name: 'LIATrackingEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_LogicAppWorkflowRuntime 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'LogicAppWorkflowRuntime'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'LogicAppWorkflowRuntime'
      name: 'LogicAppWorkflowRuntime'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MCCEventLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MCCEventLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MCCEventLogs'
      name: 'MCCEventLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MCVPAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MCVPAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MCVPAuditLogs'
      name: 'MCVPAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MCVPOperationLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MCVPOperationLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MCVPOperationLogs'
      name: 'MCVPOperationLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MDCDetectionDNSEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MDCDetectionDNSEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MDCDetectionDNSEvents'
      name: 'MDCDetectionDNSEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MDCDetectionFimEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MDCDetectionFimEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MDCDetectionFimEvents'
      name: 'MDCDetectionFimEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MDCDetectionGatingValidationEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MDCDetectionGatingValidationEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MDCDetectionGatingValidationEvents'
      name: 'MDCDetectionGatingValidationEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MDCDetectionK8SApiEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MDCDetectionK8SApiEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MDCDetectionK8SApiEvents'
      name: 'MDCDetectionK8SApiEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MDCDetectionProcessV2Events 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MDCDetectionProcessV2Events'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MDCDetectionProcessV2Events'
      name: 'MDCDetectionProcessV2Events'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MDCFileIntegrityMonitoringEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MDCFileIntegrityMonitoringEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MDCFileIntegrityMonitoringEvents'
      name: 'MDCFileIntegrityMonitoringEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MDECustomCollectionDeviceFileEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MDECustomCollectionDeviceFileEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MDECustomCollectionDeviceFileEvents'
      name: 'MDECustomCollectionDeviceFileEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MDPResourceLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MDPResourceLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MDPResourceLog'
      name: 'MDPResourceLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MeshControlPlane 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MeshControlPlane'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MeshControlPlane'
      name: 'MeshControlPlane'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MicrosoftAzureBastionAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MicrosoftAzureBastionAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MicrosoftAzureBastionAuditLogs'
      name: 'MicrosoftAzureBastionAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MicrosoftDataShareReceivedSnapshotLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MicrosoftDataShareReceivedSnapshotLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MicrosoftDataShareReceivedSnapshotLog'
      name: 'MicrosoftDataShareReceivedSnapshotLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MicrosoftDataShareSentSnapshotLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MicrosoftDataShareSentSnapshotLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MicrosoftDataShareSentSnapshotLog'
      name: 'MicrosoftDataShareSentSnapshotLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MicrosoftDataShareShareLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MicrosoftDataShareShareLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MicrosoftDataShareShareLog'
      name: 'MicrosoftDataShareShareLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MicrosoftGraphActivityLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MicrosoftGraphActivityLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MicrosoftGraphActivityLogs'
      name: 'MicrosoftGraphActivityLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MicrosoftHealthcareApisAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MicrosoftHealthcareApisAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MicrosoftHealthcareApisAuditLogs'
      name: 'MicrosoftHealthcareApisAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MicrosoftServicePrincipalSignInLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MicrosoftServicePrincipalSignInLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MicrosoftServicePrincipalSignInLogs'
      name: 'MicrosoftServicePrincipalSignInLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MNFDeviceUpdates 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MNFDeviceUpdates'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MNFDeviceUpdates'
      name: 'MNFDeviceUpdates'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MNFSystemSessionHistoryUpdates 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MNFSystemSessionHistoryUpdates'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MNFSystemSessionHistoryUpdates'
      name: 'MNFSystemSessionHistoryUpdates'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MNFSystemStateMessageUpdates 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MNFSystemStateMessageUpdates'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MNFSystemStateMessageUpdates'
      name: 'MNFSystemStateMessageUpdates'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MPCIngestionLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MPCIngestionLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MPCIngestionLogs'
      name: 'MPCIngestionLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MySqlAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MySqlAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MySqlAuditLogs'
      name: 'MySqlAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_MySqlSlowLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'MySqlSlowLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'MySqlSlowLogs'
      name: 'MySqlSlowLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NatGatewayFlowlogsV1 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NatGatewayFlowlogsV1'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NatGatewayFlowlogsV1'
      name: 'NatGatewayFlowlogsV1'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCBMBreakGlassAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCBMBreakGlassAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCBMBreakGlassAuditLogs'
      name: 'NCBMBreakGlassAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCBMSecurityDefenderLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCBMSecurityDefenderLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCBMSecurityDefenderLogs'
      name: 'NCBMSecurityDefenderLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCBMSecurityLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCBMSecurityLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCBMSecurityLogs'
      name: 'NCBMSecurityLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCBMSystemLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCBMSystemLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCBMSystemLogs'
      name: 'NCBMSystemLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCCIDRACLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCCIDRACLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCCIDRACLogs'
      name: 'NCCIDRACLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCCKubernetesAPIAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCCKubernetesAPIAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCCKubernetesAPIAuditLogs'
      name: 'NCCKubernetesAPIAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCCKubernetesLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCCKubernetesLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCCKubernetesLogs'
      name: 'NCCKubernetesLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCCPlatformOperationsLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCCPlatformOperationsLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCCPlatformOperationsLogs'
      name: 'NCCPlatformOperationsLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCCVMOrchestrationLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCCVMOrchestrationLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCCVMOrchestrationLogs'
      name: 'NCCVMOrchestrationLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCMClusterOperationsLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCMClusterOperationsLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCMClusterOperationsLogs'
      name: 'NCMClusterOperationsLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCSStorageAlerts 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCSStorageAlerts'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCSStorageAlerts'
      name: 'NCSStorageAlerts'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCSStorageAudits 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCSStorageAudits'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCSStorageAudits'
      name: 'NCSStorageAudits'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NCSStorageLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NCSStorageLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NCSStorageLogs'
      name: 'NCSStorageLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NetworkAccessAlerts 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NetworkAccessAlerts'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NetworkAccessAlerts'
      name: 'NetworkAccessAlerts'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NetworkAccessConnectionEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NetworkAccessConnectionEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NetworkAccessConnectionEvents'
      name: 'NetworkAccessConnectionEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NetworkAccessGenerativeAIInsights 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NetworkAccessGenerativeAIInsights'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NetworkAccessGenerativeAIInsights'
      name: 'NetworkAccessGenerativeAIInsights'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NetworkAccessTraffic 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NetworkAccessTraffic'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NetworkAccessTraffic'
      name: 'NetworkAccessTraffic'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NginxUpstreamUpdateLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NginxUpstreamUpdateLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NginxUpstreamUpdateLogs'
      name: 'NginxUpstreamUpdateLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NGXOperationLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NGXOperationLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NGXOperationLogs'
      name: 'NGXOperationLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NGXSecurityLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NGXSecurityLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NGXSecurityLogs'
      name: 'NGXSecurityLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NSPAccessLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NSPAccessLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NSPAccessLogs'
      name: 'NSPAccessLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NTAInsights 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NTAInsights'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NTAInsights'
      name: 'NTAInsights'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NTAIpDetails 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NTAIpDetails'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NTAIpDetails'
      name: 'NTAIpDetails'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NTANetAnalytics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NTANetAnalytics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NTANetAnalytics'
      name: 'NTANetAnalytics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NTARuleRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NTARuleRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NTARuleRecommendation'
      name: 'NTARuleRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NTATopologyDetails 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NTATopologyDetails'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NTATopologyDetails'
      name: 'NTATopologyDetails'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NWConnectionMonitorDestinationListenerResult 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NWConnectionMonitorDestinationListenerResult'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NWConnectionMonitorDestinationListenerResult'
      name: 'NWConnectionMonitorDestinationListenerResult'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NWConnectionMonitorDNSResult 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NWConnectionMonitorDNSResult'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NWConnectionMonitorDNSResult'
      name: 'NWConnectionMonitorDNSResult'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NWConnectionMonitorPathResult 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NWConnectionMonitorPathResult'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NWConnectionMonitorPathResult'
      name: 'NWConnectionMonitorPathResult'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_NWConnectionMonitorTestResult 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'NWConnectionMonitorTestResult'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'NWConnectionMonitorTestResult'
      name: 'NWConnectionMonitorTestResult'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OEPAirFlowTask 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OEPAirFlowTask'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OEPAirFlowTask'
      name: 'OEPAirFlowTask'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OEPAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OEPAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OEPAuditLogs'
      name: 'OEPAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OEPDataplaneLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OEPDataplaneLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OEPDataplaneLogs'
      name: 'OEPDataplaneLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OEPElasticOperator 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OEPElasticOperator'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OEPElasticOperator'
      name: 'OEPElasticOperator'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OEPElasticsearch 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OEPElasticsearch'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OEPElasticsearch'
      name: 'OEPElasticsearch'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OEWAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OEWAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OEWAuditLogs'
      name: 'OEWAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OEWExperimentAssignmentSummary 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OEWExperimentAssignmentSummary'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OEWExperimentAssignmentSummary'
      name: 'OEWExperimentAssignmentSummary'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OEWExperimentScorecardMetricPairs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OEWExperimentScorecardMetricPairs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OEWExperimentScorecardMetricPairs'
      name: 'OEWExperimentScorecardMetricPairs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OEWExperimentScorecards 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OEWExperimentScorecards'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OEWExperimentScorecards'
      name: 'OEWExperimentScorecards'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OGOAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OGOAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OGOAuditLogs'
      name: 'OGOAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OLPSupplyChainEntityOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OLPSupplyChainEntityOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OLPSupplyChainEntityOperations'
      name: 'OLPSupplyChainEntityOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OLPSupplyChainEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OLPSupplyChainEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OLPSupplyChainEvents'
      name: 'OLPSupplyChainEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OmsCustomerProfileFact 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OmsCustomerProfileFact'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OmsCustomerProfileFact'
      name: 'OmsCustomerProfileFact'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_Operation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'Operation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'Operation'
      name: 'Operation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OracleCloudDatabase 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OracleCloudDatabase'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OracleCloudDatabase'
      name: 'OracleCloudDatabase'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OTelEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OTelEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OTelEvents'
      name: 'OTelEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OTelLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OTelLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OTelLogs'
      name: 'OTelLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OTelResources 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OTelResources'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OTelResources'
      name: 'OTelResources'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OTelSpans 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OTelSpans'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OTelSpans'
      name: 'OTelSpans'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OTelTraces 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OTelTraces'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OTelTraces'
      name: 'OTelTraces'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_OTelTracesAgent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'OTelTracesAgent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'OTelTracesAgent'
      name: 'OTelTracesAgent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_Perf 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'Perf'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'Perf'
      name: 'Perf'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PerfInsightsFindings 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PerfInsightsFindings'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PerfInsightsFindings'
      name: 'PerfInsightsFindings'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PerfInsightsImpactedResources 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PerfInsightsImpactedResources'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PerfInsightsImpactedResources'
      name: 'PerfInsightsImpactedResources'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PerfInsightsRun 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PerfInsightsRun'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PerfInsightsRun'
      name: 'PerfInsightsRun'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PFTitleAuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PFTitleAuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PFTitleAuditLogs'
      name: 'PFTitleAuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PGSQLAutovacuumStats 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PGSQLAutovacuumStats'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PGSQLAutovacuumStats'
      name: 'PGSQLAutovacuumStats'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PGSQLDbTransactionsStats 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PGSQLDbTransactionsStats'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PGSQLDbTransactionsStats'
      name: 'PGSQLDbTransactionsStats'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PGSQLPgBouncer 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PGSQLPgBouncer'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PGSQLPgBouncer'
      name: 'PGSQLPgBouncer'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PGSQLPgStatActivitySessions 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PGSQLPgStatActivitySessions'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PGSQLPgStatActivitySessions'
      name: 'PGSQLPgStatActivitySessions'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PGSQLQueryStoreQueryText 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PGSQLQueryStoreQueryText'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PGSQLQueryStoreQueryText'
      name: 'PGSQLQueryStoreQueryText'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PGSQLQueryStoreRuntime 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PGSQLQueryStoreRuntime'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PGSQLQueryStoreRuntime'
      name: 'PGSQLQueryStoreRuntime'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PGSQLQueryStoreWaits 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PGSQLQueryStoreWaits'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PGSQLQueryStoreWaits'
      name: 'PGSQLQueryStoreWaits'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PGSQLServerLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PGSQLServerLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PGSQLServerLogs'
      name: 'PGSQLServerLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PowerBIDatasetsTenant 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PowerBIDatasetsTenant'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PowerBIDatasetsTenant'
      name: 'PowerBIDatasetsTenant'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PowerBIDatasetsWorkspace 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PowerBIDatasetsWorkspace'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PowerBIDatasetsWorkspace'
      name: 'PowerBIDatasetsWorkspace'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PurviewDataSensitivityLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PurviewDataSensitivityLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PurviewDataSensitivityLogs'
      name: 'PurviewDataSensitivityLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PurviewScanStatusLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PurviewScanStatusLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PurviewScanStatusLogs'
      name: 'PurviewScanStatusLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_PurviewSecurityLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'PurviewSecurityLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'PurviewSecurityLogs'
      name: 'PurviewSecurityLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_REDConnectionEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'REDConnectionEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'REDConnectionEvents'
      name: 'REDConnectionEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_RemoteNetworkHealthLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'RemoteNetworkHealthLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'RemoteNetworkHealthLogs'
      name: 'RemoteNetworkHealthLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ResourceManagementPublicAccessLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ResourceManagementPublicAccessLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ResourceManagementPublicAccessLogs'
      name: 'ResourceManagementPublicAccessLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_RetinaNetworkFlowLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'RetinaNetworkFlowLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'RetinaNetworkFlowLogs'
      name: 'RetinaNetworkFlowLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SCCMAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SCCMAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SCCMAssessmentRecommendation'
      name: 'SCCMAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SCGPoolExecutionLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SCGPoolExecutionLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SCGPoolExecutionLog'
      name: 'SCGPoolExecutionLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SCGPoolRequestLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SCGPoolRequestLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SCGPoolRequestLog'
      name: 'SCGPoolRequestLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SCOMAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SCOMAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SCOMAssessmentRecommendation'
      name: 'SCOMAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ServiceFabricOperationalEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ServiceFabricOperationalEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ServiceFabricOperationalEvent'
      name: 'ServiceFabricOperationalEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ServiceFabricReliableActorEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ServiceFabricReliableActorEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ServiceFabricReliableActorEvent'
      name: 'ServiceFabricReliableActorEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ServiceFabricReliableServiceEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ServiceFabricReliableServiceEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ServiceFabricReliableServiceEvent'
      name: 'ServiceFabricReliableServiceEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SfBAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SfBAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SfBAssessmentRecommendation'
      name: 'SfBAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SfBOnlineAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SfBOnlineAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SfBOnlineAssessmentRecommendation'
      name: 'SfBOnlineAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SharePointOnlineAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SharePointOnlineAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SharePointOnlineAssessmentRecommendation'
      name: 'SharePointOnlineAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SignalRServiceDiagnosticLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SignalRServiceDiagnosticLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SignalRServiceDiagnosticLogs'
      name: 'SignalRServiceDiagnosticLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SigninLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SigninLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SigninLogs'
      name: 'SigninLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SPAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SPAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SPAssessmentRecommendation'
      name: 'SPAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SQLAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SQLAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SQLAssessmentRecommendation'
      name: 'SQLAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SQLSecurityAuditEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SQLSecurityAuditEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SQLSecurityAuditEvents'
      name: 'SQLSecurityAuditEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageAntimalwareScanResults 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageAntimalwareScanResults'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageAntimalwareScanResults'
      name: 'StorageAntimalwareScanResults'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageBlobLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageBlobLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageBlobLogs'
      name: 'StorageBlobLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageCacheOperationEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageCacheOperationEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageCacheOperationEvents'
      name: 'StorageCacheOperationEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageCacheUpgradeEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageCacheUpgradeEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageCacheUpgradeEvents'
      name: 'StorageCacheUpgradeEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageCacheWarningEvents 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageCacheWarningEvents'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageCacheWarningEvents'
      name: 'StorageCacheWarningEvents'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageFileLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageFileLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageFileLogs'
      name: 'StorageFileLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageMalwareScanningResults 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageMalwareScanningResults'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageMalwareScanningResults'
      name: 'StorageMalwareScanningResults'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageMoverCopyLogsFailed 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageMoverCopyLogsFailed'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageMoverCopyLogsFailed'
      name: 'StorageMoverCopyLogsFailed'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageMoverCopyLogsTransferred 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageMoverCopyLogsTransferred'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageMoverCopyLogsTransferred'
      name: 'StorageMoverCopyLogsTransferred'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageMoverJobRunLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageMoverJobRunLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageMoverJobRunLogs'
      name: 'StorageMoverJobRunLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageQueueLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageQueueLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageQueueLogs'
      name: 'StorageQueueLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_StorageTableLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'StorageTableLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'StorageTableLogs'
      name: 'StorageTableLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SucceededIngestion 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SucceededIngestion'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SucceededIngestion'
      name: 'SucceededIngestion'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SVMPoolExecutionLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SVMPoolExecutionLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SVMPoolExecutionLog'
      name: 'SVMPoolExecutionLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SVMPoolRequestLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SVMPoolRequestLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SVMPoolRequestLog'
      name: 'SVMPoolRequestLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseBigDataPoolApplicationsEnded 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseBigDataPoolApplicationsEnded'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseBigDataPoolApplicationsEnded'
      name: 'SynapseBigDataPoolApplicationsEnded'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseBuiltinSqlPoolRequestsEnded 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseBuiltinSqlPoolRequestsEnded'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseBuiltinSqlPoolRequestsEnded'
      name: 'SynapseBuiltinSqlPoolRequestsEnded'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseDXCommand 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseDXCommand'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseDXCommand'
      name: 'SynapseDXCommand'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseDXFailedIngestion 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseDXFailedIngestion'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseDXFailedIngestion'
      name: 'SynapseDXFailedIngestion'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseDXIngestionBatching 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseDXIngestionBatching'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseDXIngestionBatching'
      name: 'SynapseDXIngestionBatching'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseDXQuery 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseDXQuery'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseDXQuery'
      name: 'SynapseDXQuery'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseDXSucceededIngestion 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseDXSucceededIngestion'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseDXSucceededIngestion'
      name: 'SynapseDXSucceededIngestion'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseDXTableDetails 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseDXTableDetails'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseDXTableDetails'
      name: 'SynapseDXTableDetails'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseDXTableUsageStatistics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseDXTableUsageStatistics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseDXTableUsageStatistics'
      name: 'SynapseDXTableUsageStatistics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseGatewayApiRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseGatewayApiRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseGatewayApiRequests'
      name: 'SynapseGatewayApiRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseIntegrationActivityRuns 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseIntegrationActivityRuns'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseIntegrationActivityRuns'
      name: 'SynapseIntegrationActivityRuns'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseIntegrationPipelineRuns 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseIntegrationPipelineRuns'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseIntegrationPipelineRuns'
      name: 'SynapseIntegrationPipelineRuns'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseIntegrationTriggerRuns 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseIntegrationTriggerRuns'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseIntegrationTriggerRuns'
      name: 'SynapseIntegrationTriggerRuns'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseLinkEvent 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseLinkEvent'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseLinkEvent'
      name: 'SynapseLinkEvent'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseRbacOperations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseRbacOperations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseRbacOperations'
      name: 'SynapseRbacOperations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseScopePoolScopeJobsEnded 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseScopePoolScopeJobsEnded'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseScopePoolScopeJobsEnded'
      name: 'SynapseScopePoolScopeJobsEnded'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseScopePoolScopeJobsStateChange 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseScopePoolScopeJobsStateChange'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseScopePoolScopeJobsStateChange'
      name: 'SynapseScopePoolScopeJobsStateChange'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseSqlPoolDmsWorkers 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseSqlPoolDmsWorkers'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseSqlPoolDmsWorkers'
      name: 'SynapseSqlPoolDmsWorkers'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseSqlPoolExecRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseSqlPoolExecRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseSqlPoolExecRequests'
      name: 'SynapseSqlPoolExecRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseSqlPoolRequestSteps 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseSqlPoolRequestSteps'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseSqlPoolRequestSteps'
      name: 'SynapseSqlPoolRequestSteps'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseSqlPoolSqlRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseSqlPoolSqlRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseSqlPoolSqlRequests'
      name: 'SynapseSqlPoolSqlRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_SynapseSqlPoolWaits 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'SynapseSqlPoolWaits'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'SynapseSqlPoolWaits'
      name: 'SynapseSqlPoolWaits'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_Syslog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'Syslog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'Syslog'
      name: 'Syslog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_TOUserAudits 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'TOUserAudits'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'TOUserAudits'
      name: 'TOUserAudits'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_TOUserDiagnostics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'TOUserDiagnostics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'TOUserDiagnostics'
      name: 'TOUserDiagnostics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_TSIIngress 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'TSIIngress'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'TSIIngress'
      name: 'TSIIngress'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_UCClient 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'UCClient'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'UCClient'
      name: 'UCClient'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_UCClientReadinessStatus 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'UCClientReadinessStatus'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'UCClientReadinessStatus'
      name: 'UCClientReadinessStatus'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_UCClientUpdateStatus 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'UCClientUpdateStatus'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'UCClientUpdateStatus'
      name: 'UCClientUpdateStatus'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_UCDeviceAlert 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'UCDeviceAlert'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'UCDeviceAlert'
      name: 'UCDeviceAlert'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_UCDOAggregatedStatus 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'UCDOAggregatedStatus'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'UCDOAggregatedStatus'
      name: 'UCDOAggregatedStatus'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_UCDOStatus 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'UCDOStatus'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'UCDOStatus'
      name: 'UCDOStatus'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_UCServiceUpdateStatus 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'UCServiceUpdateStatus'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'UCServiceUpdateStatus'
      name: 'UCServiceUpdateStatus'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_UCUpdateAlert 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'UCUpdateAlert'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'UCUpdateAlert'
      name: 'UCUpdateAlert'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_Usage 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'Usage'
  properties: {
    plan: 'Analytics'
    retentionInDays: 90
    schema: {
      displayName: 'Usage'
      name: 'Usage'
    }
    totalRetentionInDays: 90
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_VCoreMongoRequests 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'VCoreMongoRequests'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'VCoreMongoRequests'
      name: 'VCoreMongoRequests'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_VIAudit 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'VIAudit'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'VIAudit'
      name: 'VIAudit'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_VIIndexing 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'VIIndexing'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'VIIndexing'
      name: 'VIIndexing'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_VMBoundPort 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'VMBoundPort'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'VMBoundPort'
      name: 'VMBoundPort'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_VMComputer 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'VMComputer'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'VMComputer'
      name: 'VMComputer'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_VMConnection 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'VMConnection'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'VMConnection'
      name: 'VMConnection'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_VMProcess 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'VMProcess'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'VMProcess'
      name: 'VMProcess'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_W3CIISLog 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'W3CIISLog'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'W3CIISLog'
      name: 'W3CIISLog'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WebPubSubConnectivity 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WebPubSubConnectivity'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WebPubSubConnectivity'
      name: 'WebPubSubConnectivity'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WebPubSubHttpRequest 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WebPubSubHttpRequest'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WebPubSubHttpRequest'
      name: 'WebPubSubHttpRequest'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WebPubSubMessaging 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WebPubSubMessaging'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WebPubSubMessaging'
      name: 'WebPubSubMessaging'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_Windows365AuditLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'Windows365AuditLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'Windows365AuditLogs'
      name: 'Windows365AuditLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WindowsClientAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WindowsClientAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WindowsClientAssessmentRecommendation'
      name: 'WindowsClientAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WindowsServerAssessmentRecommendation 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WindowsServerAssessmentRecommendation'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WindowsServerAssessmentRecommendation'
      name: 'WindowsServerAssessmentRecommendation'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WorkloadDiagnosticLogs 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WorkloadDiagnosticLogs'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WorkloadDiagnosticLogs'
      name: 'WorkloadDiagnosticLogs'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WOUserAudits 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WOUserAudits'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WOUserAudits'
      name: 'WOUserAudits'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WOUserDiagnostics 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WOUserDiagnostics'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WOUserDiagnostics'
      name: 'WOUserDiagnostics'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDAgentHealthStatus 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDAgentHealthStatus'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDAgentHealthStatus'
      name: 'WVDAgentHealthStatus'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDAutoscaleEvaluationPooled 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDAutoscaleEvaluationPooled'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDAutoscaleEvaluationPooled'
      name: 'WVDAutoscaleEvaluationPooled'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDCheckpoints 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDCheckpoints'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDCheckpoints'
      name: 'WVDCheckpoints'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDConnectionGraphicsDataPreview 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDConnectionGraphicsDataPreview'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDConnectionGraphicsDataPreview'
      name: 'WVDConnectionGraphicsDataPreview'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDConnectionNetworkData 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDConnectionNetworkData'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDConnectionNetworkData'
      name: 'WVDConnectionNetworkData'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDConnections 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDConnections'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDConnections'
      name: 'WVDConnections'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDErrors 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDErrors'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDErrors'
      name: 'WVDErrors'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDFeeds 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDFeeds'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDFeeds'
      name: 'WVDFeeds'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDHostRegistrations 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDHostRegistrations'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDHostRegistrations'
      name: 'WVDHostRegistrations'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDManagement 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDManagement'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDManagement'
      name: 'WVDManagement'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDMultiLinkAdd 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDMultiLinkAdd'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDMultiLinkAdd'
      name: 'WVDMultiLinkAdd'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_WVDSessionHostManagement 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'WVDSessionHostManagement'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'WVDSessionHostManagement'
      name: 'WVDSessionHostManagement'
    }
    totalRetentionInDays: 30
  }
}

resource workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_ZTSRequest 'Microsoft.OperationalInsights/workspaces/tables@2025-07-01' = {
  parent: workspaces_DefaultWorkspace_c0c21e76_a03c_4747_af34_0720b273ff00_EUS_name_resource
  name: 'ZTSRequest'
  properties: {
    plan: 'Analytics'
    retentionInDays: 30
    schema: {
      displayName: 'ZTSRequest'
      name: 'ZTSRequest'
    }
    totalRetentionInDays: 30
  }
}

resource storageAccounts_stxenobiasoft_name_default 'Microsoft.Storage/storageAccounts/blobServices@2025-06-01' = {
  parent: storageAccounts_stxenobiasoft_name_resource
  name: 'default'
  properties: {
    containerDeleteRetentionPolicy: {
      days: 7
      enabled: true
    }
    cors: {
      corsRules: []
    }
    deleteRetentionPolicy: {
      allowPermanentDelete: false
      days: 7
      enabled: true
    }
  }
  sku: {
    name: 'Standard_RAGRS'
    tier: 'Standard'
  }
}

resource Microsoft_Storage_storageAccounts_fileServices_storageAccounts_stxenobiasoft_name_default 'Microsoft.Storage/storageAccounts/fileServices@2025-06-01' = {
  parent: storageAccounts_stxenobiasoft_name_resource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
    protocolSettings: {
      smb: {}
    }
    shareDeleteRetentionPolicy: {
      days: 7
      enabled: true
    }
  }
  sku: {
    name: 'Standard_RAGRS'
    tier: 'Standard'
  }
}

resource Microsoft_Storage_storageAccounts_queueServices_storageAccounts_stxenobiasoft_name_default 'Microsoft.Storage/storageAccounts/queueServices@2025-06-01' = {
  parent: storageAccounts_stxenobiasoft_name_resource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource Microsoft_Storage_storageAccounts_tableServices_storageAccounts_stxenobiasoft_name_default 'Microsoft.Storage/storageAccounts/tableServices@2025-06-01' = {
  parent: storageAccounts_stxenobiasoft_name_resource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource sites_XenobiaSoftSudoku_name_resource 'Microsoft.Web/sites@2024-11-01' = {
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'app,linux'
  location: 'East US'
  name: sites_XenobiaSoftSudoku_name
  properties: {
    clientAffinityEnabled: false
    clientAffinityProxyEnabled: false
    clientCertEnabled: false
    clientCertMode: 'Required'
    containerSize: 0
    customDomainVerificationId: '41E0FF4E8F9CD4BF0E9744634AA25AB01110CC361F04BD286E5F90CC7D5889F9'
    dailyMemoryTimeQuota: 0
    dnsConfiguration: {}
    enabled: true
    endToEndEncryptionEnabled: false
    hostNameSslStates: [
      {
        hostType: 'Standard'
        name: 'sudoku.xenobiasoft.com'
        sslState: 'SniEnabled'
        thumbprint: '6809B2705350A246BD73DECDF4482ADFD2270828'
      }
      {
        hostType: 'Standard'
        name: 'xenobiasoftsudoku.azurewebsites.net'
        sslState: 'Disabled'
      }
      {
        hostType: 'Repository'
        name: 'xenobiasoftsudoku.scm.azurewebsites.net'
        sslState: 'Disabled'
      }
    ]
    hostNamesDisabled: false
    httpsOnly: true
    hyperV: false
    ipMode: 'IPv4'
    isXenon: false
    keyVaultReferenceIdentity: 'SystemAssigned'
    outboundVnetRouting: {
      allTraffic: false
      applicationTraffic: false
      backupRestoreTraffic: false
      contentShareTraffic: false
      imagePullTraffic: false
    }
    publicNetworkAccess: 'Enabled'
    redundancyMode: 'None'
    reserved: true
    scmSiteAlsoStopped: false
    serverFarmId: serverfarms_XenobiaSoftServicePlan_name_resource.id
    siteConfig: {
      acrUseManagedIdentityCreds: false
      alwaysOn: true
      functionAppScaleLimit: 0
      http20Enabled: true
      linuxFxVersion: 'DOTNETCORE|10.0'
      minimumElasticInstanceCount: 1
      numberOfWorkers: 1
    }
    sshEnabled: true
    storageAccountRequired: false
  }
  tags: {
    environment: 'production'
    project: 'XenobiaSoftSudoku'
  }
}

resource sites_XenobiaSoftSudokuApi_name_resource 'Microsoft.Web/sites@2024-11-01' = {
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'app,linux'
  location: 'East US'
  name: sites_XenobiaSoftSudokuApi_name
  properties: {
    clientAffinityEnabled: false
    clientAffinityProxyEnabled: false
    clientCertEnabled: false
    clientCertMode: 'Required'
    containerSize: 0
    customDomainVerificationId: '41E0FF4E8F9CD4BF0E9744634AA25AB01110CC361F04BD286E5F90CC7D5889F9'
    dailyMemoryTimeQuota: 0
    dnsConfiguration: {}
    enabled: true
    endToEndEncryptionEnabled: false
    hostNameSslStates: [
      {
        hostType: 'Standard'
        name: 'xenobiasoftsudokuapi.azurewebsites.net'
        sslState: 'Disabled'
      }
      {
        hostType: 'Repository'
        name: 'xenobiasoftsudokuapi.scm.azurewebsites.net'
        sslState: 'Disabled'
      }
    ]
    hostNamesDisabled: false
    httpsOnly: true
    hyperV: false
    ipMode: 'IPv4'
    isXenon: false
    keyVaultReferenceIdentity: 'SystemAssigned'
    outboundVnetRouting: {
      allTraffic: false
      applicationTraffic: false
      backupRestoreTraffic: false
      contentShareTraffic: false
      imagePullTraffic: false
    }
    publicNetworkAccess: 'Enabled'
    redundancyMode: 'None'
    reserved: true
    scmSiteAlsoStopped: false
    serverFarmId: serverfarms_XenobiaSoftServicePlan_name_resource.id
    siteConfig: {
      acrUseManagedIdentityCreds: false
      alwaysOn: true
      functionAppScaleLimit: 0
      http20Enabled: true
      linuxFxVersion: 'DOTNETCORE|10.0'
      minimumElasticInstanceCount: 1
      numberOfWorkers: 1
    }
    storageAccountRequired: false
  }
  tags: {
    component: 'api'
    environment: 'production'
    project: 'XenobiaSoftSudoku'
  }
}

resource sites_XenobiaSoftSudoku_name_ftp 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: 'ftp'
  properties: {
    allow: true
  }
  tags: {
    environment: 'production'
    project: 'XenobiaSoftSudoku'
  }
}

resource sites_XenobiaSoftSudokuApi_name_ftp 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: 'ftp'
  properties: {
    allow: true
  }
  tags: {
    component: 'api'
    environment: 'production'
    project: 'XenobiaSoftSudoku'
  }
}

resource sites_XenobiaSoftSudoku_name_scm 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: 'scm'
  properties: {
    allow: true
  }
  tags: {
    environment: 'production'
    project: 'XenobiaSoftSudoku'
  }
}

resource sites_XenobiaSoftSudokuApi_name_scm 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: 'scm'
  properties: {
    allow: true
  }
  tags: {
    component: 'api'
    environment: 'production'
    project: 'XenobiaSoftSudoku'
  }
}

resource sites_XenobiaSoftSudoku_name_web 'Microsoft.Web/sites/config@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: 'web'
  properties: {
    acrUseManagedIdentityCreds: false
    alwaysOn: true
    appCommandLine: 'dotnet Sudoku.Blazor.dll'
    autoHealEnabled: true
    autoHealRules: {
      actions: {
        actionType: 'CustomAction'
        customAction: {
          exe: 'D:\\home\\data\\DaaS\\bin\\DaasConsole.exe'
          parameters: '-CollectKillAnalyze "Memory Dump"'
        }
        minProcessExecutionTime: '00:00:00'
      }
      triggers: {
        privateBytesInKB: 0
        slowRequestsWithPath: []
        statusCodes: []
        statusCodesRange: [
          {
            count: 10
            path: '/health-check'
            statusCodes: '399-500'
            timeInterval: '00:01:00'
          }
        ]
      }
    }
    azureStorageAccounts: {}
    defaultDocuments: [
      'Default.html'
      'index.html'
    ]
    detailedErrorLoggingEnabled: false
    elasticWebAppScaleLimit: 0
    experiments: {
      rampUpRules: []
    }
    ftpsState: 'FtpsOnly'
    functionsRuntimeScaleMonitoringEnabled: false
    healthCheckPath: '/health-check'
    http20Enabled: true
    http20ProxyFlag: 0
    httpLoggingEnabled: false
    ipSecurityRestrictions: [
      {
        action: 'Allow'
        description: 'Allow all access'
        ipAddress: 'Any'
        name: 'Allow all'
        priority: 2147483647
      }
    ]
    linuxFxVersion: 'DOTNETCORE|10.0'
    loadBalancing: 'LeastRequests'
    localMySqlEnabled: false
    logsDirectorySizeLimit: 35
    managedPipelineMode: 'Integrated'
    managedServiceIdentityId: 11115
    minTlsVersion: '1.2'
    minimumElasticInstanceCount: 1
    netFrameworkVersion: 'v4.0'
    numberOfWorkers: 1
    preWarmedInstanceCount: 0
    publishingUsername: '$XenobiaSoftSudoku'
    remoteDebuggingEnabled: false
    requestTracingEnabled: false
    scmIpSecurityRestrictions: [
      {
        action: 'Allow'
        description: 'Allow all access'
        ipAddress: 'Any'
        name: 'Allow all'
        priority: 2147483647
      }
    ]
    scmIpSecurityRestrictionsUseMain: false
    scmMinTlsVersion: '1.2'
    scmType: 'VSTSRM'
    use32BitWorkerProcess: true
    virtualApplications: [
      {
        physicalPath: 'site\\wwwroot'
        preloadEnabled: true
        virtualPath: '/'
      }
    ]
    vnetPrivatePortsCount: 0
    vnetRouteAllEnabled: false
    webSocketsEnabled: false
  }
  tags: {
    environment: 'production'
    project: 'XenobiaSoftSudoku'
  }
}

resource sites_XenobiaSoftSudokuApi_name_web 'Microsoft.Web/sites/config@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: 'web'
  properties: {
    acrUseManagedIdentityCreds: false
    alwaysOn: true
    autoHealEnabled: false
    azureStorageAccounts: {}
    cors: {
      allowedOrigins: [
        'https://sudoku.xenobiasoft.com'
        'https://XenobiaSoftSudoku.azurewebsites.net'
      ]
      supportCredentials: false
    }
    defaultDocuments: [
      'Default.htm'
      'Default.html'
      'Default.asp'
      'index.htm'
      'index.html'
      'iisstart.htm'
      'default.aspx'
      'index.php'
      'hostingstart.html'
    ]
    detailedErrorLoggingEnabled: false
    elasticWebAppScaleLimit: 0
    experiments: {
      rampUpRules: []
    }
    ftpsState: 'FtpsOnly'
    functionsRuntimeScaleMonitoringEnabled: false
    healthCheckPath: '/health-check'
    http20Enabled: true
    http20ProxyFlag: 0
    httpLoggingEnabled: false
    ipSecurityRestrictions: [
      {
        action: 'Allow'
        description: 'Allow all access'
        ipAddress: 'Any'
        name: 'Allow all'
        priority: 2147483647
      }
    ]
    linuxFxVersion: 'DOTNETCORE|10.0'
    loadBalancing: 'LeastRequests'
    localMySqlEnabled: false
    logsDirectorySizeLimit: 35
    managedPipelineMode: 'Integrated'
    managedServiceIdentityId: 12924
    minTlsVersion: '1.2'
    minimumElasticInstanceCount: 1
    netFrameworkVersion: 'v4.0'
    numberOfWorkers: 1
    preWarmedInstanceCount: 0
    publishingUsername: '$XenobiaSoftSudokuApi'
    remoteDebuggingEnabled: false
    requestTracingEnabled: false
    scmIpSecurityRestrictions: [
      {
        action: 'Allow'
        description: 'Allow all access'
        ipAddress: 'Any'
        name: 'Allow all'
        priority: 2147483647
      }
    ]
    scmIpSecurityRestrictionsUseMain: false
    scmMinTlsVersion: '1.2'
    scmType: 'None'
    use32BitWorkerProcess: true
    virtualApplications: [
      {
        physicalPath: 'site\\wwwroot'
        preloadEnabled: true
        virtualPath: '/'
      }
    ]
    vnetPrivatePortsCount: 0
    vnetRouteAllEnabled: false
    webSocketsEnabled: false
  }
  tags: {
    component: 'api'
    environment: 'production'
    project: 'XenobiaSoftSudoku'
  }
}

resource sites_XenobiaSoftSudokuApi_name_0b8ae224_3a73_43d3_ae54_4aefbc7bce02 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: '0b8ae224-3a73-43d3-ae54-4aefbc7bce02'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T22:58:40.8078349Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T22:57:42.9514592Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudoku_name_1ea714e7_522b_4830_8796_cedfc460e62e 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: '1ea714e7-522b-4830-8796-cedfc460e62e'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-05T04:20:46.4960799Z'
    message: 'OneDeploy'
    start_time: '2026-04-05T04:20:39.9473973Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudoku_name_32e7a0c1_6055_428a_8b30_48a4dca9409a 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: '32e7a0c1-6055-428a-8b30-48a4dca9409a'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T23:04:10.1855219Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T23:04:05.7321565Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudoku_name_41315b6f_9221_40f1_8e2e_580f22dc9e6a 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: '41315b6f-9221-40f1-8e2e-580f22dc9e6a'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T22:56:45.7415561Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T22:56:44.0322244Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudokuApi_name_470cf279_e04f_4bd1_b456_72df4ca3f5f1 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: '470cf279-e04f-4bd1-b456-72df4ca3f5f1'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-05T04:22:26.2894967Z'
    message: 'OneDeploy'
    start_time: '2026-04-05T04:21:41.5140915Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudokuApi_name_4b336a92_ec31_4def_b389_ed8dcf3be568 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: '4b336a92-ec31-4def-b389-ed8dcf3be568'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T23:00:08.1695559Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T22:59:29.3456113Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudoku_name_4bdee76f_7cac_4282_814c_877448025ec6 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: '4bdee76f-7cac-4282-814c-877448025ec6'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T22:57:33.9852169Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T22:57:20.5515464Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudokuApi_name_6457048f_52a2_46c4_9367_977c8854e3ec 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: '6457048f-52a2-46c4-9367-977c8854e3ec'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-05T04:24:12.3585477Z'
    message: 'OneDeploy'
    start_time: '2026-04-05T04:23:22.3428478Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudoku_name_6e0f033f_1dd7_4e42_abae_67ec0203847d 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: '6e0f033f-1dd7-4e42-abae-67ec0203847d'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T20:59:38.3182399Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T20:59:28.6354643Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudoku_name_6ee570bd_6859_400d_94a5_bcae059dd520 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: '6ee570bd-6859-400d-94a5-bcae059dd520'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T20:53:44.5810576Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T20:53:32.8137431Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudokuApi_name_72b5aba9_33e9_4f12_869d_9c0f4434a5cb 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: '72b5aba9-33e9-4f12-869d-9c0f4434a5cb'
  properties: {
    active: true
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-05T04:38:29.0307714Z'
    message: 'OneDeploy'
    start_time: '2026-04-05T04:37:25.794724Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudokuApi_name_90f6906c_5088_4fdb_ad88_60b47d835990 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: '90f6906c-5088-4fdb-ad88-60b47d835990'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T20:41:31.3756929Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T20:41:03.9093445Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudokuApi_name_913ce31e_c23e_43b0_9645_2524916b8b64 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: '913ce31e-c23e-43b0-9645-2524916b8b64'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T21:01:44.8738127Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T21:01:20.4352109Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudokuApi_name_922e65cb_ced8_4a8a_9b4a_b620c8381084 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: '922e65cb-ced8-4a8a-9b4a-b620c8381084'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T20:55:24.7857173Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T20:54:42.139743Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudoku_name_9786b6b8_c05d_46e0_bf5b_0f45009469d0 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: '9786b6b8-c05d-46e0-bf5b-0f45009469d0'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T20:49:40.7294662Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T20:49:38.2075233Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudokuApi_name_b606e303_f092_4f2d_bce4_4c2922589bc8 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: 'b606e303-f092-4f2d-bce4-4c2922589bc8'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T23:04:39.5348791Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T23:04:25.261796Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudoku_name_dd5d59f9_c3c0_4dad_88a8_7857df8b8501 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: 'dd5d59f9-c3c0-4dad-88a8-7857df8b8501'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-05T04:20:18.9802526Z'
    message: 'OneDeploy'
    start_time: '2026-04-05T04:20:16.7631719Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudokuApi_name_ed70053a_27b5_4a8a_8641_ca0d49323e74 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: 'ed70053a-27b5-4a8a-8641-ca0d49323e74'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T20:50:56.9879934Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T20:50:29.2784045Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudoku_name_efd27343_d6a0_4ef6_8c95_7aab0b109ce7 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: 'efd27343-d6a0-4ef6-8c95-7aab0b109ce7'
  properties: {
    active: false
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-04T20:40:24.2794512Z'
    message: 'OneDeploy'
    start_time: '2026-04-04T20:40:22.148224Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudoku_name_fb01637e_17dc_4b57_8e40_1642a1d3a119 'Microsoft.Web/sites/deployments@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: 'fb01637e-17dc-4b57-8e40-1642a1d3a119'
  properties: {
    active: true
    author: 'N/A'
    author_email: 'N/A'
    deployer: 'OneDeploy'
    end_time: '2026-04-05T04:36:37.117863Z'
    message: 'OneDeploy'
    start_time: '2026-04-05T04:36:35.2976135Z'
    status: 4
  }
}

resource sites_XenobiaSoftSudoku_name_sudoku_xenobiasoft_com 'Microsoft.Web/sites/hostNameBindings@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: 'sudoku.xenobiasoft.com'
  properties: {
    hostNameType: 'Verified'
    siteName: 'XenobiaSoftSudoku'
    sslState: 'SniEnabled'
    thumbprint: '6809B2705350A246BD73DECDF4482ADFD2270828'
  }
}

resource sites_XenobiaSoftSudoku_name_sites_XenobiaSoftSudoku_name_azurewebsites_net 'Microsoft.Web/sites/hostNameBindings@2024-11-01' = {
  parent: sites_XenobiaSoftSudoku_name_resource
  location: 'East US'
  name: '${sites_XenobiaSoftSudoku_name}.azurewebsites.net'
  properties: {
    hostNameType: 'Verified'
    siteName: 'XenobiaSoftSudoku'
  }
}

resource sites_XenobiaSoftSudokuApi_name_sites_XenobiaSoftSudokuApi_name_azurewebsites_net 'Microsoft.Web/sites/hostNameBindings@2024-11-01' = {
  parent: sites_XenobiaSoftSudokuApi_name_resource
  location: 'East US'
  name: '${sites_XenobiaSoftSudokuApi_name}.azurewebsites.net'
  properties: {
    hostNameType: 'Verified'
    siteName: 'XenobiaSoftSudokuApi'
  }
}

resource staticSites_xenobiasoft_blog_name_default 'Microsoft.Web/staticSites/basicAuth@2024-11-01' = {
  parent: staticSites_xenobiasoft_blog_name_resource
  location: 'Central US'
  name: 'default'
  properties: {
    applicableEnvironmentsMode: 'SpecifiedEnvironments'
  }
}

resource staticSites_xenobiasoft_blog_name_asuid_xenobiasoft_com 'Microsoft.Web/staticSites/customDomains@2024-11-01' = {
  parent: staticSites_xenobiasoft_blog_name_resource
  location: 'Central US'
  name: 'asuid.xenobiasoft.com'
  properties: {}
}

resource staticSites_xenobiasoft_blog_name_xenobiasoft_com 'Microsoft.Web/staticSites/customDomains@2024-11-01' = {
  parent: staticSites_xenobiasoft_blog_name_resource
  location: 'Central US'
  name: 'xenobiasoft.com'
  properties: {}
}

resource smartdetectoralertrules_failure_anomalies_xenobiasoftsudoku_name_resource 'microsoft.alertsmanagement/smartdetectoralertrules@2021-04-01' = {
  location: 'global'
  name: smartdetectoralertrules_failure_anomalies_xenobiasoftsudoku_name
  properties: {
    actionGroups: {
      groupIds: [
        actionGroups_Application_Insights_Smart_Detection_name_resource.id
      ]
    }
    description: 'Failure Anomalies notifies you of an unusual rise in the rate of failed HTTP requests or dependency calls.'
    detector: {
      id: 'FailureAnomaliesDetector'
    }
    frequency: 'PT1M'
    scope: [
      components_XenobiaSoftSudoku_name_resource.id
    ]
    severity: 'Sev3'
    state: 'Enabled'
  }
}

resource databaseAccounts_cosmos_sudoku_prod_name_sudoku_games 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_sudoku
  name: 'games'
  properties: {
    resource: {
      computedProperties: []
      conflictResolutionPolicy: {
        conflictResolutionPath: '/_ts'
        mode: 'LastWriterWins'
      }
      fullTextPolicy: {
        defaultLanguage: 'en-US'
        fullTextPaths: []
      }
      id: 'games'
      indexingPolicy: {
        automatic: true
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
        fullTextIndexes: []
        includedPaths: [
          {
            path: '/*'
          }
        ]
        indexingMode: 'consistent'
      }
      partitionKey: {
        kind: 'Hash'
        paths: [
          '/gameId'
        ]
        version: 2
      }
      uniqueKeyPolicy: {
        uniqueKeys: []
      }
    }
  }
  dependsOn: [
    databaseAccounts_cosmos_sudoku_prod_name_resource
  ]
}

resource storageAccounts_stxenobiasoft_name_default_sudoku_puzzles 'Microsoft.Storage/storageAccounts/blobServices/containers@2025-06-01' = {
  parent: storageAccounts_stxenobiasoft_name_default
  name: 'sudoku-puzzles'
  properties: {
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    immutableStorageWithVersioning: {
      enabled: false
    }
    publicAccess: 'None'
  }
  dependsOn: [
    storageAccounts_stxenobiasoft_name_resource
  ]
}

resource storageAccounts_stxenobiasoft_name_default_admin_fileshare 'Microsoft.Storage/storageAccounts/fileServices/shares@2025-06-01' = {
  parent: Microsoft_Storage_storageAccounts_fileServices_storageAccounts_stxenobiasoft_name_default
  name: 'admin-fileshare'
  properties: {
    accessTier: 'TransactionOptimized'
    enabledProtocols: 'SMB'
    shareQuota: 6
  }
  dependsOn: [
    storageAccounts_stxenobiasoft_name_resource
  ]
}

resource databaseAccounts_cosmos_sudoku_prod_name_sudoku_games_default 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/throughputSettings@2025-11-01-preview' = {
  parent: databaseAccounts_cosmos_sudoku_prod_name_sudoku_games
  name: 'default'
  properties: {
    resource: {
      throughput: 400
    }
  }
  dependsOn: [
    databaseAccounts_cosmos_sudoku_prod_name_sudoku
    databaseAccounts_cosmos_sudoku_prod_name_resource
  ]
}
