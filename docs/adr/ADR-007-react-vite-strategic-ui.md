# ADR-007 — React/Vite as the Strategic UI Target

| Field        | Value               |
| ------------ | ------------------- |
| **Date**     | 2026-04-15          |
| **Status**   | Completed           |
| **Deciders** | Project maintainers |

---

## Context

The Sudoku project previously maintained two frontend implementations:

| Project         | Technology                | Deployment                      | Role                           |
| --------------- | ------------------------- | ------------------------------- | ------------------------------ |
| `Sudoku.Blazor` | Blazor Server (.NET)      | Direct Aspire service reference | Retired — archived 2026-05-10  |
| `Sudoku.React`  | React + Vite (TypeScript) | Azure Static Web App            | **Canonical UI**               |

The transition to a single frontend is now complete. `Sudoku.Blazor` has been archived to `archive/Sudoku.Blazor` and removed from the solution, Aspire, CI/CD, and infrastructure.

---

## Decision

**React/Vite (`Sudoku.React`) is the strategic and long-term UI target for the Sudoku application.**

`Sudoku.Blazor` is retained during the transition period but receives **maintenance investment only**. No new features are to be built in Blazor. All new UI development occurs in `Sudoku.React`.

### Rationale

| Factor                      | React/Vite                                                                                    | Blazor Server                                                |
| --------------------------- | --------------------------------------------------------------------------------------------- | ------------------------------------------------------------ |
| Deployment model            | Containerized via `PublishAsDockerFile()` — portable across Azure Container Apps, AKS, Docker | Requires .NET runtime host; tightly coupled to Aspire        |
| UI ecosystem                | Rich component ecosystem (shadcn/ui, Radix, Tailwind, etc.)                                   | Microsoft component ecosystem only                           |
| Separation from backend     | Pure frontend; decoupled from .NET runtime lifecycle                                          | Co-hosted in the .NET process; harder to scale independently |
| Long-term investment signal | Active in this project; aligned with modern web standards                                     | Transitional only                                            |
| API consumption pattern     | Fetches from `Sudoku.Api` via `VITE_API_BASE_URL` injected at runtime                         | Fetches from `Sudoku.Api` via Aspire service reference       |

### Outcome

`Sudoku.React` is the sole frontend. `Sudoku.Blazor` has been archived and all exit criteria were satisfied prior to removal.

---

## Consequences

### Positive

- **Focused investment**: All new feature development targets a single frontend. Effort is not split across two UI codebases.
- **Portability**: The React/Vite frontend is containerized and deployable to any container runtime — not exclusively to Azure Aspire-hosted .NET environments.
- **Ecosystem breadth**: React's component and tooling ecosystem (Vite, TypeScript, testing with Vitest/Testing Library) is broader and more independently scalable than Blazor Server.
- **Clear contributor guidance**: New contributors know immediately where to build new UI features.

### Tradeoffs

- **Blazor investment amortization**: Investment in `Sudoku.Blazor` (components, state management, service integration) is not directly transferable to React. This is a sunk cost that must be accepted.
- **Transition period risk**: During the dual-frontend period, API contract changes must remain backward compatible for both consumers. This creates a short-term constraint on API evolution.
- **Blazor Server advantages foregone**: Blazor Server's real-time SignalR connection and server-side rendering without a separate API layer are not replicated in the React model. This is acceptable given the API-first architecture already in place.

### Rules Enforced by This Decision

1. **All UI features are implemented in `Sudoku.React` only.**
2. **API contracts are owned by the React frontend** — no legacy Blazor compatibility considerations remain.

---

## Related ADRs

- [ADR-001 — Adoption of Clean Architecture](ADR-001-clean-architecture.md)
- [ADR-008 — Azure Aspire for Service Orchestration](ADR-008-aspire.md)
