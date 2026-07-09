#!/bin/bash
# =============================================================================
# Azure RBAC Role Assignments – Sudoku Production
# =============================================================================
# Grants the app service / function managed identities the permissions they
# need. This is the single source of truth for role assignments (they are NOT
# declared in Bicep — see .claude/rules/rbac-role-assignments.md).
#
# It runs automatically as the last step of the `deploy-infra` job in
# .github/workflows/main.yml, and can also be run manually. It is safe to
# re-run — assignments that already exist are detected and skipped, and genuine
# failures are printed and cause a non-zero exit.
#
# Assignments are created via `az rest` (a direct ARM call) rather than
# `az role assignment create` on purpose: the latter fails with
# `MissingSubscription` for guest/B2B accounts (a known az CLI defect), whereas
# `az rest` works for both guest users and the CI service principal.
#
# Usage:
#   chmod +x assign-roles.sh
#   ./assign-roles.sh
#
# Prerequisites:
#   - Azure CLI installed and logged in (az login)
#   - Owner or User Access Administrator role on the resource group
#   - A GUID generator: uuidgen, /proc/sys/kernel/random/uuid, or python
# =============================================================================

# -u: fail on unset variables (catches typos / missing principals).
# pipefail: a failing command in a pipeline fails the pipeline.
# (Intentionally NOT -e: we attempt every assignment and report failures at the
# end rather than aborting on the first error.)
set -uo pipefail

# Set to 1 by the helpers when an assignment genuinely fails, so the script can
# exit non-zero at the end instead of silently reporting success.
FAILED=0

SUBSCRIPTION_ID="c0c21e76-a03c-4747-af34-0720b273ff00"
RESOURCE_GROUP="rg-xenobiasoft-sudoku-prod-westus2"

DEVELOPER_OID="79d81279-ad74-4496-a1bc-32636c40c0e3"

API_APP_NAME="XenobiasoftSudokuApi-prod"
FUNCTIONS_APP_NAME="xenobiasoftsudokufunctions-prod"
MCP_APP_NAME="xenobiasoftsudokumcp-prod"

KEY_VAULT_NAME="kv-xenobiasoft-prod"
# Migrated off the free tier (2026-07) to a new serverless paid account; the
# old cosmos-sudoku-prod account is left running as a rollback copy until the
# migration is verified — see docs/runbooks/cosmos-db-tier-migration.md.
COSMOS_ACCOUNT_NAME="cosmos-sudoku-prod2"
APP_CONFIG_NAME="appcs-xenobiasoft-prod"
STORAGE_ACCOUNT_NAME="stxenobiasoftprod"

# Built-in role definition IDs
KEY_VAULT_SECRETS_USER="4633458b-17de-408a-b874-0445c86b69e6"       # Read secret values only
APP_CONFIG_DATA_READER="516239f1-63e1-4d78-a4de-a74fb236a071"       # Read App Configuration keys
COSMOS_DATA_CONTRIBUTOR="00000000-0000-0000-0000-000000000002"      # Cosmos DB Built-in Data Contributor (SQL)
STORAGE_BLOB_DATA_CONTRIBUTOR="ba92f5b4-2d11-453d-a403-e96b0029c9fe" # Read/write blob data
STORAGE_BLOB_DATA_OWNER="b7e6dc6d-f1e8-4753-8033-0f276bb0955b"      # Full blob data-plane (Functions host: AzureWebJobsStorage + leases)
LOG_ANALYTICS_READER="73c42c96-874c-492b-b04d-ab87d138a893"        # Query Log Analytics workspace
MONITORING_READER="43d0d8ad-25c7-4714-9337-8ba259a9fe05"          # Read Application Insights metrics

