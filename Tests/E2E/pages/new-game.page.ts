import { type Locator, type Page } from '@playwright/test';
import type { AppType } from '../fixtures/app-fixture';

/**
 * Page Object Model for the New Game page (/new/{difficulty}).
 *
 * Both apps display a loading spinner while the puzzle is being generated,
 * then redirect to /game/{id} on success or / on failure.
 */
export class NewGamePage {
  constructor(
    private readonly page: Page,
    private readonly app: AppType,
  ) {}

  // ── Navigation ──────────────────────────────────────────────────────────────

  async goto(difficulty: string): Promise<void> {
    await this.page.goto(`/new/${difficulty}`);
  }

  // ── Locators ────────────────────────────────────────────────────────────────

  /**
   * The loading container shown while the API call is in progress.
   *
   * Blazor:  `.loading-container` CSS class.
   * React:   CSS Module class containing "loadingContainer".
   */
  loadingIndicator(): Locator {
    if (this.app === 'blazor') {
      return this.page.locator('.loading-container');
    }
    return this.page.locator('[class*="loadingContainer"]');
  }

  // ── Interactions ─────────────────────────────────────────────────────────────

  async waitForRedirect(expected: string | RegExp): Promise<void> {
    await this.page.waitForURL(expected);
  }
}
