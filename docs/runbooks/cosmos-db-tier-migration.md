# Runbook — Migrating `cosmos-sudoku-prod` from Free Tier to Paid (Serverless)

| Field | Value |
|---|---|
| **Date** | 2026-07-09 |
| **Status** | Code/config changes staged on branch `feature/cosmos-tier-migration` — not merged. Phase 0 complete (see below). Phases 1-9 not yet executed. |
| **Owner** | Project maintainer |
| **Related** | [ADR-004 — Cosmos DB as persistence backend](../adr/ADR-004-cosmosdb.md), [.claude/rules/rbac-role-assignments.md](../../.claude/rules/rbac-role-assignments.md) |

## What's already done in this repo

These file changes exist on the feature branch and require no further edits. Everything else below is an Azure CLI step you must run yourself — this environment has no Azure credentials.

- `infra/main.bicep` / `infra/modules/storage.bicep`: added a `cosmosDbServerless` param. When `true`, the account gets the `EnableServerless` capability, the `capacity.totalThroughputLimit` property is omitted entirely (serverless rejects an explicit throughput cap), `isZoneRedundant` is forced off (serverless is single-region and has no AZ support), and the `gamesThroughput`/`profilesThroughput` sub-resources are skipped.
- `infra/params/prod.bicepparam`: now points at `cosmosDbAccountName = 'cosmos-sudoku-prod2'`, `cosmosDbEnableFreeTier = false`, `cosmosDbServerless = true`.
- `scripts/assign-roles.sh`: `COSMOS_ACCOUNT_NAME` updated to `cosmos-sudoku-prod2`.
- `.github/workflows/main.yml`: the Functions-app-recreate step's `COSMOS` variable updated to match.
- `scripts/set-app-config.sh`: `CosmosDb:AccountEndpoint` (Production label) updated to the new endpoint — **but see the warning below: this key is not what re-points the API.**
- `scripts/migrate-cosmos-data.py`: a parameterized bulk-copy / delta-sync / delete-reconciliation script for Phases 3 and 5.2. Run `python scripts/migrate-cosmos-data.py --help` for usage. Requires `pip install "azure-cosmos>=4.5,<5"`.

## ⚠️ The endpoint the API actually uses

**`CosmosDb:AccountEndpoint` is dead config.** Nothing in the codebase reads `CosmosDbOptions.AccountEndpoint`. Changing it in App Configuration — or changing the `CosmosDb__AccountEndpoint` app setting that `compute.bicep` / `functions.bicep` push — has no effect on which account the API talks to.

The `CosmosClient` is constructed by Aspire's `builder.AddAzureCosmosClient("CosmosDb")` (`src/backend/Sudoku.Api/Program.cs:36`), which resolves the **connection string** `ConnectionStrings:CosmosDb`. In production that value comes from the Key Vault secret `ConnectionStrings--CosmosDb` in `kv-xenobiasoft-prod`, loaded by `AddAzureKeyVault` (`Program.cs:28`). Key Vault's configuration provider is registered *after* the App Configuration provider, so it wins.

**The cutover is therefore a Key Vault secret change (Phase 5.3), not an App Config change.** If you had flipped only the App Config key, the cutover would have appeared to succeed, the API would have kept serving from `cosmos-sudoku-prod`, Phase 6 validation would have looked perfectly clean — and Phase 8's `az cosmosdb delete` would have been what finally took production down.

### The API authenticates with an account key, not managed identity

Phase 0 confirmed the secret holds a **full connection string** of the form `AccountEndpoint=…;AccountKey=…`. Three consequences:

- **`CosmosDb:UseManagedIdentity = true` in App Configuration is misleading.** It does not select credentials. `CosmosDbOptions.UseManagedIdentity` is read in exactly one place — `CosmosDbService.cs:182` — where it gates *container auto-creation*. Cosmos authentication is whatever the connection string implies, and it implies key auth.
- **The Cosmos data-plane RBAC grants in `scripts/assign-roles.sh` are vestigial for the API.** They're harmless, and `Sudoku.Functions` may still depend on them, so leave them in place. But they are not what lets the API read and write.
- **`disableLocalAuth` must stay `false` on the new account** (it is — `storage.bicep:170`). Setting it `true` would break the API instantly, since key auth would be refused.

Phase 5.3 must therefore carry the **new account's key**, not just its endpoint. And because the key is embedded in the secret, rotating either account's keys out-of-band will break the app until the secret is updated.

## Why

