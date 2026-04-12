#!/bin/bash
# =============================================================================
# Azure App Configuration – Sudoku Production Settings
# =============================================================================
# Usage:
#   chmod +x set-app-config.sh
#   ./set-app-config.sh
#
# Prerequisites:
#   - Azure CLI installed and logged in (az login)
#   - Contributor or App Configuration Data Owner role on the App Config store
# =============================================================================

APP_CONFIG_NAME="appcs-xenobiasoft-prod"
LABEL="Production"

echo "Setting App Configuration keys for: $APP_CONFIG_NAME (label: $LABEL)"
echo "---------------------------------------------------------------------"

set_key() {
    local key=$1
    local value=$2
    echo "Setting: $key = $value"
    az appconfig kv set \
        --name "$APP_CONFIG_NAME" \
        --key "$key" \
        --value "$value" \
        --label "$LABEL" \
        --yes
}

# -----------------------------------------------------------------------------
# App Configuration Bootstrap
# -----------------------------------------------------------------------------
set_key "appconfig:Endpoint"                              "https://appcs-xenobiasoft-prod.azconfig.io"
set_key "appconfig:ManagedIdentityEnabled"                "true"
set_key "AzureAppConfiguration:KeyFilter"                 "*"
set_key "AzureAppConfiguration:LabelFilter"               "Production"
set_key "AzureAppConfiguration:RefreshInterval"           "30"
set_key "AzureAppConfiguration:FeatureFlags:Enabled"      "true"
set_key "AzureAppConfiguration:FeatureFlags:RefreshInterval" "30"

# -----------------------------------------------------------------------------
# Cosmos DB
# -----------------------------------------------------------------------------
set_key "UseCosmosDb"                   "true"
set_key "CosmosDb:DatabaseName"         "sudoku"
set_key "CosmosDb:ContainerName"        "games"
set_key "CosmosDb:UseManagedIdentity"   "true"
set_key "CosmosDb:AccountEndpoint"      "https://cosmos-sudoku-prod.documents.azure.com:443/"

# -----------------------------------------------------------------------------
# Azure Storage
# -----------------------------------------------------------------------------
set_key "AzureStorage:ContainerName"        "sudoku-games"
set_key "AzureStorage:UseManagedIdentity"   "true"
set_key "AzureStorage:AccountName"          "stxenobiasoftprod"

# -----------------------------------------------------------------------------
# API & CORS
# -----------------------------------------------------------------------------
set_key "ApiBaseUrl"                "https://xenobiasoftsudokuapi.azurewebsites.net/"
set_key "AllowedHosts"              "*"
set_key "Cors:AllowedOrigins:0"     "https://sudoku.xenobiasoft.com"
set_key "Cors:AllowedOrigins:1"     "https://xenobiasoftsudoku.azurewebsites.net"

# -----------------------------------------------------------------------------
# Sudoku Game Settings
# -----------------------------------------------------------------------------
set_key "Sudoku:Game:DefaultDifficulty"             "Medium"
set_key "Sudoku:Game:MaxHintsPerGame"               "3"
set_key "Sudoku:Game:AutoSaveIntervalSeconds"       "30"
set_key "Sudoku:Game:EnableStatistics"              "true"
set_key "Sudoku:UI:DefaultTheme"                    "Light"
set_key "Sudoku:UI:ShowTimer"                       "true"
set_key "Sudoku:UI:EnableAnimations"                "true"
set_key "Sudoku:UI:CellSizePixels"                  "40"
set_key "Sudoku:Performance:GameStateCacheTimeoutSeconds"    "300"
set_key "Sudoku:Performance:MaxConcurrentGamesPerUser"      "5"
set_key "Sudoku:Performance:EnableResponseCompression"      "true"

# -----------------------------------------------------------------------------
# Logging
# -----------------------------------------------------------------------------
set_key "Logging:LogLevel:Default"                              "Information"
set_key "Logging:LogLevel:Microsoft.AspNetCore"                 "Warning"
set_key "Logging:ApplicationInsights:LogLevel:Default"          "Information"
set_key "Logging:ApplicationInsights:LogLevel:Microsoft"        "Warning"

# -----------------------------------------------------------------------------
# Swagger (temporary — disable when done testing)
# -----------------------------------------------------------------------------
set_key "EnableSwagger" "true"

echo ""
echo "Done! All keys set under label '$LABEL' in '$APP_CONFIG_NAME'."
echo ""
echo "REMINDERS:"
echo "  - Update CosmosDb:AccountEndpoint with your actual WestUS2 Cosmos DB endpoint"
echo "  - Update AzureStorage:AccountName with your actual WestUS2 storage account name"
echo "  - Set EnableSwagger back to 'false' when you're done testing"
