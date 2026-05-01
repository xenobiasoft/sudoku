---
paths:
  - "src/frontend/Sudoku.React/**/*.ts"
  - "src/frontend/Sudoku.React/**/*.tsx"
---

# React Frontend Guidelines

## Stack
- React 19 + Vite + TypeScript (strict mode)
- React Router v7 for routing
- Vitest + @testing-library/react for tests
- CSS Modules for scoped styling
- No Redux/Zustand — custom hooks own all state
- No Axios — fetch-based `apiClient` singleton

## Folder Structure
```
src/
├── api/          # apiClient singleton and service methods
├── components/   # Reusable UI components (co-locate .test.tsx)
├── hooks/        # Custom hooks for state and side-effects
├── pages/        # Route-level page components (co-locate .test.tsx)
├── types/        # Shared TypeScript interfaces (index.ts)
└── utils/        # Pure utility functions (co-locate .test.ts)
```

## Components
- Functional components only; define props as a TypeScript interface
- Destructure props at the function signature
- Style with CSS Modules (`.module.css`), not inline styles

```tsx
interface CellInputProps {
  value: number;
  isFixed: boolean;
  onChange: (value: number) => void;
}

export function CellInput({ value, isFixed, onChange }: CellInputProps) {
  return <input className={styles.cell} readOnly={isFixed} />;
}
```

## State Management — Custom Hooks
All application state lives in hooks (`useGameService`, `usePlayerService`).
- Use `useState` for state, `useCallback` for stable action references
- Use `useRef` for concurrency guards (prevent double-fetches)
- Cache game state in `localStorage` with a parsed fallback to the API

```ts
const loadingRef = useRef(false);

const loadGame = useCallback(async (id: string) => {
  if (loadingRef.current) return;
  loadingRef.current = true;
  try {
    // fetch logic
  } finally {
    loadingRef.current = false;
  }
}, []);
```

## API Calls
Use the centralized `apiClient` — never call `fetch` directly in components or pages.
- `request<T>()` — throws on non-2xx
- `requestWithStatus<T>()` — returns status for caller-controlled error handling
- Base URL comes from `VITE_API_BASE_URL`; Vite proxy routes `/api` to the backend in dev

## Routing
Five routes via React Router v7 (`BrowserRouter` + `Routes`/`Route`):
- `/` → `HomePage`
- `/new/:difficulty` → `NewGamePage`
- `/game/:puzzleId` → `GamePage`
- `/create-profile` → `CreateProfilePage`
- `/profile` → `ProfilePage`

## Testing
- Co-locate test files with source (e.g., `GameBoard.tsx` → `GameBoard.test.tsx`)
- Use Vitest globals + `@testing-library/react` + `jsdom`
- Test helpers live in `src/test/helpers.ts`
- Run: `npm test` (once), `npm run test:watch` (watch), `npm run test:coverage` (coverage)
- Never mock the `apiClient` at the network layer — mock at the module boundary

## TypeScript
- Strict mode is on (`strict`, `noUnusedLocals`, `noUnusedParameters`)
- Define shared types in `src/types/index.ts`
- No `any` — use `unknown` and narrow with type guards where needed

## Linting
- ESLint 9 flat config (no Prettier); run `npm run lint`
- `eslint-plugin-react-hooks` enforces hooks rules — fix violations, don't disable
