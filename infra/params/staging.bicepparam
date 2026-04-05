using '../main.bicep'

// Global
param location = 'WestUS2'
param environment = 'staging'

// Compute
param appServicePlanName = 'XenobiasoftServicePlan-staging'
param webAppName = 'XenobiasoftSudoku-staging'
param apiAppName = 'XenobiasoftSudokuApi-staging'
param appServicePlanSku = 'B1'
param enableCustomDomain = false

// Storage
param storageAccountName = 'stxenobiasoftstaging'
param cosmosDbAccountName = 'cosmos-sudoku-staging'
param cosmosDbEnableFreeTier = false

// Key Vault
param keyVaultName = 'kv-xenobiasoft-staging'

// App Configuration
param appConfigName = 'appcs-xenobiasoft-staging'

// Monitoring
param logAnalyticsWorkspaceName = 'log-sudoku-xenobiasoft-staging'
param appInsightsName = 'appi-sudoku-xenobiasoft-staging'
param actionGroupName = 'Application Insights Smart Detection-staging'
param alertRuleName = 'failure-anomalies-sudoku-staging'
