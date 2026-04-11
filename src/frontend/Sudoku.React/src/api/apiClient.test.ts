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

describe('apiClient.createPlayer', () => {
  it('POSTs to /api/players and returns alias string', async () => {
    vi.stubGlobal('fetch', mockFetch(200, 'player-alias'));
    const result = await apiClient.createPlayer('testAlias');
    expect(result).toBe('player-alias');
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players'),
      expect.objectContaining({ method: 'POST' })
    );
  });
});

describe('apiClient.playerExists', () => {
  it('GETs /api/players/:alias/exists and returns boolean', async () => {
    vi.stubGlobal('fetch', mockFetch(200, true));
    const result = await apiClient.playerExists('testAlias');
    expect(result).toBe(true);
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/testAlias/exists'),
      expect.any(Object)
    );
  });

  it('returns false when player does not exist', async () => {
    vi.stubGlobal('fetch', mockFetch(200, false));
    const result = await apiClient.playerExists('unknown');
    expect(result).toBe(false);
  });
});

describe('apiClient.createGame', () => {
  it('POSTs to /api/players/:alias/games/:difficulty and returns GameModel', async () => {
    const game = makeGame();
    vi.stubGlobal('fetch', mockFetch(200, game));
    const result = await apiClient.createGame('player1', 'Easy');
    expect(result).toEqual(game);
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/Easy'),
      expect.objectContaining({ method: 'POST' })
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

describe('apiClient.updateStatus', () => {
  it('PATCHes /api/players/:alias/games/:gameId/status/:status', async () => {
    vi.stubGlobal('fetch', mockFetch(200, ''));
    await apiClient.updateStatus('player1', 'game-1', 'Paused');
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/players/player1/games/game-1/status/Paused'),
      expect.objectContaining({ method: 'PATCH' })
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
