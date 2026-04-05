using '../main.bicep'

// Global
param location = 'WestUS2'
param environment = 'production'

// Compute
param appServicePlanName = 'XenobiasoftServicePlan'
param webAppName = 'XenobiasoftSudoku'
param apiAppName = 'XenobiasoftSudokuApi'
param appServicePlanSku = 'B1'
param customDomainName = 'sudoku.xenobiasoft.com'

// Storage
param storageAccountName = 'stxenobiasoft'
param cosmosDbAccountName = 'cosmos-sudoku-prod'
param cosmosDbEnableFreeTier = true

// Key Vault
param keyVaultName = 'kv-xenobiasoft'

// App Configuration
param appConfigName = 'appcs-xenobiasoft-com'

// Monitoring
param logAnalyticsWorkspaceName = 'log-sudoku-xenobiasoft'
param appInsightsName = 'appi-sudoku-xenobiasoft'
