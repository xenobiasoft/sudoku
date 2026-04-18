import { type Locator, type Page } from '@playwright/test';
import type { AppType } from '../fixtures/app-fixture';

/**
 * Page Object Model for the Home / Index page.
 *
 * Selectors that differ between Blazor and React are encapsulated here so
 * that test specs remain app-agnostic.
 */
export class HomePage {
  constructor(
    private readonly page: Page,
    private readonly app: AppType,
  ) {}

  // ── Navigation ──────────────────────────────────────────────────────────────

  async goto(): Promise<void> {
    await this.page.goto('/');
    await this.newGameButton().waitFor({ state: 'visible' });
  }

  // ── Locators ────────────────────────────────────────────────────────────────

  /** "Start New Game" button — identical text in both apps. */
  newGameButton(): Locator {
    return this.page.getByRole('button', { name: /start new game/i });
  }

  /** "Load Game" button — identical text in both apps. */
  loadGameButton(): Locator {
    return this.page.getByRole('button', { name: /load game/i });
  }

  /** Difficulty sub-menu buttons (Easy / Medium / Hard). */
  difficultyButton(difficulty: 'Easy' | 'Medium' | 'Hard'): Locator {
    return this.page.getByRole('button', { name: difficulty, exact: true });
  }

  /**
   * Clickable thumbnail elements inside the Load Game sub-menu.
   *
   * Blazor:  `.sudoku-thumbnail` class.
   * React:   thumbnail div carries a `title` like "Easy - InProgress".
   */
  gameThumbnails(): Locator {
    if (this.app === 'blazor') {
      return this.page.locator('.sudoku-thumbnail');
    }
    return this.page.locator('[title*=" - "]');
  }

  /**
   * Delete buttons on saved-game thumbnails.
   *
   * Blazor:  buttons inside `.saved-game-card`.
   * React:   buttons carry `title="Delete game"`.
   */
  deleteGameButtons(): Locator {
    if (this.app === 'blazor') {
      return this.page.locator('.saved-game-card button');
    }
    return this.page.getByTitle('Delete game');
  }

  // ── Interactions ─────────────────────────────────────────────────────────────

  async openNewGameMenu(): Promise<void> {
    await this.newGameButton().click();
    await this.difficultyButton('Easy').waitFor({ state: 'visible' });
  }

  async openLoadGameMenu(): Promise<void> {
    await this.loadGameButton().click();
  }

  async startNewGame(difficulty: 'Easy' | 'Medium' | 'Hard'): Promise<void> {
    await this.openNewGameMenu();
    await this.difficultyButton(difficulty).click();
  }
}