Azure Cosmos DB's free tier discount can only be selected **at account creation** and cannot be toggled on an existing account. There is no in-place "upgrade" path. The only way off free tier is to stand up a new, non-free account and move the data over.

Decisions locked in for this plan:
- **Cutover style:** brief maintenance window, not dual-write. Acceptable for this app's traffic level and much simpler to script and verify.
- **Capacity mode:** switch to **Serverless** on the new account instead of re-provisioning 400 RU/s × 2 containers. Provisioned throughput would cost a flat ~$45-50/mo the moment free tier is gone, regardless of traffic; serverless bills per request, which fits a low/bursty personal-project workload far better.
- **Account name:** the new account gets a new name, `cosmos-sudoku-prod2`. The old `cosmos-sudoku-prod` account is left completely untouched until the migration is verified, giving a free rollback path with no extra backup step.

## Current state (verified in this repo)

- Resource group: `rg-xenobiasoft-sudoku-prod-westus2`, subscription `c0c21e76-a03c-4747-af34-0720b273ff00`.
- Account: `cosmos-sudoku-prod` — `enableFreeTier: true`, kind `GlobalDocumentDB`, Session consistency, zone-redundant, periodic backup (4h interval / 8h retention / geo-redundant), account-wide `totalThroughputLimit: 1000`.
- Database: `sudoku`. Containers:
  - `games` — partition key `/gameId`, 400 RU/s manual throughput.
  - `profiles` — partition key `/profileId`, 400 RU/s manual throughput.
- `disableLocalAuth: false`, so the key-based migration script works.
- Access: managed identity via **Cosmos DB Built-in Data Contributor** for the API app, Functions app, and the developer principal, granted in `scripts/assign-roles.sh` (not Bicep — see the RBAC rule doc). `Sudoku.Functions` wires up a `CosmosClient` even though its actual writes go to blob storage for the puzzle pool, so its identity needs the same grant.
- **Only the API writes to Cosmos.** `Sudoku.Functions` registers the Cosmos repositories via `AddInfrastructureServices` but resolves only `IPuzzlePoolService` (blob-backed); `Sudoku.McpServer` has no Cosmos dependency at all. Stopping the API is sufficient to freeze all Cosmos writes.
- No TTL on any container, and no ETag/optimistic-concurrency usage, so an upsert-based copy is safe.
- `infra/params/staging.bicepparam` uses a separate, non-free account and leaves `cosmosDbServerless` at its `false` default — staging is unaffected.

## Phase 0 — Confirm the Key Vault secret ✅ complete

```
az keyvault secret list --vault-name kv-xenobiasoft-prod --query "[].name" -o tsv
az keyvault secret show --vault-name kv-xenobiasoft-prod --name ConnectionStrings--CosmosDb --query value -o tsv
```

**Result:** the secret `ConnectionStrings--CosmosDb` holds a full connection string, `AccountEndpoint=https://cosmos-sudoku-prod.documents.azure.com:443/;AccountKey=…`. The API is on **key auth**. See the warning section above for what that implies.

Optionally confirm nothing else supplies the endpoint:
```
az webapp config appsettings list --name XenobiasoftSudokuApi-prod \
  -g rg-xenobiasoft-sudoku-prod-westus2 --query "[?contains(name,'Cosmos')]"
```
Expect to see only the dead `CosmosDb__AccountEndpoint` / `CosmosDb__UseManagedIdentity` pair.

## Phase 1 — Provision the new account via Bicep

1. Push the branch through the pipeline (merge to `main`), or run manually:
   ```
   az deployment group create \
     --resource-group rg-xenobiasoft-sudoku-prod-westus2 \
     --template-file infra/main.bicep \
     --parameters infra/params/prod.bicepparam
   ```
   Bicep only manages resources declared in the template by name, so changing `cosmosDbAccountName` makes this an **additive** deployment for the Cosmos account: it creates `cosmos-sudoku-prod2` fresh and leaves `cosmos-sudoku-prod` alone.

2. **It does not leave the running apps alone.** The same deployment rewrites `CosmosDb__AccountEndpoint` on the API and Functions apps (`compute.bicep:115`, `functions.bicep:169`) to the new account's endpoint and restarts them. This is harmless **only because that app setting is dead config** (see the warning above) — the API's client still resolves the old endpoint from the Key Vault secret. Do not "fix" this by pointing the Key Vault secret at the new account yet; the data isn't there.

