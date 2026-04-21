import type { GameModel } from '../types';

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
  
  // Try to parse as JSON first
  try {
    return JSON.parse(text);
  } catch {
    // If parsing fails, return the text as-is (for plain string responses)
    return text as T;
  }
}

export const apiClient = {
  createPlayer: (alias?: string): Promise<string> =>
    request('/api/players', { method: 'POST', body: JSON.stringify({ alias }) }),

  playerExists: (alias: string): Promise<boolean> =>
    request(`/api/players/${alias}/exists`),

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

  updateStatus: (alias: string, gameId: string, status: string): Promise<void> =>
    request(`/api/players/${alias}/games/${gameId}/status/${status}`, { method: 'PATCH' }),

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
