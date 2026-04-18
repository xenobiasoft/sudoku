import type { Page } from '@playwright/test';
import type { GameModel } from './game-data';
import { makeTestGame, TEST_ALIAS, TEST_GAME_ID } from './game-data';

export interface ApiMockOptions {
  /** Whether GET /api/players/{alias}/exists returns true (default: true). */
  playerExists?: boolean;
  /** Games returned by GET /api/players/{alias}/games (default: []). */
  gameList?: GameModel[];
  /** Game state returned by GET /api/players/{alias}/games/{id} on first load. */
  initialGame?: GameModel;
  /**
   * Game returned in the response body of PUT /actions (makeMove), and also by
   * the subsequent GET /games/{id} that Blazor issues after every action.
   * React reads the response body directly; Blazor issues a follow-up GET.
   */
  gameAfterMove?: GameModel;
  /**
   * Game returned after POST /actions/undo. Blazor re-fetches via GET; React
   * uses the response body.
   */
  gameAfterUndo?: GameModel;
  /**
   * Game returned after POST /actions/reset. Blazor re-fetches via GET; React
   * uses the response body.
   */
  gameAfterReset?: GameModel;
  /** Game returned after POST/DELETE /possible-values. */
  gameAfterPencil?: GameModel;
  /** Game returned by POST /games/{difficulty} (create game). */
  newGame?: GameModel;
}

/**
 * Registers Playwright route interceptors for all API endpoints used by both
 * the Blazor and React Sudoku applications.
 *
 * The handler uses a single glob pattern for all player API routes so that it
 * captures requests regardless of the API base URL (relative on localhost or
 * absolute on a staging domain).
 *
 * Behavioural note – Blazor vs React:
 *   • Blazor: action endpoints return 204 No Content; Blazor then issues a
 *     follow-up GET /games/{id} to refresh state.
 *   • React:  action endpoints return 200 with a full GameModel body; React
 *     uses that body directly and does NOT issue a follow-up GET.
 *
 * This function returns HTTP 200 + a GameModel body for all action endpoints.
 * Blazor ignores the body (it only checks the status code) and will then hit
 * the follow-up GET which is also handled here; React consumes the body.
 */
export async function setupApiMocks(page: Page, options: ApiMockOptions = {}): Promise<void> {
  const defaultGame = makeTestGame();
  const {
    playerExists = true,
    gameList = [],
    initialGame = defaultGame,
    gameAfterMove = defaultGame,
    gameAfterUndo = defaultGame,
    gameAfterReset = defaultGame,
    gameAfterPencil = defaultGame,
    newGame = makeTestGame({ id: TEST_GAME_ID }),
  } = options;

  // Tracks the "current" game state so that Blazor's follow-up GET /games/{id}
  // (issued after every action) returns the updated state.
  let currentGame: GameModel = initialGame;

  await page.route('**/api/players/**', async (route) => {
    const rawUrl = route.request().url();
    const method = route.request().method().toUpperCase();

    // Use pathname only so query-string parameters don't affect matching.
    let path: string;
    try {
      path = new URL(rawUrl).pathname;
    } catch {
      path = rawUrl;
    }

    // ── Player existence check (React only, harmless to mock for Blazor) ──────
    if (path.endsWith('/exists') && method === 'GET') {
      await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(playerExists) });
      return;
    }

    // ── Create player ─────────────────────────────────────────────────────────
    if (/\/api\/players$/.test(path) && method === 'POST') {
      await route.fulfill({ status: 201, contentType: 'application/json', body: JSON.stringify(TEST_ALIAS) });
      return;
    }

    // ── Game status update (both apps call this on start/pause/resume) ────────
    if (path.includes('/status/') && method === 'PATCH') {
      await route.fulfill({ status: 204 });
      return;
    }

    // ── Reset game ────────────────────────────────────────────────────────────
    if (path.includes('/actions/reset') && method === 'POST') {
      currentGame = gameAfterReset;
      await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(currentGame) });
      return;
    }

    // ── Undo last move ────────────────────────────────────────────────────────
    if (path.includes('/actions/undo') && method === 'POST') {
      currentGame = gameAfterUndo;
      await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(currentGame) });
      return;
    }

    // ── Make a move (PUT /actions) ────────────────────────────────────────────
    if (path.includes('/actions') && method === 'PUT') {
      currentGame = gameAfterMove;
      await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(currentGame) });
      return;
    }

    // ── Clear possible values (DELETE /possible-values/clear) ─────────────────
    if (path.includes('/possible-values/clear') && method === 'DELETE') {
      currentGame = gameAfterPencil;
      await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(currentGame) });
      return;
    }

    // ── Remove possible value (DELETE /possible-values) ───────────────────────
    if (path.includes('/possible-values') && method === 'DELETE') {
      currentGame = gameAfterPencil;
      await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(currentGame) });
      return;
    }

    // ── Add possible value (POST /possible-values) ────────────────────────────
    if (path.includes('/possible-values') && method === 'POST') {
      currentGame = gameAfterPencil;
      await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(currentGame) });
      return;
    }

    // ── Delete a specific game ────────────────────────────────────────────────
    if (/\/games\/[^/]+$/.test(path) && method === 'DELETE') {
      await route.fulfill({ status: 204 });
      return;
    }

    // ── Get all games for player (home page) ──────────────────────────────────
    if (/\/games$/.test(path) && method === 'GET') {
      await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(gameList) });
      return;
    }

    // ── Create game: POST /games/{difficulty} (difficulty is all alpha) ───────
    if (/\/games\/[A-Za-z]+$/.test(path) && method === 'POST') {
      await route.fulfill({ status: 201, contentType: 'application/json', body: JSON.stringify(newGame) });
      return;
    }

    // ── Get a specific game by ID ──────────────────────────────────────────────
    if (/\/games\/[^/]+$/.test(path) && method === 'GET') {
      await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(currentGame) });
      return;
    }

    // Unmatched request — let it through so failures are visible
    await route.continue();
  });
}
