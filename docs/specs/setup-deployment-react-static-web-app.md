# Deploy React App to Azure Static Web App

## Problem Statement

1. The React app and Blazor app should co-exist. The React app will not have a custom domain at this time.
2. The default `.azurestaticapps.net` domain will be used.
3. The React static web app should connect to the existing API.
4. It should use Preview/Staging environments. PR previews should point to the production API.
5. Deploy as an additional step in the existing `main.yml` workflow.
6. Add the Infrastructure-as-Code to the existing `main.bicep`.
7. Separate values per environment, stored as GitHub secrets.
8. The API's CORS `allowedOrigins` should be updated to include the SWA URLs.

---

## 1. Azure Infrastructure (`main.bicep`)

### New Resource: Azure Static Web App

Add a new `Microsoft.Web/staticSites` resource to `main.bicep`:

- **Name**: e.g., `XenobiaSoftSudokuReact`
- **SKU**: `Standard` tier (required for PR preview / staging environments)
- **Location**: `resourceGroup().location`
- **Tags**: `environment: production`, `project: XenobiaSoftSudokuReact`
- **Output**: `staticWebAppUrl` — the default `.azurestaticapps.net` hostname
- **Output**: `staticWebAppApiKey` — deployment token (used by CI/CD)

### Update Existing API CORS (`apiServiceConfig`)

Add the production Static Web App URL to `allowedOrigins` in the `cors` block:

- `https://<swa-default-hostname>` — referenced via the SWA resource's `properties.defaultHostname`

> **Note:** Azure App Service CORS does not support wildcard subdomains, so PR preview URLs (which follow the pattern `https://<name>-<pr-number>.<region>.azurestaticapps.net`) cannot be whitelisted individually. PR preview environments will rely on the production API which already allows the production SWA URL.

---

## 2. GitHub Secrets

Add the following secrets to the GitHub repository:

| Secret Name                          | Description                                                              |
|--------------------------------------|--------------------------------------------------------------------------|
| `AZURE_STATIC_WEB_APP_API_TOKEN`     | Deployment token for the SWA (from Azure portal or Bicep output)        |
| `VITE_API_BASE_URL_PRODUCTION`       | Full URL of the production API (e.g., `https://<api>.azurewebsites.net`) |
| `VITE_API_BASE_URL_PREVIEW`          | API URL for PR preview environments — same value as production for now  |

> Keeping `VITE_API_BASE_URL_PREVIEW` and `VITE_API_BASE_URL_PRODUCTION` as separate secrets preserves flexibility to point previews to a different environment in the future.

---

## 3. Static Web App Configuration (`staticwebapp.config.json`)

Add a new file `Sudoku.React/public/staticwebapp.config.json`:

- Configure SPA fallback routing so all routes that don't match a static file return `index.html`
- Set `navigationFallback` to `index.html`
- Optionally configure response headers (e.g., `X-Frame-Options`, `X-Content-Type-Options`)

---

## 4. Vite Build Configuration

- `VITE_API_BASE_URL` is injected at build time via the CI/CD workflow (not at runtime)
- The `vite.config.ts` proxy is only used for local development — no changes needed for production builds
- Ensure `vite build` output directory (`dist/`) is correctly targeted by the SWA deploy action

---

## 5. CI/CD Workflow (`main.yml`)

Add the following steps to the **existing** workflow after the .NET build/deploy steps:

### Step: Setup Node.js
- Use `actions/setup-node@v4` with Node `20.x` (LTS)
- Cache npm dependencies using `Sudoku.React/package-lock.json`

### Step: Install React dependencies
- Run `npm ci` in `Sudoku.React/`

### Step: Lint React app
- Run `npm run lint` in `Sudoku.React/`

### Step: Test React app
- Run `npm test` in `Sudoku.React/`

### Step: Build React app
- Run `npm run build` in `Sudoku.React/`
- Inject `VITE_API_BASE_URL` as an env var:
  - On `push` to `main`: use `${{ secrets.VITE_API_BASE_URL_PRODUCTION }}`
  - On `pull_request`: use `${{ secrets.VITE_API_BASE_URL_PREVIEW }}`

### Step: Deploy to Azure Static Web App (push to main / PR open & sync)
- Use `Azure/static-web-apps-deploy@v1`
- `action`: `upload`
- `app_location`: `Sudoku.React`
- `output_location`: `dist`
- `skip_app_build`: `true` (build is handled manually above to control env vars)
- `azure_static_web_apps_api_token`: `${{ secrets.AZURE_STATIC_WEB_APP_API_TOKEN }}`

### Step: Close PR staging environment (PR closed)
- Use `Azure/static-web-apps-deploy@v1` with `action: close`
- Triggered on `pull_request` type `closed`
- Tears down the PR preview environment automatically

> **Note:** Ensure the `pull_request` trigger in `main.yml` includes `opened`, `synchronize`, and `closed` types to fully support PR preview environments.

---

## 6. Acceptance Criteria

- [ ] Bicep template provisions the SWA resource without errors
- [ ] Existing API CORS includes the SWA production URL
- [ ] `main.yml` builds, tests, and deploys the React app on every push to `main`
- [ ] Every PR automatically gets a unique preview URL at `*.azurestaticapps.net`
- [ ] PR preview environments are automatically torn down when a PR is closed
- [ ] `VITE_API_BASE_URL` is injected correctly at build time for both production and PR environments
- [ ] React SPA routing works (deep links don't 404) via `staticwebapp.config.json`
- [ ] Existing Blazor app and .NET API deployments are unaffected
