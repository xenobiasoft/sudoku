# Sudoku E2E Tests

Playwright end-to-end tests that verify feature parity between the **Blazor** and **React** front-end applications.

## ⚠️ Staging Environment Blocker

These tests are designed to run against **deployed staging environments**. The staging environment does not yet exist.

Until staging is provisioned, tests can only be run locally against development servers. Once the staging environment is available, configure the following environment variables or GitHub Actions variables:

| Variable | Description |
|---|---|
| `BLAZOR_BASE_URL` | URL of the deployed Blazor app (e.g. `https://blazor-staging.example.com`) |
| `REACT_BASE_URL` | URL of the deployed React app (e.g. `https://react-staging.example.com`) |

## How Tests Work

All tests use **Playwright's network interception** (`page.route()`) to mock API responses. No real API calls are made during a test run. This means:

- Tests are fast and deterministic.
- Tests work against any deployed front-end regardless of whether the API is running.
- Both the Blazor and React apps are exercised against the **same set of behavioural specs**.

## Prerequisites

- Node.js ≥ 22
- `npm install` inside this directory
- `npm run install:browsers` (downloads Playwright's Chromium binary)

## Running Tests Locally

```bash
# Install dependencies
npm install

# Install Playwright browser
npm run install:browsers

# Run all tests against both apps (uses default localhost URLs)
npm test

# Run only against Blazor (requires Blazor dev server on :5000)
npm run test:blazor

# Run only against React (requires React dev server on :5173)
npm run test:react

# Open the HTML report after a test run
npm run test:report
```

## Test Structure

```
Tests/E2E/
├── playwright.config.ts      # Two projects: blazor + react
├── fixtures/
│   ├── game-data.ts          # Shared mock game/player data factories
│   ├── api-mocks.ts          # Playwright route-interception helpers
│   └── app-fixture.ts        # Extended test fixture with app-aware POMs
├── pages/
│   ├── home.page.ts          # Page Object Model – Home
│   ├── new-game.page.ts      # Page Object Model – New Game
│   └── game.page.ts          # Page Object Model – Game
└── tests/
    ├── home.spec.ts          # Home page behaviour
    ├── new-game.spec.ts      # New game creation behaviour
    └── game.spec.ts          # In-game behaviour
```

## CI Integration

The GitHub Actions workflow (`.github/workflows/e2e.yml`) is configured as a **manual trigger** (`workflow_dispatch`) until all tests pass against a stable staging environment. Once staging is live and tests are stable, the trigger can be changed to `push` / `pull_request`.