3. Confirm the new account and both containers exist:
   ```
   az cosmosdb show --name cosmos-sudoku-prod2 -g rg-xenobiasoft-sudoku-prod-westus2 --query "{capabilities:capabilities, freeTier:enableFreeTier}"
   az cosmosdb sql container list --account-name cosmos-sudoku-prod2 -g rg-xenobiasoft-sudoku-prod-westus2 -d sudoku --query "[].id"
   ```

4. Confirm production is **still serving from the old account** — this is the check that proves step 2's reasoning held:
   ```
   az webapp log tail --name XenobiasoftSudokuApi-prod -g rg-xenobiasoft-sudoku-prod-westus2
   ```
   Load an existing game in the app. It must still work. Also confirm the Functions app started cleanly — its Cosmos client registration reads `ConnectionStrings:CosmosDb`, but `Sudoku.Functions/Program.cs` registers neither a Key Vault nor an App Configuration provider, so where it gets that value is unverified.

5. **Deploy-time fallbacks.** `isZoneRedundant` and `capacity` are already gated on `cosmosDbServerless`. If ARM additionally rejects `enableAutomaticFailover: true` or `enableBurstCapacity: false` on a serverless account, gate those on `!cosmosDbServerless` the same way in `storage.bicep` and redeploy.

## Phase 2 — Grant access on the new account

`scripts/assign-roles.sh` and `.github/workflows/main.yml` already target `cosmos-sudoku-prod2`. Run it after Phase 1's deploy:
```
bash scripts/assign-roles.sh
```
It's idempotent and grants Cosmos Data Contributor to the API app, Functions app, and your developer principal. It also runs automatically as the last step of `deploy-infra` in CI, so a normal `main` push after Phase 1 applies it for you.

Since Phase 0 established that the API authenticates with an account key, these grants are **not** on the critical path for the cutover — the API would work even if they hadn't propagated. Run the script anyway: it keeps the new account's grants consistent with the old, and `Sudoku.Functions` (whose Cosmos credential source is unverified — see Phase 1 step 4) may depend on them.

Cosmos data-plane role assignments propagate slowly, typically a few minutes. Verify they landed:
```
az cosmosdb sql role assignment list --account-name cosmos-sudoku-prod2 -g rg-xenobiasoft-sudoku-prod-westus2 -o table
```

## Phase 3 — Bulk-copy existing data

Fetch each account's primary key:
```
az cosmosdb keys list --name cosmos-sudoku-prod  -g rg-xenobiasoft-sudoku-prod-westus2 --query primaryMasterKey -o tsv
az cosmosdb keys list --name cosmos-sudoku-prod2 -g rg-xenobiasoft-sudoku-prod-westus2 --query primaryMasterKey -o tsv
```
Then run the bulk copy (add `--dry-run` first for a count-only pass):
```
python scripts/migrate-cosmos-data.py \
  --src-endpoint https://cosmos-sudoku-prod.documents.azure.com:443/ --src-key <old-key> \
  --dst-endpoint https://cosmos-sudoku-prod2.documents.azure.com:443/ --dst-key <new-key> \
  --database sudoku --containers games profiles
```
It prints a start epoch — **record it**, you'll pass it as `--since` in Phase 5.2.

Notes:
- `upsert_item` preserves `id` and the partition key field (`gameId` / `profileId`) exactly, which is all `CosmosDbGameRepository` / `CosmosDbUserProfileRepository` rely on. Cosmos system properties (`_rid`, `_self`, `_etag`, `_attachments`, `_ts`) are stripped before writing; the destination regenerates them.
- Keys are passed as CLI args or `SRC_KEY`/`DST_KEY` env vars — don't commit them anywhere.
- The copy reads the source with `read_all_items()`, consuming RU on the old account's 400 RU/s containers. The SDK retries throttles automatically; expect it to be slow rather than to fail.

## Phase 4 — Verify the copy

- Compare item counts per container: `SELECT VALUE COUNT(1) FROM c` against both accounts (Data Explorer or SDK). Traffic is still live, so the new account will be slightly behind. That's expected and handled in Phase 5.
- Spot-check a handful of `games` and `profiles` documents by `id`. Compare **domain fields only** — `_ts`, `_etag`, `_rid`, and `_self` are regenerated by the destination and will always differ, so a byte-for-byte comparison is meaningless.
- Do **not** cut over yet.

## Phase 5 — Cutover (maintenance window)

Pick a low-traffic time.

