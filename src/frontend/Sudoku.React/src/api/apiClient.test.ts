import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { apiClient } from './apiClient';
import type { GameModel } from '../types';
import { makeGame } from '../test/helpers';

function mockFetch(status: number, body: unknown) {
  const text = typeof body === 'string' ? body : JSON.stringify(body);
  return vi.fn().mockResolvedValue({
    ok: status >= 200 && status < 300,
    status,
    statusText: status === 200 ? 'OK' : 'Error',
    text: () => Promise.resolve(text),
  });
}

beforeEach(() => {
  vi.stubGlobal('fetch', mockFetch(200, ''));
});

afterEach(() => {
  vi.restoreAllMocks();
});

describe('apiClient.createGame', () => {
  it('POSTs to create, follows Location header, and GETs the new game', async () => {
    const game = makeGame({ id: 'new-game-id' });
    const createRes = {
      ok: true, status: 201, statusText: 'Created',
      text: () => Promise.resolve(''),
      headers: { get: (name: string) => name === 'Location' ? '/api/players/player1/games/new-game-id' : null },
    };
    const getRes = {
      ok: true, status: 200, statusText: 'OK',
      text: () => Promise.resolve(JSON.stringify(game)),
      headers: { get: () => null },
    };
    let call = 0;
    vi.stubGlobal('fetch', vi.fn().mockImplementation(() => Promise.resolve(call++ === 0 ? createRes : getRes)));

    const result = await apiClient.createGame('player1', 'Easy');

    expect(result).toEqual(game);
    expect(fetch).toHaveBeenCalledTimes(2);
    expect(fetch).toHaveBeenNthCalledWith(1,
      expect.stringContaining('/api/players/player1/games/Easy'),
      expect.objectContaining({ method: 'POST' })
    );
    expect(fetch).toHaveBeenNthCalledWith(2,
      expect.stringContaining('/api/players/player1/games/new-game-id'),
      expect.not.objectContaining({ method: 'POST' })
    );
  });
});

describe('apiClient.getGames', () => {
  it('GETs /api/players/:alias/games and returns GameModel[]', async () => {
    const games: GameModel[] = [makeGame({ id: 'g1' }), makeGame({ id: 'g2' })];
    vi.stubGlobal('fetch', mockFetch(200, games));
    const result = await apiClient.getGames('player1');
    expect(result).toHaveLength(2);
    expect(result[0].id).toBe('g1');
  });
});

describe('apiClient.getGame', () => {
  it('GETs /api/players/:alias/games/:gameId and returns GameModel', async () => {
    const game = makeGame({ id: 'game-123' });
    vi.stubGlobal('fetch', mockFetch(200, game));
    const result = await apiClient.getGame('player1', 'game-123');
    expect(result.id).toBe('game-123');
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/game-123'),
      expect.any(Object)
    );
  });
});

describe('apiClient.deleteGame', () => {
  it('DELETEs /api/players/:alias/games/:gameId', async () => {
    vi.stubGlobal('fetch', mockFetch(200, ''));
    await apiClient.deleteGame('player1', 'game-123');
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/game-123'),
      expect.objectContaining({ method: 'DELETE' })
    );
  });
});

describe('apiClient.makeMove', () => {
  it('PUTs to /api/players/:alias/games/:gameId/actions', async () => {
    const updated = makeGame();
    vi.stubGlobal('fetch', mockFetch(200, updated));
    const result = await apiClient.makeMove('player1', 'game-1', 0, 0, 5, '00:01:00');
    expect(result).toEqual(updated);
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/game-1/actions'),
      expect.objectContaining({ method: 'PUT' })
    );
  });
});

describe('apiClient.resetGame', () => {
  it('POSTs to /api/players/:alias/games/:gameId/actions/reset', async () => {
    const updated = makeGame();
    vi.stubGlobal('fetch', mockFetch(200, updated));
    await apiClient.resetGame('player1', 'game-1');
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/game-1/actions/reset'),
      expect.objectContaining({ method: 'POST' })
    );
  });
});

describe('apiClient.undoMove', () => {
  it('POSTs to /api/players/:alias/games/:gameId/actions/undo', async () => {
    const updated = makeGame();
    vi.stubGlobal('fetch', mockFetch(200, updated));
    await apiClient.undoMove('player1', 'game-1');
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/game-1/actions/undo'),
      expect.objectContaining({ method: 'POST' })
    );
  });
});

describe('apiClient.pauseGame', () => {
  it('POSTs to /api/players/:alias/games/:gameId/status/pause', async () => {
    vi.stubGlobal('fetch', mockFetch(200, ''));
    await apiClient.pauseGame('player1', 'game-1');
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/game-1/status/pause'),
      expect.objectContaining({ method: 'POST' })
    );
  });
});

describe('apiClient.resumeGame', () => {
  it('POSTs to /api/players/:alias/games/:gameId/status/resume', async () => {
    vi.stubGlobal('fetch', mockFetch(200, ''));
    await apiClient.resumeGame('player1', 'game-1');
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/game-1/status/resume'),
      expect.objectContaining({ method: 'POST' })
    );
  });
});

describe('apiClient.addPossibleValue', () => {
  it('POSTs to /api/players/:alias/games/:gameId/possible-values', async () => {
    const updated = makeGame();
    vi.stubGlobal('fetch', mockFetch(200, updated));
    await apiClient.addPossibleValue('player1', 'game-1', 0, 0, 3);
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/game-1/possible-values'),
      expect.objectContaining({ method: 'POST' })
    );
  });
});

describe('apiClient.removePossibleValue', () => {
  it('DELETEs /api/players/:alias/games/:gameId/possible-values', async () => {
    const updated = makeGame();
    vi.stubGlobal('fetch', mockFetch(200, updated));
    await apiClient.removePossibleValue('player1', 'game-1', 0, 0, 3);
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/game-1/possible-values'),
      expect.objectContaining({ method: 'DELETE' })
    );
  });
});

describe('apiClient.clearPossibleValues', () => {
  it('DELETEs /api/players/:alias/games/:gameId/possible-values/clear', async () => {
    const updated = makeGame();
    vi.stubGlobal('fetch', mockFetch(200, updated));
    await apiClient.clearPossibleValues('player1', 'game-1', 0, 0);
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/game-1/possible-values/clear'),
      expect.objectContaining({ method: 'DELETE' })
    );
  });
});

describe('request error handling', () => {
  it('throws when response is not ok', async () => {
    vi.stubGlobal('fetch', mockFetch(404, 'Not Found'));
    await expect(apiClient.getGame('player1', 'bad-id')).rejects.toThrow('HTTP 404');
  });
});
