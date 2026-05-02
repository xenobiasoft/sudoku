import type { GameModel, ProfileModel } from '../types';

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';

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

  createGame: (alias: string, difficulty: string): Promise<GameModel> =>
    request(`/api/players/${alias}/games/${difficulty}`, { method: 'POST' }),

  getGames: (alias: string): Promise<GameModel[]> =>
    request(`/api/players/${alias}/games`),

  getGame: (alias: string, gameId: string): Promise<GameModel> =>
    request(`/api/players/${alias}/games/${gameId}`),

  deleteGame: (alias: string, gameId: string): Promise<void> =>
    request(`/api/players/${alias}/games/${gameId}`, { method: 'DELETE' }),

  makeMove: async (
    alias: string,
    gameId: string,
    row: number,
    column: number,
    value: number | null,
    playDuration: string
  ): Promise<GameModel> => {
    await request(`/api/players/${alias}/games/${gameId}/actions`, {
      method: 'PUT',
      body: JSON.stringify({ row, column, value, playDuration }),
    });
    return request(`/api/players/${alias}/games/${gameId}`);
  },

  resetGame: async (alias: string, gameId: string): Promise<GameModel> => {
    await request(`/api/players/${alias}/games/${gameId}/actions/reset`, { method: 'POST' });
    return request(`/api/players/${alias}/games/${gameId}`);
  },

  undoMove: async (alias: string, gameId: string): Promise<GameModel> => {
    await request(`/api/players/${alias}/games/${gameId}/actions/undo`, { method: 'POST' });
    return request(`/api/players/${alias}/games/${gameId}`);
  },

  pauseGame: (alias: string, gameId: string): Promise<void> =>
    request(`/api/players/${alias}/games/${gameId}/status/pause`, { method: 'POST' }),

  resumeGame: (alias: string, gameId: string): Promise<void> =>
    request(`/api/players/${alias}/games/${gameId}/status/resume`, { method: 'POST' }),

  addPossibleValue: async (
    alias: string,
    gameId: string,
    row: number,
    column: number,
    value: number
  ): Promise<GameModel> => {
    await request(`/api/players/${alias}/games/${gameId}/possible-values`, {
      method: 'POST',
      body: JSON.stringify({ row, column, value }),
    });
    return request(`/api/players/${alias}/games/${gameId}`);
  },

  removePossibleValue: async (
    alias: string,
    gameId: string,
    row: number,
    column: number,
    value: number
  ): Promise<GameModel> => {
    await request(`/api/players/${alias}/games/${gameId}/possible-values`, {
      method: 'DELETE',
      body: JSON.stringify({ row, column, value }),
    });
    return request(`/api/players/${alias}/games/${gameId}`);
  },

  clearPossibleValues: async (
    alias: string,
    gameId: string,
    row: number,
    column: number
  ): Promise<GameModel> => {
    await request(`/api/players/${alias}/games/${gameId}/possible-values/clear`, {
      method: 'DELETE',
      body: JSON.stringify({ row, column }),
    });
    return request(`/api/players/${alias}/games/${gameId}`);
  },
};
