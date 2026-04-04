🔄 Design → Implementation Workflow
This workflow defines how features, refactors, and architectural changes move from idea → design → implementation → PR using the Design Agent and Implementation Agent personas.

1. 🧭 Initiate the Design Phase
Trigger
A new feature, change request, bug fix, or architectural improvement is identified.
Action
Start a dedicated design session with the Design Agent:
You are the Design Agent. I want to produce a detailed design spec for a new feature. 
Ask me clarifying questions until you have enough to propose a full design.


Design Agent Responsibilities
- Gather requirements through clarifying questions
- Identify domain rules, data flow, UI/UX needs, API boundaries
- Produce a structured, implementation‑ready spec
- Ensure alignment with Clean Architecture, CQRS, DDD, domain events
- Avoid generating code
- Surface risks, alternatives, and open questions
Output
A complete spec following the repo’s Spec Template.

2. 📄 Finalize the Design Specification
Action
Iterate with the Design Agent until the spec is:
- Complete
- Unambiguous
- Architecturally sound
- Testable
- Ready for implementation
Storage
Save the finalized spec to the repo:
/docs/specs/<feature-name>.md


Why this matters
- Creates a single source of truth
- Prevents design drift
- Allows clean handoff to the Implementation Agent
- Enables future maintainers to understand the reasoning

3. 🛠️ Start the Implementation Phase
Trigger
The design spec is finalized and saved.
Action
Start a new, clean Implementation Agent session:
You are the Implementation Agent. Implement the feature described in 
/docs/specs/<feature-name>.md. Follow the spec exactly. Do not redesign anything.


Why a new session?
- Clears design‑phase context
- Prevents hallucinated requirements
- Ensures strict adherence to the spec
- Mirrors real‑world architect → engineer handoff

4. 🧩 Implementation Agent Execution
Responsibilities
The Implementation Agent:
- Writes code across all affected projects
- Creates new files and updates existing ones
- Implements commands, queries, handlers, domain models
- Updates Blazor or React/Vite components
- Adds or updates API endpoints
- Ensures DI, routing, and Aspire components are correct
- Generates tests (unit, integration, UI)
- Ensures code compiles and follows project conventions
- Flags missing details with concise questions
Rules
- Follow the spec exactly
- No redesigning
- No alternative architectures
- No speculative code
- Keep changes scoped to the feature

5. 🧪 Testing & Validation
Implementation Agent Responsibilities
- Generate tests according to the spec
- Ensure domain invariants are enforced
- Validate end‑to‑end behavior
- Confirm that the implementation matches the design
Human Responsibilities
- Run the solution
- Validate UI behavior (Blazor or React)
- Confirm API correctness
- Review logs, metrics, and Aspire dashboards if applicable

6. 📬 Prepare the Pull Request
Implementation Agent Output
The agent provides:
- A summary of all changes
- A suggested PR description
- A list of files created/modified
- Any follow‑up tasks or cleanup items
Human Responsibilities
- Review the diff
- Ensure adherence to the spec
- Request changes if needed
- Approve and merge

7. 🔁 Post‑Merge Follow‑Up (Optional)
Depending on the feature:
- Update documentation
- Add monitoring or dashboards
- Schedule refactors or cleanup
- Add additional tests
- Update Aspire components
