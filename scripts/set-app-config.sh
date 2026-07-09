#!/bin/bash
# =============================================================================
# Azure App Configuration – Sudoku Settings
# =============================================================================
# Usage:
#   chmod +x set-app-config.sh
#   ./set-app-config.sh                    # push both Development and Production
#   ./set-app-config.sh Development        # push Development only
#   ./set-app-config.sh Production         # push Production only
#
# Prerequisites:
#   - Azure CLI installed and logged in (az login)
#   - Contributor or App Configuration Data Owner role on the App Config store
# =============================================================================

APP_CONFIG_NAME="appcs-xenobiasoft-prod"
TARGET_LABEL=${1:-"both"}

set_key() {
    local label=$1
    local key=$2
    local value=$3
    echo "  [$label] $key = $value"
    az appconfig kv set \
        --name "$APP_CONFIG_NAME" \
        --key "$key" \
        --value "$value" \
        --label "$label" \
        --yes
}

# =============================================================================
# Production settings
# =============================================================================
push_production() {
    local LABEL="Production"
    echo ""
    echo "Setting App Configuration keys (label: $LABEL)"
    echo "---------------------------------------------------------------------"

    # -------------------------------------------------------------------------
    # Cosmos DB
    # -------------------------------------------------------------------------
    set_key "$LABEL" "UseCosmosDb"                   "true"
    set_key "$LABEL" "CosmosDb:DatabaseName"         "sudoku"
    set_key "$LABEL" "CosmosDb:DisableSslValidation" "false"
    set_key "$LABEL" "CosmosDb:AutoCreateContainers" "false"
    # NOTE: this does NOT select credentials. CosmosDbOptions.UseManagedIdentity
    # is read only by CosmosDbService to gate container auto-creation. The API
    # actually authenticates with the account key embedded in the Key Vault
    # secret below.
    set_key "$LABEL" "CosmosDb:UseManagedIdentity"   "true"
    # NOTE: this key is NOT what points the CosmosClient at an account. Nothing
    # reads CosmosDbOptions.AccountEndpoint; the client is built by Aspire's
    # AddAzureCosmosClient("CosmosDb"), which resolves ConnectionStrings:CosmosDb
    # from the Key Vault secret ConnectionStrings--CosmosDb. That secret holds a
    # full AccountEndpoint=...;AccountKey=... connection string and is what a
    # tier migration must change.
    # Kept in sync here only so the value isn't misleadingly stale.
    # See docs/runbooks/cosmos-db-tier-migration.md.
    set_key "$LABEL" "CosmosDb:AccountEndpoint"      "https://cosmos-sudoku-prod2.documents.azure.com:443/"
    set_key "$LABEL" "CosmosDb:ConnectionMode"       "Direct"

    # -------------------------------------------------------------------------
    # Azure Storage
    # -------------------------------------------------------------------------
    set_key "$LABEL" "AzureStorage:ContainerName"        "sudoku-games"
    set_key "$LABEL" "AzureStorage:UseManagedIdentity"   "true"
    set_key "$LABEL" "AzureStorage:AccountName"          "stxenobiasoftprod"

    # -------------------------------------------------------------------------
    # API & CORS
    # -------------------------------------------------------------------------
    set_key "$LABEL" "ApiBaseUrl"                "https://xenobiasoftsudokuapi-prod.azurewebsites.net/"
    set_key "$LABEL" "AllowedHosts"              "*"
    set_key "$LABEL" "Cors:AllowedOrigins:0"     "https://sudoku.xenobiasoft.com"
    set_key "$LABEL" "Cors:AllowedOrigins:1"     "https://xenobiasoftsudoku-prod.azurewebsites.net"

    # -------------------------------------------------------------------------
    # Sudoku Game Settings
    # -------------------------------------------------------------------------
    set_key "$LABEL" "Sudoku:Game:DefaultDifficulty"                         "Medium"
    set_key "$LABEL" "Sudoku:Game:MaxHintsPerGame"                           "3"
    set_key "$LABEL" "Sudoku:Game:AutoSaveIntervalSeconds"                   "30"
    set_key "$LABEL" "Sudoku:Game:EnableStatistics"                          "true"
    set_key "$LABEL" "Sudoku:UI:DefaultTheme"                                "Light"
    set_key "$LABEL" "Sudoku:UI:ShowTimer"                                   "true"
    set_key "$LABEL" "Sudoku:UI:EnableAnimations"                            "true"
    set_key "$LABEL" "Sudoku:UI:CellSizePixels"                              "40"
    set_key "$LABEL" "Sudoku:Performance:GameStateCacheTimeoutSeconds"       "300"
    set_key "$LABEL" "Sudoku:Performance:MaxConcurrentGamesPerUser"          "5"
    set_key "$LABEL" "Sudoku:Performance:EnableResponseCompression"          "true"

    # -------------------------------------------------------------------------
    # Logging
    # -------------------------------------------------------------------------
    set_key "$LABEL" "Logging:LogLevel:Default"                              "Information"
    set_key "$LABEL" "Logging:LogLevel:Microsoft.AspNetCore"                 "Warning"
    set_key "$LABEL" "Logging:ApplicationInsights:LogLevel:Default"          "Information"
    set_key "$LABEL" "Logging:ApplicationInsights:LogLevel:Microsoft"        "Warning"

    # -------------------------------------------------------------------------
    # Swagger (disabled in production)
    # -------------------------------------------------------------------------
    set_key "$LABEL" "EnableSwagger" "false"

    echo ""
    echo "Done! Production keys set in '$APP_CONFIG_NAME'."
}

