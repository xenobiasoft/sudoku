#!/bin/bash
# =============================================================================
# Azure RBAC Role Assignments – Sudoku Production
# =============================================================================
# Run this script once per environment to grant the app service managed
# identities the permissions they need. It is safe to re-run — Azure silently
# skips assignments that already exist.
#
# Usage:
#   chmod +x assign-roles.sh
#   ./assign-roles.sh
#
# Prerequisites:
#   - Azure CLI installed and logged in (az login)
#   - Owner or User Access Administrator role on the resource group
# =============================================================================

SUBSCRIPTION_ID="c0c21e76-a03c-4747-af34-0720b273ff00"
RESOURCE_GROUP="rg-xenobiasoft-sudoku-prod-westus2"

DEVELOPER_OID="79d81279-ad74-4496-a1bc-32636c40c0e3"

WEB_APP_NAME="XenobiasoftSudoku-prod"
API_APP_NAME="XenobiasoftSudokuApi-prod"

KEY_VAULT_NAME="kv-xenobiasoft-prod"
COSMOS_ACCOUNT_NAME="cosmos-sudoku-prod"
APP_CONFIG_NAME="appcs-xenobiasoft-prod"

# Built-in role definition IDs
KEY_VAULT_SECRETS_USER="4633458b-17de-408a-b874-0445c86b69e0"       # Read secret values only
APP_CONFIG_DATA_READER="516239f1-63e1-4d78-a4de-a74fb236a071"       # Read App Configuration keys
COSMOS_DATA_CONTRIBUTOR="00000000-0000-0000-0000-000000000002"      # Cosmos DB Built-in Data Contributor (SQL)

echo "Fetching managed identity principal IDs..."
WEB_APP_PRINCIPAL=$(az webapp identity show \
  --name "$WEB_APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --query principalId --output tsv)

API_APP_PRINCIPAL=$(az webapp identity show \
  --name "$API_APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --query principalId --output tsv)

echo "  Blazor app principal: $WEB_APP_PRINCIPAL"
echo "  API app principal:    $API_APP_PRINCIPAL"
echo ""

KEY_VAULT_ID="/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.KeyVault/vaults/$KEY_VAULT_NAME"
APP_CONFIG_ID="/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.AppConfiguration/configurationStores/$APP_CONFIG_NAME"
COSMOS_ID="/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.DocumentDB/databaseAccounts/$COSMOS_ACCOUNT_NAME"

assign_role() {
  local description=$1
  local assignee=$2
  local role=$3
  local scope=$4
  echo "Assigning: $description"
  az role assignment create \
    --assignee "$assignee" \
    --role "$role" \
    --scope "$scope" \
    --output none 2>/dev/null \
    && echo "  OK" \
    || echo "  Already exists or skipped"
}

assign_cosmos_role() {
  local description=$1
  local assignee=$2
  echo "Assigning: $description"
  az cosmosdb sql role assignment create \
    --account-name "$COSMOS_ACCOUNT_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --role-definition-id "$COSMOS_DATA_CONTRIBUTOR" \
    --principal-id "$assignee" \
    --scope "/" \
    --output none 2>/dev/null \
    && echo "  OK" \
    || echo "  Already exists or skipped"
}

echo "---------------------------------------------------------------------"
echo "Key Vault — Secrets User (read-only)"
echo "---------------------------------------------------------------------"
assign_role "Blazor   → Key Vault" "$WEB_APP_PRINCIPAL" "$KEY_VAULT_SECRETS_USER" "$KEY_VAULT_ID"
assign_role "API      → Key Vault" "$API_APP_PRINCIPAL" "$KEY_VAULT_SECRETS_USER" "$KEY_VAULT_ID"
assign_role "DevEng	  → Key Vault" "$DEVELOPER_OID" "$KEY_VAULT_SECRETS_USER" "$KEY_VAULT_ID"

echo ""
echo "---------------------------------------------------------------------"
echo "App Configuration — Data Reader"
echo "---------------------------------------------------------------------"
assign_role "Blazor   → App Config" "$WEB_APP_PRINCIPAL" "$APP_CONFIG_DATA_READER" "$APP_CONFIG_ID"
assign_role "API      → App Config" "$API_APP_PRINCIPAL" "$APP_CONFIG_DATA_READER" "$APP_CONFIG_ID"
assign_role "DevEng   → App Config" "$DEVELOPER_OID" "$APP_CONFIG_DATA_READER" "$APP_CONFIG_ID"

echo ""
echo "---------------------------------------------------------------------"
echo "Cosmos DB — Built-in Data Contributor (API only)"
echo "---------------------------------------------------------------------"
assign_cosmos_role "API     → Cosmos DB" "$API_APP_PRINCIPAL"
assign_cosmos_role "DevEng 	→ Cosmos DB" "$DEVELOPER_OID"

echo ""
echo "Done! All role assignments are in place for '$RESOURCE_GROUP'."
echo ""
