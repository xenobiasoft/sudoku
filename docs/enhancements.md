# Sudoku App — Enhancement Backlog

> Both frontend apps (Blazor and React) should be kept in feature parity unless noted otherwise.

---

## Infrastructure & Deployment

- [ ] **Deploy Sudoku.React to Static Web App** — Add the React frontend to the existing Azure deployment pipeline, alongside the Blazor app. A spec for this already exists at `docs/specs/setup-deployment-react-static-web-app.md`.
- [ ] **Add VNet to infrastructure** — Integrate a Virtual Network into the Bicep infrastructure (`infra/main.bicep`) to secure communication between the API, storage, and frontends.

---

## User Profiles & Identity

- [ ] **Profile creation flow** — Replace the auto-generated alias with an explicit "create your profile" onboarding step when a user first visits the app. The user picks their own alias/display name.
- [ ] **Passwordless profile locking** — Secure profiles without traditional passwords. Options to explore: a device-bound passkey (WebAuthn/FIDO2), a one-time magic link sent to email, or a PIN stored in a signed/encrypted browser token. Goal: prevent another user from claiming or hijacking an existing alias.
- [ ] **User profile page** — A dedicated page where users can view and edit their alias/display name, see their profile info, and manage their identity across sessions.

---

## Navigation & UX

- [ ] **New landing/home page** — Replace the current entry point with a proper home screen offering clear navigation to: Manage Profile, View Game Stats, Start a New Game, and Browse Game List.
- [ ] **Game stats page** — Surface per-user statistics: games played, win rate, average solve time by difficulty, best times, streaks, etc.

---

## Progressive Web App (PWA)

- [ ] **Convert Sudoku.React to PWA** — Add a Web App Manifest, service worker, and offline caching strategy so the React app can be installed on mobile/desktop and played without a connection.
- [ ] **Convert Sudoku.Blazor to PWA** — Add PWA support to the Blazor Server app. Note: Blazor Server relies on a live SignalR connection, so offline play may be limited — consider caching the shell and showing a graceful offline message.

---

## Gameplay Enhancements

- [ ] **Timer display** — Show an elapsed timer per game session, visible in both frontends.
- [ ] **Hint system** — Allow users to request a hint (reveal one correct cell) with a configurable limit per game.
- [ ] **Difficulty-based scoring** — Award points based on difficulty, solve time, and hints used.
- [ ] **Pause/resume** — Let players pause a game (hiding the board) and resume later without losing progress.
- [ ] **Keyboard navigation** — Full keyboard support for cell selection and number entry (important for desktop PWA experience).

---

## Quality & Polish

- [ ] **Feature flag / shared feature contract** — Define a shared list of features that both Blazor and React must implement, so parity gaps are caught early (could be a simple checklist in this file or a lightweight config).
- [ ] **End-to-end tests** — Add Playwright or Cypress tests covering the main user flows (new game, solve, profile creation) for both frontends.
- [ ] **Mobile-responsive layout audit** — Review both frontends on small screens now that PWA is a goal; fix any layout issues.
