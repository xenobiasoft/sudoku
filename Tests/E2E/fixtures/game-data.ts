/**
 * Shared mock data factories for Sudoku E2E tests.
 *
 * All cells are derived from this standard valid Sudoku solution so that
 * isSolved() checks in both Blazor and React evaluate correctly.
 */

// A valid, fully-solved 9×9 Sudoku board (row-major order).
const SOLVED_BOARD: number[][] = [
  [5, 3, 4, 6, 7, 8, 9, 1, 2],
  [6, 7, 2, 1, 9, 5, 3, 4, 8],
  [1, 9, 8, 3, 4, 2, 5, 6, 7],
  [8, 5, 9, 7, 6, 1, 4, 2, 3],
  [4, 2, 6, 8, 5, 3, 7, 9, 1],
  [7, 1, 3, 9, 2, 4, 8, 5, 6],
  [9, 6, 1, 5, 3, 7, 2, 8, 4],
  [2, 8, 7, 4, 1, 9, 6, 3, 5],
  [3, 4, 5, 2, 8, 6, 1, 7, 9],
];

export const TEST_ALIAS = 'test-player';
export const TEST_GAME_ID = 'game-e2e-test-id-1';
export const TEST_GAME_ID_2 = 'game-e2e-test-id-2';

/** The row/column of the single empty (user-editable) cell used in most tests. */
export const EMPTY_CELL_ROW = 0;
export const EMPTY_CELL_COL = 0;
/** The correct value for the empty cell — completing it solves the board. */
export const EMPTY_CELL_VALUE = SOLVED_BOARD[0][0]; // 5

export interface CellModel {
  row: number;
  column: number;
  value: number | null;
  isFixed: boolean;
  hasValue: boolean;
  possibleValues: number[];
}

export interface GameStatisticsModel {
  totalMoves: number;
  invalidMoves: number;
  playDuration: string;
}

export interface MoveHistoryModel {
  row: number;
  column: number;
  value: number | null;
  isValid: boolean;
}

export interface GameModel {
  id: string;
  playerAlias: string;
  difficulty: string;
  status: string;
  statistics: GameStatisticsModel;
  createdAt: string;
  startedAt: string | null;
  completedAt: string | null;
  pausedAt: string | null;
  cells: CellModel[];
  moveHistory: MoveHistoryModel[];
}

// ---------------------------------------------------------------------------
// Internal helpers
// ---------------------------------------------------------------------------

function makeCell(row: number, col: number, value: number, isFixed: boolean): CellModel {
  return { row, column: col, value, isFixed, hasValue: true, possibleValues: [] };
}

function makeEmptyCell(row: number, col: number): CellModel {
  return { row, column: col, value: null, isFixed: false, hasValue: false, possibleValues: [] };
}

function makeStats(overrides: Partial<GameStatisticsModel> = {}): GameStatisticsModel {
  return { totalMoves: 0, invalidMoves: 0, playDuration: '00:00:00', ...overrides };
}

/**
 * Builds 81 cells where every position is fixed with the correct value
 * EXCEPT the cell at (emptyRow, emptyCol), which is left empty (user-editable).
 */
function makeNearSolvedCells(emptyRow = 0, emptyCol = 0): CellModel[] {
  const cells: CellModel[] = [];
  for (let r = 0; r < 9; r++) {
    for (let c = 0; c < 9; c++) {
      cells.push(
        r === emptyRow && c === emptyCol
          ? makeEmptyCell(r, c)
          : makeCell(r, c, SOLVED_BOARD[r][c], true),
      );
    }
  }
  return cells;
}

/**
 * Builds a fully solved 81-cell board.
 * Cell (0,0) is user-placed (isFixed=false) to reflect that the user filled it in.
 */
function makeSolvedCells(): CellModel[] {
  const cells: CellModel[] = [];
  for (let r = 0; r < 9; r++) {
    for (let c = 0; c < 9; c++) {
      // (0,0) is the cell the user just placed to complete the puzzle
      cells.push(makeCell(r, c, SOLVED_BOARD[r][c], !(r === 0 && c === 0)));
    }
  }
  return cells;
}

// ---------------------------------------------------------------------------
// Public factory functions
// ---------------------------------------------------------------------------

/**
 * Standard test game: near-solved, only cell (0,0) is empty.
 * Use for board rendering and most interaction tests.
 */
export function makeTestGame(overrides: Partial<GameModel> = {}): GameModel {
  return {
    id: TEST_GAME_ID,
    playerAlias: TEST_ALIAS,
    difficulty: 'Easy',
    status: 'InProgress',
    statistics: makeStats({ totalMoves: 2, invalidMoves: 0, playDuration: '00:00:30' }),
    createdAt: '2024-01-01T00:00:00Z',
    startedAt: '2024-01-01T00:00:01Z',
    completedAt: null,
    pausedAt: null,
    cells: makeNearSolvedCells(EMPTY_CELL_ROW, EMPTY_CELL_COL),
    moveHistory: [],
    ...overrides,
  };
}

/**
 * A game with one user-placed value at cell (0,0) = 5 and one move in history.
 * Use for undo / reset tests where there is already a recorded move.
 */
export function makeTestGameWithMove(): GameModel {
  const cells = makeNearSolvedCells(EMPTY_CELL_ROW, EMPTY_CELL_COL);
  const cell = cells.find(c => c.row === EMPTY_CELL_ROW && c.column === EMPTY_CELL_COL)!;
  cell.value = EMPTY_CELL_VALUE;
  cell.hasValue = true;
  cell.isFixed = false;

  return makeTestGame({
    cells,
    moveHistory: [{ row: EMPTY_CELL_ROW, column: EMPTY_CELL_COL, value: EMPTY_CELL_VALUE, isValid: true }],
    statistics: makeStats({ totalMoves: 1, invalidMoves: 0, playDuration: '00:00:10' }),
  });
}

/**
 * A game with statistics populated for testing the stats panel.
 */
export function makeTestGameWithStats(): GameModel {
  return makeTestGame({
    statistics: makeStats({ totalMoves: 5, invalidMoves: 2, playDuration: '00:02:30' }),
  });
}

/**
 * A fully solved game. Use for victory overlay tests.
 */
export function makeSolvedGame(): GameModel {
  return makeTestGame({
    status: 'Completed',
    completedAt: '2024-01-01T00:01:00Z',
    cells: makeSolvedCells(),
    moveHistory: [{ row: EMPTY_CELL_ROW, column: EMPTY_CELL_COL, value: EMPTY_CELL_VALUE, isValid: true }],
    statistics: makeStats({ totalMoves: 1, invalidMoves: 0, playDuration: '00:01:00' }),
  });
}

/**
 * A lightweight saved game for populating the home-page load-game list.
 */
export function makeSavedGame(id: string, difficulty = 'Easy'): GameModel {
  return {
    id,
    playerAlias: TEST_ALIAS,
    difficulty,
    status: 'InProgress',
    statistics: makeStats(),
    createdAt: '2024-01-01T00:00:00Z',
    startedAt: '2024-01-01T00:00:01Z',
    completedAt: null,
    pausedAt: null,
    cells: makeNearSolvedCells(),
    moveHistory: [],
  };
}