# =============================================================================
# Development settings
# =============================================================================
push_development() {
    local LABEL="Development"
    echo ""
    echo "Setting App Configuration keys (label: $LABEL)"
    echo "---------------------------------------------------------------------"

    # -------------------------------------------------------------------------
    # Cosmos DB (same account as prod, separate container for dev data)
    # -------------------------------------------------------------------------
    set_key "$LABEL" "UseCosmosDb"                   "true"
    set_key "$LABEL" "CosmosDb:DatabaseName"         "sudoku"
    set_key "$LABEL" "CosmosDb:DisableSslValidation" "true"
    set_key "$LABEL" "CosmosDb:AutoCreateContainers" "true"
    set_key "$LABEL" "CosmosDb:UseManagedIdentity"   "false"
    set_key "$LABEL" "CosmosDb:AccountEndpoint"      "https://localhost:8081/"
    set_key "$LABEL" "CosmosDb:ConnectionMode"       "Gateway"

    # -------------------------------------------------------------------------
    # Azure Storage
    # -------------------------------------------------------------------------
    set_key "$LABEL" "AzureStorage:ContainerName"        "sudoku-games"
    set_key "$LABEL" "AzureStorage:UseManagedIdentity"   "true"
    set_key "$LABEL" "AzureStorage:AccountName"          "stxenobiasoftprod"

    # -------------------------------------------------------------------------
    # API (no CORS restriction — API uses AllowAnyOrigin() in Development)
    # -------------------------------------------------------------------------
    set_key "$LABEL" "ApiBaseUrl"       "http://sudoku-api"
    set_key "$LABEL" "AllowedHosts"     "*"

    # -------------------------------------------------------------------------
    # Sudoku Game Settings (same as production)
    # -------------------------------------------------------------------------
    set_key "$LABEL" "Sudoku:Game:DefaultDifficulty"                         "Medium"
    set_key "$LABEL" "Sudoku:Game:MaxHintsPerGame"                           "3"
    set_key "$LABEL" "Sudoku:Game:AutoSaveIntervalSeconds"                   "30"
    set_key "$LABEL" "Sudoku:Game:EnableStatistics"                          "true"
    set_key "$LABEL" "Sudoku:UI:DefaultTheme"                                "Light"
    set_key "$LABEL" "Sudoku:UI:ShowTimer"                                   "true"
    set_key "$LABEL" "Sudoku:UI:EnableAnimations"                            "true"
    set_key "$LABEL" "Sudoku:UI:CellSizePixels"                              "40"
    set_key "$LABEL" "Sudoku:Performance:GameStateCacheTimeoutSeconds"       "300"
    set_key "$LABEL" "Sudoku:Performance:MaxConcurrentGamesPerUser"          "5"
    set_key "$LABEL" "Sudoku:Performance:EnableResponseCompression"          "true"

    # -------------------------------------------------------------------------
    # Logging (more verbose in development)
    # -------------------------------------------------------------------------
    set_key "$LABEL" "Logging:LogLevel:Default"                              "Debug"
    set_key "$LABEL" "Logging:LogLevel:Microsoft.AspNetCore"                 "Information"
    set_key "$LABEL" "Logging:ApplicationInsights:LogLevel:Default"          "Information"
    set_key "$LABEL" "Logging:ApplicationInsights:LogLevel:Microsoft"        "Warning"

    # -------------------------------------------------------------------------
    # Swagger (enabled in development)
    # -------------------------------------------------------------------------
    set_key "$LABEL" "EnableSwagger" "true"

    echo ""
    echo "Done! Development keys set in '$APP_CONFIG_NAME'."
}

# =============================================================================
# Run
# =============================================================================
case "$TARGET_LABEL" in
    "Production")
        push_production
        ;;
    "Development")
        push_development
        ;;
    "both")
        push_production
        push_development
        ;;
    *)
        echo "Unknown label '$TARGET_LABEL'. Valid options: Production, Development, both"
        exit 1
        ;;
esac

echo ""
echo "All done!"
echo ""
