import { test as base, expect } from '@playwright/test';
import { HomePage } from '../pages/home.page';
import { NewGamePage } from '../pages/new-game.page';
import { GamePage } from '../pages/game.page';
import { TEST_ALIAS } from './game-data';

export type AppType = 'blazor' | 'react';

export interface AppFixtures {
  homePage: HomePage;
  newGamePage: NewGamePage;
  gamePage: GamePage;
}

/**
 * Extended Playwright test fixture that:
 * 1. Seeds localStorage with a stable test player alias before each page load,
 *    so neither app attempts to create a new player via the API.
 * 2. Provides app-aware Page Object Models derived from the Playwright project
 *    name ('blazor' | 'react').
 *
 * Usage in test files:
 *   import { test, expect } from '../fixtures/app-fixture';
 */
export const test = base.extend<AppFixtures>({
  // Override the base `page` fixture to inject the player alias into
  // localStorage before each navigation. Both apps use the 'sudoku-alias' key.
  page: async ({ page }, use) => {
    await page.addInitScript((alias: string) => {
      localStorage.setItem('sudoku-alias', alias);
    }, TEST_ALIAS);
    await use(page);
  },

  homePage: async ({ page }, use, testInfo) => {
    const app = testInfo.project.name.toLowerCase() as AppType;
    await use(new HomePage(page, app));
  },

  newGamePage: async ({ page }, use, testInfo) => {
    const app = testInfo.project.name.toLowerCase() as AppType;
    await use(new NewGamePage(page, app));
  },

  gamePage: async ({ page }, use, testInfo) => {
    const app = testInfo.project.name.toLowerCase() as AppType;
    await use(new GamePage(page, app));
  },
});

export { expect };
