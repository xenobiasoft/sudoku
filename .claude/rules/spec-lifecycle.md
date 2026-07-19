# Spec Lifecycle

## Rules
- Feature specs live in `docs/specs/` while being implemented.
- Once a spec's implementation is fully merged to `main` (all phases/PRs for it are done), move the spec file into `docs/specs/completed/` in the same commit/PR that finishes the work — don't leave completed specs in the active `docs/specs/` root.
- Move the file as-is (`git mv`); don't rewrite or summarize it on the way in.