**5.1 — Stop writes.** Stop the API App Service so no `MakeMove`/`CreateGame`/profile-update calls land during the sync. A few minutes of API downtime. The Functions and MCP apps don't write to Cosmos and can stay up.
```
az webapp stop --name XenobiasoftSudokuApi-prod -g rg-xenobiasoft-sudoku-prod-westus2
```

**5.2 — Delta sync with delete reconciliation.** Re-run the script with the epoch from Phase 3, plus `--prune`:
```
python scripts/migrate-cosmos-data.py \
  --src-endpoint https://cosmos-sudoku-prod.documents.azure.com:443/ --src-key <old-key> \
  --dst-endpoint https://cosmos-sudoku-prod2.documents.azure.com:443/ --dst-key <new-key> \
  --database sudoku --containers games profiles --since <epoch-from-phase-3> --prune
```

`--prune` is not optional. `DeleteGameCommandHandler`, `DeletePlayerGamesCommandHandler`, and `DeleteProfileCommandHandler` all hard-delete with no tombstone, so a `--since` delta cannot observe a deletion — the document is simply absent from the source query, and the copy made during Phase 3 survives on the destination. Without `--prune`, any game or profile deleted between the bulk copy and now **reappears** on the new account, and a resurrected profile can collide with the alias-uniqueness query in `CosmosDbUserProfileRepository`. `--prune` enumerates both sides and deletes destination documents the source no longer has. It is only safe because 5.1 stopped writes.

Run it once with `--dry-run --prune` first and read the list of documents it would delete.

**5.3 — Flip the Key Vault secret.** This is the actual cutover. Phase 0 established the value is a full connection string, so it must carry the new account's **endpoint and key**.

First record the current value — you need it verbatim to roll back (Phase 7), and once Phase 8 deletes the old account its key dies with it:
```
az keyvault secret show --vault-name kv-xenobiasoft-prod \
  --name ConnectionStrings--CosmosDb --query value -o tsv
```
Keep it somewhere outside the repo. Key Vault also retains the prior version automatically — `az keyvault secret list-versions --vault-name kv-xenobiasoft-prod --name ConnectionStrings--CosmosDb` — so this is belt-and-braces.

Then fetch the new account's key and write the new secret:
```
NEW_KEY=$(az cosmosdb keys list --name cosmos-sudoku-prod2 \
  -g rg-xenobiasoft-sudoku-prod-westus2 --query primaryMasterKey -o tsv)

az keyvault secret set --vault-name kv-xenobiasoft-prod \
  --name ConnectionStrings--CosmosDb \
  --value "AccountEndpoint=https://cosmos-sudoku-prod2.documents.azure.com:443/;AccountKey=${NEW_KEY};"
```
Match the old value's exact shape (trailing `;`, any `Database=` segment) rather than assuming the form above. `az keyvault secret set` writes the value as a command-line argument, so it will land in your shell history — clear it afterwards.

Optionally also run `scripts/set-app-config.sh` (manual-only, not part of CI) so the dead `CosmosDb:AccountEndpoint` key isn't left stale. It changes nothing functionally.

**5.4 — Restart the API.**
```
az webapp start --name XenobiasoftSudokuApi-prod -g rg-xenobiasoft-sudoku-prod-westus2
```
The restart is **required**, not merely advisable: the Key Vault configuration provider reads once at startup and does not poll, and the `CosmosClient` is a singleton built from that value at registration time. Nothing picks up the new secret without a process restart.

**5.5 — Smoke test.** The critical check is that **pre-existing data is present under the new account** — that's what proves Phases 3 and 5.2 worked:

1. Load a game that existed *before* the migration. It must open normally. If it 404s, the copy or the cutover failed — roll back (Phase 7).
2. Confirm the profile list and its aliases look right, and that nothing you deleted before the migration has reappeared (this verifies `--prune`).
3. Create a new game and make a move.
4. Check Application Insights for `CosmosException` immediately after restart.

## Phase 6 — Post-cutover validation

- **Confirm you are actually on the new account.** The failure mode this runbook exists to prevent is a cutover that silently didn't happen — and a cutover that didn't happen produces *no errors at all*. `CosmosDbService.InitializeCosmosDbAsync` logs the endpoint it connected to (`CosmosDbService.cs:180`), so check it directly:
  ```
  az webapp log tail --name XenobiasoftSudokuApi-prod -g rg-xenobiasoft-sudoku-prod-westus2 \
    | grep "Initializing CosmosDB at endpoint"
  ```
  It must name `cosmos-sudoku-prod2`. Corroborate with metrics: requests on the new account non-zero, requests on the old account dropping to zero.
