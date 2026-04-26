import { test, expect } from '../fixtures/app-fixture';
import { setupApiMocks } from '../fixtures/api-mocks';
import {
  TEST_GAME_ID,
  EMPTY_CELL_ROW,
  EMPTY_CELL_COL,
  EMPTY_CELL_VALUE,
  makeTestGame,
  makeTestGameWithMove,
  makeTestGameWithStats,
  makeSolvedGame,
  makeTestGameForInteraction,
  makeTestGameAfterInteractionMove,
} from '../fixtures/game-data';

// ────────────────────────────────────────────────────────────────────────────
// Board rendering
// ────────────────────────────────────────────────────────────────────────────

test.describe('Game Page – Board', () => {
  test.beforeEach(async ({ page }) => {
    await setupApiMocks(page, { initialGame: makeTestGame() });
  });

  test('renders a 9×9 board with 81 cells', async ({ gamePage }) => {
    await gamePage.goto(TEST_GAME_ID);

    await expect(gamePage.gameCells()).toHaveCount(81);
  });

  test('renders number buttons 1–9', async ({ gamePage }) => {
    await gamePage.goto(TEST_GAME_ID);

    for (let n = 1; n <= 9; n++) {
      await expect(gamePage.numberButton(n)).toBeVisible();
    }
  });

  test('renders action buttons (home, undo, reset, pencil)', async ({ gamePage }) => {
    await gamePage.goto(TEST_GAME_ID);

    await expect(gamePage.homeButton()).toBeVisible();
    await expect(gamePage.undoButton()).toBeVisible();
    await expect(gamePage.resetButton()).toBeVisible();
    await expect(gamePage.pencilButton()).toBeVisible();
  });

  test('renders the game statistics panel', async ({ page, gamePage }) => {
    await setupApiMocks(page, { initialGame: makeTestGameWithStats() });
    await gamePage.goto(TEST_GAME_ID);

    await expect(gamePage.statsHeader()).toBeVisible();
  });
});

// ────────────────────────────────────────────────────────────────────────────
// Cell interaction
// ────────────────────────────────────────────────────────────────────────────

test.describe('Game Page – Cell Interaction', () => {
  test('clicking a number button places a value in the selected cell', async ({ page, gamePage }) => {
    await setupApiMocks(page, {
      initialGame: makeTestGameForInteraction(),
      gameAfterMove: makeTestGameAfterInteractionMove(),
    });
    await gamePage.goto(TEST_GAME_ID);

    await gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL).click();
    await gamePage.numberButton(EMPTY_CELL_VALUE).click();

    await expect(gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL))
      .toHaveValue(EMPTY_CELL_VALUE.toString());
  });

  test('pressing a number key on the keyboard places a value in the selected cell', async ({ page, gamePage }) => {
    await setupApiMocks(page, {
      initialGame: makeTestGameForInteraction(),
      gameAfterMove: makeTestGameAfterInteractionMove(),
    });
    await gamePage.goto(TEST_GAME_ID);

    await gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL).click();
    await page.keyboard.press(EMPTY_CELL_VALUE.toString());

    await expect(gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL))
      .toHaveValue(EMPTY_CELL_VALUE.toString());
  });

  test('pressing arrow keys moves the cell selection without error', async ({ page, gamePage }) => {
    await setupApiMocks(page, { initialGame: makeTestGame() });
    await gamePage.goto(TEST_GAME_ID);

    // Select the empty cell at (0,0) and arrow right — should not throw.
    await gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL).click();
    await page.keyboard.press('ArrowRight');
    await page.keyboard.press('ArrowDown');

    // The board must still be fully rendered after navigation.
    await expect(gamePage.gameBoard()).toBeVisible();
    await expect(gamePage.gameCells()).toHaveCount(81);
  });
});

// ────────────────────────────────────────────────────────────────────────────
// Game actions
// ────────────────────────────────────────────────────────────────────────────

