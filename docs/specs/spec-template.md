# Feature / Change Specification Template

Purpose: Provide a complete, unambiguous, implementation‑ready design specification for any feature, refactor, or architectural change.
Audience: Design Agent, Implementation Agent, reviewers, and future maintainers.


1. 🧭 Overview
Feature Name
(Short, descriptive title)
Problem Statement
(What problem are we solving? Why does it matter? Who is affected?)
Goals
- (Primary goal #1)
- (Primary goal #2)
Non‑Goals
- (Explicitly list what is NOT included to prevent scope creep)

2. 📌 Functional Requirements
List each requirement as a clear, testable statement.
|  |  | 
|  |  | 
|  |  | 
|  |  | 



3. 🛡️ Non‑Functional Requirements
Include constraints relevant to your architecture and environment.
- Performance
- Security
- Reliability
- Observability (logging, metrics, tracing)
- Accessibility
- Localization
- Deployment considerations
- Scalability (Azure/Aspire implications)

4. 🏛️ Architecture Overview
High‑Level Description
(How the feature fits into the existing architecture. Include domain boundaries, aggregates, cross‑project interactions.)
Affected Projects
- Domain
- Application
- Infrastructure
- API
- Blazor or React/Vite
- Aspire components (if applicable)
Sequence Diagram (Mermaid recommended)
sequenceDiagram
    participant UI
    participant API
    participant AppLayer
    participant Domain
    participant Infra

    UI->>API: Request
    API->>AppLayer: Command/Query
    AppLayer->>Domain: Execute logic
    Domain->>Infra: Persist / Fetch
    Infra-->>AppLayer: Result
    AppLayer-->>API: DTO
    API-->>UI: Response



5. 📦 Data Models & Contracts
Domain Models
(New or updated aggregates, entities, value objects, invariants)
DTOs / API Contracts
(Request/response shapes, validation rules, serialization details)
Persistence Changes
- New tables
- Modified tables
- Indexing
- Migrations
- Event store changes (if applicable)

6. 🔄 CQRS Components
Commands
(Name, purpose, inputs, invariants, side effects, events triggered)
Queries
(Name, filters, sorting, pagination, projections)
Handlers
(Responsibilities, dependencies, domain interactions)

7. 📣 Domain Events (if applicable)
|  |  |  |  | 
| XCreatedEvent |  |  |  | 



8. 🖥️ UI/UX Flow
Frontend Target
- Blazor
- React/Vite
(Specify which applies)
Screens / Components
- New components
- Updated components
- Routing changes
User Flow Diagram
(Optional but helpful)
State Management
- Local state
- Server interactions
- Caching
- Error handling

9. 🌐 API Endpoints (if applicable)
|  |  |  |  |  |  | 
|  | /api/... |  |  |  |  | 
|  | /api/... |  |  |  |  | 



10. 🧪 Testing Strategy
Unit Tests
(Domain, handlers, validators)
Integration Tests
(API, persistence, event handling)
UI Tests
(Blazor or React/Vite)
Test Data / Fixtures
(Any special setup needed)

11. ⚠️ Risks & Considerations
- Architectural risks
- Performance concerns
- Migration complexity
- Breaking changes
- Security implications
- Cross‑project impacts

12. 🔧 Implementation Plan (Optional)
A step‑by‑step outline for the Implementation Agent.
- Create/update domain model
- Add command + handler
- Add query + handler
- Add API endpoint
- Update Blazor/React UI
- Add tests
- Update Aspire components
- Validate end‑to‑end

13. ❓ Open Questions
(Anything the Design Agent needs clarified before finalizing the spec.)

If you want, I can also regenerate:
- A filled‑in example spec
- A PR Review Checklist
- A Design Review Checklist
- A Repo Governance section
- A combined Agents section for .github/copilot-instructions.md
Just tell me what direction you want to go next.
