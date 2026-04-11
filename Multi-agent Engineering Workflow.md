# Multi‑Agent Workflow Orchestration

This document defines how architectural design, specification review, implementation, testing, documentation, and operational validation flow through the repository using a hybrid activation model. The workflow combines path‑based activation for predictability with label‑based refinement for flexibility.

---

## Workflow Overview

The workflow moves through three phases:

1. **Conversational design** — You collaborate directly with the Architect agent to shape the design. No automation is triggered.
2. **Spec PR review** — When you click "Create PR," the spec enters a structured review process governed by architecture‑related agents.
3. **Implementation PR review** — After the spec merges, implementation PRs activate the correct agents based on paths and labels.

This creates a predictable pipeline from idea → spec → implementation → review → merge.

---

## Repository Structure

The workflow relies on a clear folder layout:

```
/src
  /backend
  /frontend
/infra
/docs
  /specs
  /adr
.github
  /workflows
  /agents
```

Each directory maps to specific agent responsibilities.

---

## Path‑Based Activation

Path rules determine which agents activate automatically when a PR touches certain parts of the repository.

### Backend code — `/src/backend/**`

Activates:
- Reviewer
- Testing Engineer
- Project Governance

### Frontend code — `/src/frontend/**`

Activates:
- UI/UX Engineer
- Reviewer
- Testing Engineer
- Project Governance

### Infrastructure — `/infra/**`

Activates:
- DevOps Engineer
- Project Governance

### GitHub workflows — `/.github/workflows/**`

Activates:
- DevOps Engineer
- Project Governance

### Specifications — `/docs/specs/**`

Activates:
- Architect
- Documentation Engineer
- Project Governance
- Reviewer

### ADRs — `/docs/adr/**`

Activates:
- Architect
- Documentation Engineer
- Project Governance

### Other documentation — `/docs/**`

Activates:
- Documentation Engineer
- Project Governance

---

## Label‑Based Refinement

Labels adjust or override the default path‑based behavior.

| Label | Effect |
|---|---|
| `ui-change` | Force UI/UX Engineer |
| `infra` | Force DevOps Engineer |
| `docs-only` | Suppress Reviewer, Testing, and UI/UX |
| `spec-update` | Route back to Architect + Documentation |
| `breaking-change` | Require Architect + Reviewer + Governance |
| `hotfix` | Allow bypassing non‑critical agents (optional) |

Labels provide nuance without weakening the core path rules.

---

## Approval Gates

Each PR type has required approvals before merge.

### Spec PRs
- Architect
- Documentation Engineer
- Project Governance
- Reviewer

### Backend implementation PRs
- Reviewer
- Testing Engineer
- Project Governance

### Frontend implementation PRs
- UI/UX Engineer
- Reviewer
- Testing Engineer
- Project Governance

### Infrastructure PRs
- DevOps Engineer
- Project Governance

### Workflow PRs
- DevOps Engineer
- Project Governance

### Documentation PRs
- Documentation Engineer
- Project Governance

Approval gates ensure each dimension of quality is validated.

---

## Orchestration Matrix

| Path | Architect | Doc Engineer | Reviewer | Testing Eng | UI/UX Eng | DevOps Eng | Governance |
|---|---|---|---|---|---|---|---|
| `/docs/specs/**` | ✅ | ✅ | ✅ | | | | ✅ |
| `/docs/adr/**` | ✅ | ✅ | | | | | ✅ |
| `/docs/**` | | ✅ | | | | | ✅ |
| `/src/backend/**` | | | ✅ | ✅ | | | ✅ |
| `/src/frontend/**` | | | ✅ | ✅ | ✅ | | ✅ |
| `/infra/**` | | | | | | ✅ | ✅ |
| `.github/workflows/**` | | | | | | ✅ | ✅ |

---

## End‑to‑End Flow

1. You and the Architect iterate conversationally until the design is ready.
2. You click "Create PR," triggering the spec review workflow.
3. The spec PR merges after all required agents approve.
4. Implementation PRs activate the correct agents based on paths and labels.
5. Approval gates enforce quality across architecture, implementation, testing, UX, documentation, and operations.
6. The PR merges once all required agents approve.

This creates a disciplined, multi‑agent engineering workflow with clear boundaries and predictable behavior.