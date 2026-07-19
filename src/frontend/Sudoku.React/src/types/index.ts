export interface CellModel {
  row: number;
  column: number;
  value: number | null;
  isFixed: boolean;
  isHint: boolean;
  hasValue: boolean;
  possibleValues: number[];
}

export interface GameStatisticsModel {
  totalMoves: number;
  invalidMoves: number;
  hintsUsed: number;
  hintsRemaining: number;
  playDuration: string;
}

export interface MoveHistoryModel {
  row: number;
  column: number;
  value: number | null;
  isValid: boolean;
}

export interface ProfileModel {
  profileId: string;
  alias: string;
  createdAt: string;
  updatedAt: string;
}

export interface ProfileInfo {
  profileId: string;
  alias: string;
}

export interface GameModel {
  id: string;
  profileId: string;
  displayName: string;
  difficulty: string;
  status: string;
  statistics: GameStatisticsModel;
  createdAt: string;
  startedAt: string | null;
  completedAt: string | null;
  pausedAt: string | null;
  cells: CellModel[];
  moveHistory: MoveHistoryModel[];
  size: number;
}

export interface DifficultyStatsModel {
  difficulty: string;
  size: number;
  gamesPlayed: number;
  gamesWon: number;
  /** "HH:MM:SS", or null when the player has no wins at this difficulty. */
  averageSolveTime: string | null;
  bestSolveTime: string | null;
}

export interface PlayerStatsModel {
  gamesPlayed: number;
  gamesWon: number;
  /** 0–1. Zero when the player has no games. */
  winRate: number;
  byDifficulty: DifficultyStatsModel[];
}
