import type { GameModel, PlayerStatsModel, ProfileModel } from '../types';

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';

/**
 * Preserves the HTTP status and `Retry-After` header of a failed request —
 * needed by NewGamePage to distinguish a 503 pool-empty response (16x16) from
 * other create-game failures.
 */
export class ApiError extends Error {
  status: number;
  retryAfterSeconds?: number;

  constructor(message: string, status: number, retryAfterSeconds?: number) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.retryAfterSeconds = retryAfterSeconds;
  }
}

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  console.log(`BASE_URL: ${BASE_URL}, Requesting: ${path}`);
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options?.headers },
    ...options,
  });
  if (!res.ok) throw new Error(`HTTP ${res.status}: ${res.statusText}`);
  const text = await res.text();

  if (!text) return undefined as T;

  try {
    return JSON.parse(text);
  } catch {
    return text as T;
  }
}

async function requestWithStatus<T>(path: string, options?: RequestInit): Promise<{ status: number; data: T | null }> {
  console.log(`BASE_URL: ${BASE_URL}, Requesting: ${path}`);
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options?.headers },
    ...options,
  });
  const text = await res.text();
  let data: T | null = null;
  if (text) {
    try { data = JSON.parse(text); } catch { data = text as unknown as T; }
  }
  return { status: res.status, data };
}

export const apiClient = {
  createProfile: (alias: string): Promise<{ status: number; data: ProfileModel | null }> =>
    requestWithStatus<ProfileModel>('/api/profiles', { method: 'POST', body: JSON.stringify({ alias }) }),

  getProfile: (alias: string): Promise<{ status: number; data: ProfileModel | null }> =>
    requestWithStatus<ProfileModel>(`/api/profiles/${encodeURIComponent(alias)}`),

  updateProfileAlias: (alias: string, newAlias: string): Promise<{ status: number; data: ProfileModel | null }> =>
    requestWithStatus<ProfileModel>(`/api/profiles/${encodeURIComponent(alias)}`, {
      method: 'PATCH',
      body: JSON.stringify({ newAlias }),
    }),

  deleteProfile: (alias: string): Promise<{ status: number; data: null }> =>
    requestWithStatus<null>(`/api/profiles/${encodeURIComponent(alias)}`, { method: 'DELETE' }),

  createGame: async (profileId: string, difficulty: string, size = 9): Promise<GameModel> => {
    const res = await fetch(`${BASE_URL}/api/players/${profileId}/games/${difficulty}?size=${size}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
    });
    if (!res.ok) {
      // Retry-After can be delta-seconds ("30") or an HTTP-date; only the numeric
      // form is meaningful here, so normalize anything else (including NaN) to undefined.
      const retryAfterHeader = res.headers.get('Retry-After');
      const parsedRetryAfter = retryAfterHeader ? parseInt(retryAfterHeader, 10) : NaN;
      const retryAfterSeconds = Number.isNaN(parsedRetryAfter) ? undefined : parsedRetryAfter;
      throw new ApiError(`HTTP ${res.status}: ${res.statusText}`, res.status, retryAfterSeconds);
    }
    const location = res.headers.get('Location');
    const gameId = location?.split('/').pop();
    if (!gameId) throw new Error('No Location header in createGame response');
    return request(`/api/players/${profileId}/games/${gameId}`);
  },

  getGames: (profileId: string): Promise<GameModel[]> =>
    request(`/api/players/${profileId}/games`),

  getPlayerStats: (profileId: string): Promise<PlayerStatsModel> =>
    request(`/api/players/${profileId}/stats`),

  getGame: (profileId: string, gameId: string): Promise<GameModel> =>
    request(`/api/players/${profileId}/games/${gameId}`),

  deleteGame: (profileId: string, gameId: string): Promise<void> =>
    request(`/api/players/${profileId}/games/${gameId}`, { method: 'DELETE' }),

  makeMove: async (
    profileId: string,
    gameId: string,
    row: number,
    column: number,
    value: number | null,
    playDuration: string
  ): Promise<GameModel> => {
    await request(`/api/players/${profileId}/games/${gameId}/actions`, {
      method: 'PUT',
      body: JSON.stringify({ row, column, value, playDuration }),
    });
    return request(`/api/players/${profileId}/games/${gameId}`);
  },

  resetGame: async (profileId: string, gameId: string): Promise<GameModel> => {
    await request(`/api/players/${profileId}/games/${gameId}/actions/reset`, { method: 'POST' });
    return request(`/api/players/${profileId}/games/${gameId}`);
  },

  undoMove: async (profileId: string, gameId: string): Promise<GameModel> => {
    await request(`/api/players/${profileId}/games/${gameId}/actions/undo`, { method: 'POST' });
    return request(`/api/players/${profileId}/games/${gameId}`);
  },

  getHint: async (profileId: string, gameId: string, playDuration: string): Promise<GameModel> => {
    await request(`/api/players/${profileId}/games/${gameId}/actions/hint`, {
      method: 'POST',
      body: JSON.stringify({ playDuration }),
    });
    return request(`/api/players/${profileId}/games/${gameId}`);
  },

  pauseGame: (profileId: string, gameId: string): Promise<void> =>
    request(`/api/players/${profileId}/games/${gameId}/status/pause`, { method: 'POST' }),

  resumeGame: (profileId: string, gameId: string): Promise<void> =>
    request(`/api/players/${profileId}/games/${gameId}/status/resume`, { method: 'POST' }),

  addPossibleValue: async (
    profileId: string,
    gameId: string,
    row: number,
    column: number,
    value: number
  ): Promise<GameModel> => {
    await request(`/api/players/${profileId}/games/${gameId}/possible-values`, {
      method: 'POST',
      body: JSON.stringify({ row, column, value }),
    });
    return request(`/api/players/${profileId}/games/${gameId}`);
  },

  removePossibleValue: async (
    profileId: string,
    gameId: string,
    row: number,
    column: number,
    value: number
  ): Promise<GameModel> => {
    await request(`/api/players/${profileId}/games/${gameId}/possible-values`, {
      method: 'DELETE',
      body: JSON.stringify({ row, column, value }),
    });
    return request(`/api/players/${profileId}/games/${gameId}`);
  },

  clearPossibleValues: async (
    profileId: string,
    gameId: string,
    row: number,
    column: number
  ): Promise<GameModel> => {
    await request(`/api/players/${profileId}/games/${gameId}/possible-values/clear`, {
      method: 'DELETE',
      body: JSON.stringify({ row, column }),
    });
    return request(`/api/players/${profileId}/games/${gameId}`);
  },
};
