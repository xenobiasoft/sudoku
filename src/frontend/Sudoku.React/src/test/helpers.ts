import type { CellModel, DifficultyStatsModel, GameModel, GameStatisticsModel, PlayerStatsModel } from '../types';

export function makeCell(overrides: Partial<CellModel> = {}): CellModel {
  return {
    row: 0,
    column: 0,
    value: null,
    isFixed: false,
    isHint: false,
    hasValue: false,
    possibleValues: [],
    ...overrides,
  };
}

export function makeStats(overrides: Partial<GameStatisticsModel> = {}): GameStatisticsModel {
  return {
    totalMoves: 0,
    invalidMoves: 0,
    hintsUsed: 0,
    hintsRemaining: 3,
    playDuration: '00:00:00',
    ...overrides,
  };
}

export function makeCells(size = 9, overrides: Partial<CellModel>[] = []): CellModel[] {
  const cells: CellModel[] = [];
  for (let r = 0; r < size; r++) {
    for (let c = 0; c < size; c++) {
      const idx = r * size + c;
      cells.push(makeCell({ row: r, column: c, ...overrides[idx] }));
    }
  }
  return cells;
}

export const make81Cells = (overrides: Partial<CellModel>[] = []): CellModel[] => makeCells(9, overrides);

export function makeDifficultyStats(overrides: Partial<DifficultyStatsModel> = {}): DifficultyStatsModel {
  return {
    difficulty: 'Easy',
    size: 9,
    gamesPlayed: 0,
    gamesWon: 0,
    averageSolveTime: null,
    bestSolveTime: null,
    ...overrides,
  };
}

export function makePlayerStats(overrides: Partial<PlayerStatsModel> = {}): PlayerStatsModel {
  return {
    gamesPlayed: 0,
    gamesWon: 0,
    winRate: 0,
    byDifficulty: [
      makeDifficultyStats({ difficulty: 'Easy' }),
      makeDifficultyStats({ difficulty: 'Medium' }),
      makeDifficultyStats({ difficulty: 'Hard' }),
      makeDifficultyStats({ difficulty: 'Expert' }),
    ],
    ...overrides,
  };
}

export function makeGame(overrides: Partial<GameModel> = {}): GameModel {
  const size = overrides.size ?? 9;
  return {
    id: 'game-1',
    profileId: 'profile-1',
    displayName: 'player1',
    difficulty: 'Easy',
    status: 'InProgress',
    statistics: makeStats(),
    createdAt: '2024-01-01T00:00:00Z',
    startedAt: '2024-01-01T00:00:00Z',
    completedAt: null,
    pausedAt: null,
    cells: makeCells(size),
    moveHistory: [],
    size,
    ...overrides,
  };
}
