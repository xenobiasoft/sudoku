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
}
