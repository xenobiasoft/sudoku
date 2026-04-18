import { test, expect } from '../fixtures/app-fixture';
import { setupApiMocks } from '../fixtures/api-mocks';
import { TEST_GAME_ID, TEST_GAME_ID_2, makeSavedGame, makeTestGame } from '../fixtures/game-data';

const savedGame1 = makeSavedGame(TEST_GAME_ID, 'Easy');
const savedGame2 = makeSavedGame(TEST_GAME_ID_2, 'Medium');

test.describe('Home Page', () => {
  test('displays Start New Game and Load Game buttons', async ({ page, homePage }) => {
    await setupApiMocks(page);
    await homePage.goto();

    await expect(homePage.newGameButton()).toBeVisible();
    await expect(homePage.loadGameButton()).toBeVisible();
  });

  test('Load Game button is disabled when there are no saved games', async ({ page, homePage }) => {
    await setupApiMocks(page, { gameList: [] });
    await homePage.goto();

    await expect(homePage.loadGameButton()).toBeDisabled();
  });

  test('clicking Start New Game expands the difficulty sub-menu', async ({ page, homePage }) => {
    await setupApiMocks(page);
    await homePage.goto();

    await homePage.newGameButton().click();

    await expect(homePage.difficultyButton('Easy')).toBeVisible();
    await expect(homePage.difficultyButton('Medium')).toBeVisible();
    await expect(homePage.difficultyButton('Hard')).toBeVisible();
  });

  test('selecting a difficulty navigates to the new-game page', async ({ page, homePage }) => {
    await setupApiMocks(page);
    await homePage.goto();

    await homePage.startNewGame('Easy');

    await expect(page).toHaveURL(/\/new\/Easy/);
  });

  test('Load Game button is enabled when saved games exist', async ({ page, homePage }) => {
    await setupApiMocks(page, { gameList: [savedGame1] });
    await homePage.goto();

    await expect(homePage.loadGameButton()).toBeEnabled();
  });

  test('clicking Load Game displays saved game thumbnails', async ({ page, homePage }) => {
    await setupApiMocks(page, { gameList: [savedGame1, savedGame2] });
    await homePage.goto();

    await homePage.openLoadGameMenu();

    await expect(homePage.gameThumbnails()).toHaveCount(2);
  });

  test('clicking a saved game thumbnail navigates to the game page', async ({ page, homePage }) => {
    await setupApiMocks(page, {
      gameList: [savedGame1],
      initialGame: savedGame1,
    });
    await homePage.goto();

    await homePage.openLoadGameMenu();
    await homePage.gameThumbnails().first().click();

    await expect(page).toHaveURL(new RegExp(`/game/${TEST_GAME_ID}`));
  });

  test('clicking the delete button on a thumbnail removes it from the list', async ({ page, homePage }) => {
    await setupApiMocks(page, { gameList: [savedGame1, savedGame2] });
    await homePage.goto();

    await homePage.openLoadGameMenu();
    await expect(homePage.gameThumbnails()).toHaveCount(2);

    await homePage.deleteGameButtons().first().click();

    await expect(homePage.gameThumbnails()).toHaveCount(1);
  });
});
