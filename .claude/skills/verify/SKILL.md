---
name: verify
description: Build, launch, and drive the Sudoku app end-to-end to observe a change actually working (React UI + API + emulators).
---

# Verifying a change in this repo

## Launch the whole stack (one command)

`Sudoku.AppHost` is an Aspire host that brings up **everything** — Cosmos DB emulator,
Azurite storage, the API, the Functions pool-seeder, and the React dev server:

```bash
dotnet run --project src/backend/Sudoku.AppHost
```

- **Requires Docker running** (it starts the Cosmos + storage emulator containers).
- Takes ~60–90s cold. Wait on the React dev server rather than sleeping:
  `until curl -s -o /dev/null http://localhost:5173; do sleep 3; done`
- React: `http://localhost:5173`. API: `https://localhost:7272` (**self-signed cert** —
  use `curl -k`, or `ignoreHTTPSErrors: true` / `NODE_TLS_REJECT_UNAUTHORIZED=0`).
- Aspire dashboard URL + login token are printed in the AppHost stdout.
- Teardown: kill `Sudoku.AppHost.exe`, then `docker stop <the two containers it named>`.
  Stop them **by name** — never `docker ps -q | xargs docker stop`.

## Driving the UI

There is no Playwright in the repo. Install it in a scratch dir (not the project — it would
dirty `package.json`) and drive `http://localhost:5173` headless.

**The app gates on having a profile.** A fresh browser context is a "new player", and
`SelectDifficultyPage` / `NewGamePage` both redirect to `/` until an alias exists. So the
first steps of any UI flow are always:

1. Home → click the **Profile** card (goes to `/create-profile`)
2. Fill the single textbox, click **Begin**
3. *Then* navigate to `/select-difficulty` etc.

Skip that and you'll silently land back on the home page and your selectors won't match.

## Gotchas worth knowing

- **StrictMode double-fires effects in dev.** `NewGamePage`'s create-game effect has no
  concurrency guard, so **every new game POSTs twice and creates two games** in dev. This is
  not difficulty-specific and not a regression — don't chase it as a bug in your change.
  (A production build won't double-invoke.)
- **A fresh local puzzle pool is empty**, so the first game per difficulty takes the on-demand
  generation fallback and is slower than prod (where the blob pool is pre-seeded).
- Ground truth for a board is the API payload, not the DOM: `GET /api/players/{pid}/games/{id}`
  → count `cells` where `value === null`.

## Checking generator/difficulty behavior

Don't hand-roll clue counting — the repo has a quality report:

```bash
dotnet run -c Release --project src/backend/Sudoku.Benchmarks -- --validate --samples 25
```

Prints unique-solution rate and min/avg/max clue counts per difficulty.
