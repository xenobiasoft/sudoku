using '../main.bicep'

// Global
param location = 'westus2'
param environment = 'staging'

// Compute
param appServicePlanName = 'XenobiasoftServicePlan-staging'
param webAppName = 'XenobiasoftSudoku-staging'
param apiAppName = 'XenobiasoftSudokuApi-staging'
param appServicePlanSku = 'B1'
param enableCustomDomain = false

// Static Web App — targets the staging environment within the prod SWA.
// enableSwaCustomDomain should stay false until:
//   1. The staging environment has been deployed at least once (workflow must run first).
//   2. A CNAME record for sudoku-beta.xenobiasoft.com points to the staging hostname.
param staticWebAppName = 'swa-sudoku-xenobiasoft-prod'
param swaCustomDomainName = 'sudoku-beta.xenobiasoft.com'
param enableSwaCustomDomain = false

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