test.describe('Game Page – Game Actions', () => {
  test('clicking Undo reverts the last move', async ({ page, gamePage }) => {
    await setupApiMocks(page, {
      initialGame: makeTestGameWithMove(),    // cell (0,0) = 5
      gameAfterUndo: makeTestGame(),          // cell (0,0) empty
    });
    await gamePage.goto(TEST_GAME_ID);

    // Confirm the move is visible initially.
    await expect(gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL))
      .toHaveValue(EMPTY_CELL_VALUE.toString());

    await gamePage.undoButton().click();

    // After undo the cell should be empty.
    await expect(gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL)).toHaveValue('');
  });

  test('clicking Reset clears all user-placed values', async ({ page, gamePage }) => {
    await setupApiMocks(page, {
      initialGame: makeTestGameWithMove(),    // cell (0,0) = 5
      gameAfterReset: makeTestGame(),         // cell (0,0) empty
    });
    await gamePage.goto(TEST_GAME_ID);

    await expect(gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL))
      .toHaveValue(EMPTY_CELL_VALUE.toString());

    await gamePage.resetButton().click();

    await expect(gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL)).toHaveValue('');
  });

  test('clicking the Home button navigates back to the home page', async ({ page, gamePage }) => {
    await setupApiMocks(page, { initialGame: makeTestGame() });
    await gamePage.goto(TEST_GAME_ID);

    await gamePage.homeButton().click();

    await expect(page).toHaveURL('/');
  });
});

// ────────────────────────────────────────────────────────────────────────────
// Pencil mode
// ────────────────────────────────────────────────────────────────────────────

test.describe('Game Page – Pencil Mode', () => {
  test('clicking the Pencil button toggles pencil mode', async ({ page, gamePage }) => {
    await setupApiMocks(page, { initialGame: makeTestGame() });
    await gamePage.goto(TEST_GAME_ID);

    await gamePage.pencilButton().click();

    // After toggling, the button and board must still be rendered.
    await expect(gamePage.pencilButton()).toBeVisible();
    await expect(gamePage.gameBoard()).toBeVisible();
  });

  test('in pencil mode, clicking a number adds it as a possible value in the cell', async ({ page, gamePage }) => {
    // Build the expected state after adding a possible value to cell (0,0).
    const baseGame = makeTestGame();
    const gameWithPencil = makeTestGame({
      cells: baseGame.cells.map(c =>
        c.row === EMPTY_CELL_ROW && c.column === EMPTY_CELL_COL
          ? { ...c, possibleValues: [EMPTY_CELL_VALUE] }
          : c,
      ),
    });

    await setupApiMocks(page, {
      initialGame: baseGame,
      gameAfterPencil: gameWithPencil,
    });
    await gamePage.goto(TEST_GAME_ID);

    await gamePage.pencilButton().click();
    await gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL).click();
    await gamePage.numberButton(EMPTY_CELL_VALUE).click();

    // The cell should now contain the possible value somewhere in its content.
    await expect(gamePage.cellAt(EMPTY_CELL_ROW, EMPTY_CELL_COL))
      .toContainText(EMPTY_CELL_VALUE.toString());
  });
});

// ────────────────────────────────────────────────────────────────────────────
// Statistics panel
// ────────────────────────────────────────────────────────────────────────────

test.describe('Game Page – Statistics', () => {
  test('clicking the stats header expands the detailed statistics', async ({ page, gamePage }) => {
    await setupApiMocks(page, { initialGame: makeTestGameWithStats() });
    await gamePage.goto(TEST_GAME_ID);

    await gamePage.statsHeader().click();

    await expect(gamePage.statsExpanded()).toBeVisible();
  });
});

// ────────────────────────────────────────────────────────────────────────────
// Victory overlay
// ────────────────────────────────────────────────────────────────────────────

test.describe('Game Page – Victory', () => {
  test('shows the victory overlay when the puzzle is solved', async ({ page, gamePage }) => {
    await setupApiMocks(page, {
      initialGame: makeTestGame(),      // one empty cell at (0,0)
      gameAfterMove: makeSolvedGame(),  // all cells filled → isSolved() = true
    });
    await gamePage.goto(TEST_GAME_ID);

    await gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL).click();
    await gamePage.numberButton(EMPTY_CELL_VALUE).click();

    await expect(gamePage.victoryOverlay()).toBeVisible({ timeout: 5000 });
  });

  test('clicking the victory close button navigates to the home page', async ({ page, gamePage }) => {
    await setupApiMocks(page, {
      initialGame: makeTestGame(),
      gameAfterMove: makeSolvedGame(),
    });
    await gamePage.goto(TEST_GAME_ID);

    await gamePage.cellInputAt(EMPTY_CELL_ROW, EMPTY_CELL_COL).click();
    await gamePage.numberButton(EMPTY_CELL_VALUE).click();

    await expect(gamePage.victoryOverlay()).toBeVisible({ timeout: 5000 });
    await gamePage.victoryCloseButton().click();

    await expect(page).toHaveURL('/');
  });
});
