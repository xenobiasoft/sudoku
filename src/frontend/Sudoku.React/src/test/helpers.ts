import type { CellModel, GameModel, GameStatisticsModel } from '../types';

export function makeCell(overrides: Partial<CellModel> = {}): CellModel {
  return {
    row: 0,
    column: 0,
    value: null,
    isFixed: false,
    hasValue: false,
    possibleValues: [],
    ...overrides,
  };
}

export function makeStats(overrides: Partial<GameStatisticsModel> = {}): GameStatisticsModel {
  return {
    totalMoves: 0,
    invalidMoves: 0,
    playDuration: '00:00:00',
    ...overrides,
  };
}

export function make81Cells(overrides: Partial<CellModel>[] = []): CellModel[] {
  const cells: CellModel[] = [];
  for (let r = 0; r < 9; r++) {
    for (let c = 0; c < 9; c++) {
      const idx = r * 9 + c;
      cells.push(makeCell({ row: r, column: c, ...overrides[idx] }));
    }
  }
  return cells;
}

export function makeGame(overrides: Partial<GameModel> = {}): GameModel {
  return {
    id: 'game-1',
    playerAlias: 'player1',
    difficulty: 'Easy',
    status: 'InProgress',
    statistics: makeStats(),
    createdAt: '2024-01-01T00:00:00Z',
    startedAt: '2024-01-01T00:00:00Z',
    completedAt: null,
    pausedAt: null,
    cells: make81Cells(),
    moveHistory: [],
    ...overrides,
  };
}
