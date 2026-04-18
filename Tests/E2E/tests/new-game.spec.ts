import { test, expect } from '../fixtures/app-fixture';
import { setupApiMocks } from '../fixtures/api-mocks';
import { TEST_GAME_ID, makeTestGame } from '../fixtures/game-data';

const newGame = makeTestGame({ id: TEST_GAME_ID });

test.describe('New Game Page', () => {
  test('displays a loading indicator while the puzzle is being created', async ({ page, newGamePage }) => {
    // Delay the create-game API response so the loading state is visible long
    // enough for Playwright to observe it.
    await page.route('**/api/players/**/games/Easy', async (route) => {
      await new Promise<void>(resolve => setTimeout(resolve, 300));
      await route.fulfill({ status: 201, contentType: 'application/json', body: JSON.stringify(newGame) });
    });
    // Mock all other API calls (player exists, status update, get game …)
    await setupApiMocks(page, { newGame, initialGame: newGame });

    await newGamePage.goto('Easy');

    await expect(newGamePage.loadingIndicator()).toBeVisible();
  });

  test('redirects to the game page after the puzzle is created', async ({ page, newGamePage }) => {
    await setupApiMocks(page, { newGame, initialGame: newGame });

    await newGamePage.goto('Easy');

    await newGamePage.waitForRedirect(new RegExp(`/game/${TEST_GAME_ID}`));
    await expect(page).toHaveURL(new RegExp(`/game/${TEST_GAME_ID}`));
  });

  test('navigates back to home when game creation fails', async ({ page, newGamePage }) => {
    // Override the create-game route to return a server error.
    await page.route('**/api/players/**/games/Easy', async (route) => {
      await route.fulfill({ status: 500 });
    });
    // Still mock player-existence so React does not spin forever.
    await page.route('**/api/players/**/exists', async (route) => {
      await route.fulfill({ status: 200, contentType: 'application/json', body: 'true' });
    });

    await newGamePage.goto('Easy');

    await expect(page).toHaveURL('/');
  });
});