- Watch Application Insights / logs for `CosmosException` or the `InvalidOperationException` thrown by `InitializeCosmosDbAsync` (missing database/container) for the first hour of real traffic. A `401`/`403` here means the connection string's key doesn't match the new account.
- Confirm the new account's metrics show request charges (serverless: per-operation RU, no throttling).
- Leave `cosmos-sudoku-prod` **running and untouched** — this is your rollback path.

## Phase 7 — Rollback (if something's wrong post-cutover)

Restore the connection string you recorded in Phase 5.3 — endpoint **and** the old account's key — then restart the API:
```
az keyvault secret set --vault-name kv-xenobiasoft-prod \
  --name ConnectionStrings--CosmosDb \
  --value "<the exact value captured in Phase 5.3>"
az webapp restart --name XenobiasoftSudokuApi-prod -g rg-xenobiasoft-sudoku-prod-westus2
```
If you didn't capture it, recover the previous version:
```
az keyvault secret list-versions --vault-name kv-xenobiasoft-prod --name ConnectionStrings--CosmosDb -o table
az keyvault secret show --vault-name kv-xenobiasoft-prod --name ConnectionStrings--CosmosDb --version <prior-version-id> --query value -o tsv
```
Confirm the rollback took using the endpoint log line from Phase 6.

Any writes that landed on the new account during the failed window must be manually replayed back to the old account (same script, endpoints reversed, with `--since` set to the cutover epoch) before re-attempting. This is the cost of the maintenance-window approach vs. dual-write — keep the window short.

## Phase 8 — Decommission the old account

Only after at least 1-2 weeks of clean operation on the new account, and only once Phase 6 has **positively confirmed via the endpoint log line** that traffic is hitting `cosmos-sudoku-prod2`. This is the irreversible step, and the whole point of Phase 6's check is to stop you taking it while the API is quietly still on the old account.

1. `az cosmosdb delete --name cosmos-sudoku-prod --resource-group rg-xenobiasoft-sudoku-prod-westus2`
2. This also frees the subscription's one free-tier slot.

After this, Phase 7 rollback is gone: the old account's access key dies with the account, so the connection string you saved is worthless.

## Phase 9 — Cleanup

- **Remove the dead Cosmos config.** `CosmosDbOptions.AccountEndpoint`, `.DisableSslValidation`, and `.ConnectionMode` are bound but never read, as is the `CosmosDb__AccountEndpoint` app setting pushed by `compute.bicep` and `functions.bicep`. Deleting them would have prevented the entire class of confusion this runbook's warning section describes.
- **Decide on Cosmos authentication.** The API uses an account key while `CosmosDb:UseManagedIdentity` reads `true` — a flag that only gates container auto-creation (`CosmosDbService.cs:182`) and has nothing to do with credentials. The managed identities already hold Cosmos Data Contributor via `assign-roles.sh`, so switching the Key Vault secret to a bare endpoint URI would let `DefaultAzureCredential` take over and remove the embedded key entirely. Worth doing as a follow-up, but **not** during this migration — one variable at a time.
- Clarify how `Sudoku.Functions` obtains `ConnectionStrings:CosmosDb`; it registers a `CosmosClient` but no Key Vault or App Configuration provider.
- Remove the now-unused `cosmosDbEnableFreeTier` parameter from `infra/main.bicep` and `infra/modules/storage.bicep`, or leave it defaulted to `false` — harmless either way.
- Cosmos account names are immutable, so the `2` suffix is permanent short of another full migration. Not worth doing.
- Add a note to [ADR-004](../adr/ADR-004-cosmosdb.md) recording the account rename and the serverless switch. Its "Cost model" section currently doesn't mention capacity mode at all.

## Things to double-check before running this for real

- **Phase 0 first.** The name and value format of the `ConnectionStrings--CosmosDb` secret determine Phase 5.3 entirely.
- Whether Azure accepts the existing backup policy, `enableAutomaticFailover`, and `enableBurstCapacity` on a **serverless** account in `westus2` — confirm at Phase 1 deploy time and gate on `!cosmosDbServerless` if not.
- Exact current document counts in `games` and `profiles` (Phase 4) before trusting the copy.
- That no other environment or local dev config points at `cosmos-sudoku-prod`'s endpoint directly (checked: the `Development` label in `set-app-config.sh` and `appsettings.Development.json` both point at the Cosmos emulator, `https://localhost:8081/` — local dev is unaffected).
