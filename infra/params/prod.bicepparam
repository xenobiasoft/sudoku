using '../main.bicep'

// Global
param location = 'westus2'
param environment = 'prod'

// Compute
param appServicePlanName = 'XenobiasoftServicePlan-prod'
param webAppName = 'XenobiasoftSudoku-prod'
param apiAppName = 'XenobiasoftSudokuApi-prod'
param appServicePlanSku = 'B1'
param customDomainName = 'sudoku.xenobiasoft.com'
param enableCustomDomain = true

// Storage
param storageAccountName = 'stxenobiasoftprod'
param cosmosDbAccountName = 'cosmos-sudoku-prod'
param cosmosDbEnableFreeTier = true

// Key Vault
param keyVaultName = 'kv-xenobiasoft-prod'

// App Configuration
param appConfigName = 'appcs-xenobiasoft-prod'

// Monitoring
param logAnalyticsWorkspaceName = 'log-sudoku-xenobiasoft-prod'
param appInsightsName = 'appi-sudoku-xenobiasoft-prod'
param actionGroupName = 'Application Insights Smart Detection-prod'
param alertRuleName = 'failure-anomalies-sudoku-prod'
