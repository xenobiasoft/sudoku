# Sudoku React Frontend - AI Coding Instructions

## Architecture Overview

This is a React TypeScript frontend for a Sudoku game that communicates with a separate backend API. The app uses React Router for navigation between three main flows: home (game selection), new game creation, and active gameplay.

### Core Data Flow

- **Player Management**: Auto-creates anonymous players via `apiClient.createPlayer()` and stores alias in localStorage
- **Game State**: All game logic is server-side; frontend only handles presentation and input validation
- **API Integration**: Centralized in `src/api/apiClient.ts` using environment-based URL configuration

## Key Patterns & Conventions

### Component Architecture

- **CSS Modules**: Every component has a `.module.css` file (e.g., `GameBoard.module.css`)
- **TypeScript Interface Props**: All components use explicit TypeScript interfaces for props
- **Functional Components**: Uses modern React with hooks, no class components

```tsx
// Standard component pattern
interface ComponentProps {
  prop: Type;
}

export default function Component({ prop }: ComponentProps) {
  // Component logic
}
```

### State Management

- **No Global State**: Uses React local state + localStorage for persistence
- **API-Driven**: Game state lives on server; components receive via props from pages
- **Player Session**: Player alias stored in localStorage, created automatically on first visit

### Styling Conventions

- **CSS Modules**: Import as `styles` object: `import styles from './Component.module.css'`
- **BEM-like Classes**: Use descriptive class names like `.cell`, `.highlight`, `.invalid`
- **Grid Layout**: Sudoku board uses HTML table with CSS Grid enhancements

## Critical File Relationships

### API Layer (`src/api/apiClient.ts`)

- Handles all backend communication with error handling and response parsing
- Uses `VITE_API_BASE_URL` environment variable for API endpoint
- All API calls return typed responses matching `src/types/index.ts`

### Game Logic (`src/utils/gameUtils.ts`)

- **Cell Utilities**: `getCell()`, `getRowCells()`, `getMiniGridCells()` for board manipulation
- **Validation**: `validateCells()` returns invalid cells for UI feedback
- **Board State**: `isSolved()` checks completion status

### Type Definitions (`src/types/index.ts`)

- **CellModel**: Core data structure with `possibleValues[]` for pencil mode
- **GameModel**: Complete game state including statistics and move history
- **API Contracts**: All interfaces match backend response schemas

## Development Workflow

### Local Development

```bash
npm run dev          # Start dev server on port 5173 (configurable via VITE_PORT)
npm run build        # TypeScript compile + Vite build
npm run lint         # ESLint with TypeScript rules
```

### Environment Configuration

- **API Proxy**: Vite dev server proxies `/api/*` requests to backend (see `vite.config.ts`)
- **Production**: Docker build creates nginx-served static files
- **Environment Variables**: Use `VITE_` prefix for client-side vars

### Component Development Guidelines

- **Props Validation**: Always use TypeScript interfaces, never inline types
- **CSS Modules**: Import styles as default export, use object notation
- **Event Handling**: Pass specific callback props rather than generic handlers
- **Accessibility**: Include proper ARIA labels and keyboard navigation (see GameBoard component)

## Integration Points

### Backend API Expectations

- RESTful endpoints under `/api/` namespace
- Player alias-based routing: `/api/players/{alias}/games/{gameId}`
- Game state mutations via move API with duration tracking
- Error responses use standard HTTP status codes

### Router Structure

```tsx
"/"; // HomePage - game selection/creation
"/new/:difficulty"; // NewGamePage - shows loading/game creation
"/game/:puzzleId"; // GamePage - active gameplay
```

### Local Storage Schema

```js
localStorage.playerAlias; // String: auto-generated player identifier
```

## Testing & Validation Patterns

- **Cell Validation**: Use `validateCells()` to highlight invalid moves
- **Move Feedback**: Invalid cells get `styles.invalid` class for user feedback
- **Game Completion**: Check `isSolved(cells)` before allowing completion
- **Error Boundaries**: Components gracefully handle API failures with try/catch

## Performance Considerations

- **Cell Rendering**: 81 individual CellInput components render efficiently via React keys
- **API Caching**: No client-side caching; relies on server state authority
- **Bundle Size**: Minimal dependencies (React, React Router, no state management libs)