echo "Fetching managed identity principal IDs..."
API_APP_PRINCIPAL=$(az webapp identity show \
  --name "$API_APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --query principalId --output tsv)

FUNCTIONS_APP_PRINCIPAL=$(az functionapp identity show \
  --name "$FUNCTIONS_APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --query principalId --output tsv)

MCP_APP_PRINCIPAL=$(az webapp identity show \
  --name "$MCP_APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --query principalId --output tsv)

echo "  API app principal:        $API_APP_PRINCIPAL"
echo "  Functions app principal:  $FUNCTIONS_APP_PRINCIPAL"
echo "  MCP app principal:        $MCP_APP_PRINCIPAL"
echo ""

KEY_VAULT_ID="/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.KeyVault/vaults/$KEY_VAULT_NAME"
APP_CONFIG_ID="/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.AppConfiguration/configurationStores/$APP_CONFIG_NAME"
COSMOS_ID="/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.DocumentDB/databaseAccounts/$COSMOS_ACCOUNT_NAME"
STORAGE_ID="/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Storage/storageAccounts/$STORAGE_ACCOUNT_NAME"
RESOURCE_GROUP_ID="/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP"

ARM="https://management.azure.com"
RBAC_API_VERSION="2022-04-01"
COSMOS_API_VERSION="2024-05-15"

# Generate a lowercase GUID for the role assignment name. Avoids python on
# purpose: on Windows `python`/`python3` is often a Microsoft Store stub that
# prints "Python was not found" and exits 0, which would poison the name. The
# /dev/urandom fallback works in Git Bash and on CI runners with no extra tools.
new_guid() {
  if command -v uuidgen >/dev/null 2>&1; then
    uuidgen | tr '[:upper:]' '[:lower:]'
  elif [[ -r /proc/sys/kernel/random/uuid ]]; then
    cat /proc/sys/kernel/random/uuid
  else
    local hex
    hex=$(head -c16 /dev/urandom | od -An -tx1 | tr -d ' \n')
    printf '%s-%s-%s-%s-%s\n' "${hex:0:8}" "${hex:8:4}" "${hex:12:4}" "${hex:16:4}" "${hex:20:12}"
  fi
}

# Azure RBAC assignment via a direct ARM PUT. ARM enforces uniqueness of
# (principal, role, scope) regardless of the assignment name, so a fresh random
# name re-creates idempotently — an existing assignment returns
# RoleAssignmentExists, which we treat as a skip.
assign_role() {
  local description=$1
  local assignee=$2
  local role=$3
  local scope=$4
  echo "Assigning: $description"
  if [[ -z "$assignee" ]]; then
    echo "  FAILED: empty principal ID — is the managed identity enabled on this app?"
    FAILED=1
    return
  fi

  # The developer is a User; the apps are managed identities (ServicePrincipal).
  # Stating the type lets ARM skip the directory lookup, which also avoids
  # replication-delay failures right after an identity is (re)created.
  local principal_type="ServicePrincipal"
  [[ "$assignee" == "$DEVELOPER_OID" ]] && principal_type="User"

  local guid output
  guid=$(new_guid) || { FAILED=1; return; }
  local url="${ARM}${scope}/providers/Microsoft.Authorization/roleAssignments/${guid}?api-version=${RBAC_API_VERSION}"
  local body="{\"properties\":{\"roleDefinitionId\":\"/subscriptions/${SUBSCRIPTION_ID}/providers/Microsoft.Authorization/roleDefinitions/${role}\",\"principalId\":\"${assignee}\",\"principalType\":\"${principal_type}\"}}"

  if output=$(az rest --method put --url "$url" --body "$body" --output none 2>&1); then
    echo "  OK"
  elif grep -qiE 'RoleAssignmentExists' <<<"$output"; then
    echo "  Already exists — skipped"
  else
    echo "  FAILED:"
    sed 's/^/    /' <<<"$output"
    FAILED=1
  fi
}

# Cosmos DB data-plane (SQL) role assignment via ARM PUT. These are NOT Azure
# RBAC and ARM does NOT enforce (principal, role, scope) uniqueness, so a blind
# re-create would accumulate duplicates. Check for an existing assignment first.
assign_cosmos_role() {
  local description=$1
  local assignee=$2
  echo "Assigning: $description"
  if [[ -z "$assignee" ]]; then
    echo "  FAILED: empty principal ID — is the managed identity enabled on this app?"
    FAILED=1
    return
  fi

  local existing
  existing=$(az rest --method get \
    --url "${ARM}${COSMOS_ID}/sqlRoleAssignments?api-version=${COSMOS_API_VERSION}" \
    --query "length(value[?properties.principalId=='${assignee}' && contains(properties.roleDefinitionId, '${COSMOS_DATA_CONTRIBUTOR}')])" \
    -o tsv 2>/dev/null || echo 0)
  if [[ "${existing:-0}" != "0" ]]; then
    echo "  Already exists — skipped"
    return
  fi

  local guid output
  guid=$(new_guid) || { FAILED=1; return; }
  local url="${ARM}${COSMOS_ID}/sqlRoleAssignments/${guid}?api-version=${COSMOS_API_VERSION}"
  local body="{\"properties\":{\"roleDefinitionId\":\"${COSMOS_ID}/sqlRoleDefinitions/${COSMOS_DATA_CONTRIBUTOR}\",\"principalId\":\"${assignee}\",\"scope\":\"${COSMOS_ID}\"}}"

  if output=$(az rest --method put --url "$url" --body "$body" --output none 2>&1); then
    echo "  OK"
  else
    echo "  FAILED:"
    sed 's/^/    /' <<<"$output"
    FAILED=1
  fi
}

echo "---------------------------------------------------------------------"
echo "Key Vault — Secrets User (read-only)"
echo "---------------------------------------------------------------------"
assign_role "API       → Key Vault" "$API_APP_PRINCIPAL"       "$KEY_VAULT_SECRETS_USER" "$KEY_VAULT_ID"
assign_role "Functions → Key Vault" "$FUNCTIONS_APP_PRINCIPAL" "$KEY_VAULT_SECRETS_USER" "$KEY_VAULT_ID"
assign_role "DevEng    → Key Vault" "$DEVELOPER_OID"           "$KEY_VAULT_SECRETS_USER" "$KEY_VAULT_ID"

echo ""
echo "---------------------------------------------------------------------"
echo "App Configuration — Data Reader"
echo "---------------------------------------------------------------------"
assign_role "API       → App Config" "$API_APP_PRINCIPAL"       "$APP_CONFIG_DATA_READER" "$APP_CONFIG_ID"
assign_role "Functions → App Config" "$FUNCTIONS_APP_PRINCIPAL" "$APP_CONFIG_DATA_READER" "$APP_CONFIG_ID"
assign_role "DevEng    → App Config" "$DEVELOPER_OID"           "$APP_CONFIG_DATA_READER" "$APP_CONFIG_ID"

echo ""
echo "---------------------------------------------------------------------"
echo "Cosmos DB — Built-in Data Contributor"
echo "---------------------------------------------------------------------"
assign_cosmos_role "API       → Cosmos DB" "$API_APP_PRINCIPAL"
assign_cosmos_role "Functions → Cosmos DB" "$FUNCTIONS_APP_PRINCIPAL"
assign_cosmos_role "DevEng    → Cosmos DB" "$DEVELOPER_OID"

echo ""
echo "---------------------------------------------------------------------"
echo "Storage — Blob Data (read/write game and puzzle blobs)"
echo "---------------------------------------------------------------------"
# API reads/writes puzzle + game blobs → Contributor is sufficient.
# Functions host needs Owner: AzureWebJobsStorage, deployment container, and
# timer-singleton leases require full blob data-plane access on the Flex host.
assign_role "API       → Storage Blob Data Contributor" "$API_APP_PRINCIPAL"       "$STORAGE_BLOB_DATA_CONTRIBUTOR" "$STORAGE_ID"
assign_role "Functions → Storage Blob Data Owner"       "$FUNCTIONS_APP_PRINCIPAL" "$STORAGE_BLOB_DATA_OWNER"       "$STORAGE_ID"
assign_role "DevEng    → Storage Blob Data Contributor" "$DEVELOPER_OID"           "$STORAGE_BLOB_DATA_CONTRIBUTOR" "$STORAGE_ID"

echo ""
echo "---------------------------------------------------------------------"
echo "Monitoring — MCP server reads Log Analytics / App Insights (RG scope)"
echo "---------------------------------------------------------------------"
assign_role "MCP       → Log Analytics Reader" "$MCP_APP_PRINCIPAL" "$LOG_ANALYTICS_READER" "$RESOURCE_GROUP_ID"
assign_role "MCP       → Monitoring Reader"    "$MCP_APP_PRINCIPAL" "$MONITORING_READER"    "$RESOURCE_GROUP_ID"

echo ""
if [[ "$FAILED" -ne 0 ]]; then
  echo "FAILED: one or more role assignments did not complete — review the errors above."
  exit 1
fi
echo "Done! All role assignments are in place for '$RESOURCE_GROUP'."
echo ""
