using '../main.bicep'

// Global
param location = 'westus2'
param environment = 'prod'

// Compute
param appServicePlanName = 'XenobiasoftServicePlan-prod'
param apiAppName = 'XenobiasoftSudokuApi-prod'
param mcpAppName = 'XenobiasoftSudokuMcp-prod'
param functionAppName = 'XenobiasoftSudokuFunctions-prod'
param functionPlanName = 'XenobiasoftFunctionsPlan-prod'
param appServicePlanSku = 'B1'

// Static Web App
param staticWebAppName = 'swa-sudoku-xenobiasoft-prod'
param swaCustomDomainName = 'sudoku.xenobiasoft.com'
param enableSwaCustomDomain = true

// Storage
param storageAccountName = 'stxenobiasoftprod'
// Migrated off the free tier (2026-07): free tier can only be set at account
// creation and cannot be converted in place, so this points at a newly
// provisioned, serverless, paid account. The original free-tier account
// (cosmos-sudoku-prod) is intentionally left out of this template so Bicep
// never touches it — see docs/runbooks/cosmos-db-tier-migration.md.
param cosmosDbAccountName = 'cosmos-sudoku-prod2'
param cosmosDbEnableFreeTier = false
param cosmosDbServerless = true

// Key Vault
param keyVaultName = 'kv-xenobiasoft-prod'

// App Configuration
param appConfigName = 'appcs-xenobiasoft-prod'

// Monitoring
param logAnalyticsWorkspaceName = 'log-sudoku-xenobiasoft-prod'
param appInsightsName = 'appi-sudoku-xenobiasoft-prod'
param actionGroupName = 'Application Insights Smart Detection-prod'
param alertRuleName = 'failure-anomalies-sudoku-prod'
