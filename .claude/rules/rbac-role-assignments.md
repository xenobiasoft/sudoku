---
paths:
  - "infra/**/*.bicep"
  - "scripts/assign-roles.sh"
  - ".github/workflows/**"
---

# RBAC & Role Assignment Guidelines

## Rule: role assignments live in `scripts/assign-roles.sh`, never in deployment code

All Azure RBAC role assignments — managed identity grants to Key Vault, App
Configuration, Cosmos DB, Storage, etc. — belong in `scripts/assign-roles.sh`.
Do **not** declare `Microsoft.Authorization/roleAssignments` resources in Bicep
(`infra/**`) or grant roles from CI/CD workflow steps.

### Why
- **Recreate churn.** When a resource (storage account, identity, app) is torn
  down and recreated, IaC role assignments referencing the old principal go
  stale and must be cleaned up, then fail to re-grant cleanly — the root cause
  of recurring `403 AuthorizationPermissionMismatch` failures and the
  "clean up stale RBAC on recreate" work.
- **Least privilege for deployments.** Keeping role assignments out of Bicep
  means the deployment principal does not need `Owner` / `User Access
  Administrator` on the resource group.
- **One auditable place.** A single idempotent script is easy to review, re-run,
  and reason about. It is safe to re-run — Azure skips assignments that already
  exist.

### What this means in practice
- Need a new permission? Add an `assign_role` (or `assign_cosmos_role`) line to
  `scripts/assign-roles.sh` and re-run it. Do not add it to a `.bicep` module.
- Provisioning a new app/function that uses managed identity? Fetch its
  principal in the script (`az webapp identity show` / `az functionapp identity
  show`) and add its grants there.
- Bicep should still **enable** the managed identity on a resource
  (`identity: { type: 'SystemAssigned' }`) and output its `principalId` — it
  just must not assign roles.

### Migration note
`infra/modules/functions.bicep` and `infra/modules/mcp.bicep` still contain
`roleAssignments` resources. These should be removed and their grants moved into
`scripts/assign-roles.sh`. (Watch for drift while migrating — e.g. the Key Vault
Secrets User role ID in Bicep must match the canonical
`4633458b-17de-408a-b874-0445c86b69e0` used in the script.)
