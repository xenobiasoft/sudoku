---
paths:
  - "infra/**/*.bicep"
  - "scripts/assign-roles.sh"
  - ".github/workflows/**"
---

# RBAC & Role Assignment Guidelines

## Rule: role assignments live in `scripts/assign-roles.sh`, not in Bicep

`scripts/assign-roles.sh` is the **single source of truth** for all Azure RBAC
and Cosmos data-plane role assignments. Do **not** declare
`Microsoft.Authorization/roleAssignments` (or `sqlRoleAssignments`) resources in
Bicep (`infra/**`).

The script is **executed automatically** as the last step of the `deploy-infra`
job in `.github/workflows/main.yml`, after Bicep has (re)created the managed
identities. It can also be run manually. It is idempotent and exits non-zero on
a genuine failure, so the deploy fails loudly if an assignment doesn't apply.

### Why keep assignments out of Bicep (but still automated)
- **Recreate churn.** When a resource (e.g. the Functions app) is deleted and
  recreated, it gets a new managed-identity principal. Bicep `roleAssignments`
  tied to the old principal go stale and fail with
  `RoleAssignmentUpdateNotPermitted`. The script re-fetches principals at run
  time, so it always assigns to the current identity. (The workflow's recreate
  step cleans up the old principal's assignments before Bicep runs.)
- **One auditable place.** A single idempotent script is easy to review and
  re-run, and keeps every grant — across Key Vault, App Config, Storage, Cosmos
  and monitoring — in one file.

### What this means in practice
- Need a new permission? Add an `assign_role` / `assign_cosmos_role` line to
  `scripts/assign-roles.sh`. Do not add a `.bicep` `roleAssignments` resource.
- Provisioning a new app/function that uses managed identity? Fetch its
  principal in the script (`az webapp identity show` / `az functionapp identity
  show`) and add its grants there.
- Bicep should still **enable** the managed identity
  (`identity: { type: 'SystemAssigned' }`) and output its `principalId` — it
  just must not assign roles.

### Implementation notes
- Assignments are created with `az rest` (direct ARM PUT), **not**
  `az role assignment create`: the latter fails with `MissingSubscription` for
  guest/B2B accounts (a known az CLI defect). `az rest` works for both guest
  users (local runs) and the CI service principal.
- The deploy service principal therefore needs permission to create role
  assignments (Owner or User Access Administrator on the RG) — it already has
  this, since the workflow's stale-RBAC cleanup deletes assignments too.
- Built-in role definition IDs must match what Azure actually publishes. Verify
  with `az role definition list --name "<Role Name>" --query "[0].name"`. Note
  the Key Vault Secrets User ID is `4633458b-17de-408a-b874-0445c86b69e6`
  (ends `e6`).
