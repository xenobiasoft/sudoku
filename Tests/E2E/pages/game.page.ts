import { type Locator, type Page } from '@playwright/test';
import type { AppType } from '../fixtures/app-fixture';

/**
 * Page Object Model for the Game page (/game/{puzzleId}).
 *
 * Where Blazor and React use different element identifiers (ids, class names,
 * button text) the locator methods branch on `this.app`.
 *
 * Key differences:
 *  ┌──────────────────┬───────────────────────────────┬──────────────────────┐
 *  │ Element          │ Blazor                        │ React                │
 *  ├──────────────────┼───────────────────────────────┼──────────────────────┤
 *  │ Number buttons   │ id="btn1"…"btn9" (icon only)  │ text "1"…"9"         │
 *  │ Erase button     │ id="btnErase"                 │ title="Erase"        │
 *  │ Home button      │ id="btnHome" "Main Menu"      │ title="Home"         │
 *  │ Undo button      │ id="btnUndo"                  │ title="Undo"         │
 *  │ Reset button     │ id="btnReset"                 │ title="Reset"        │
 *  │ Pencil button    │ id="btnPencilMode"            │ title="Pencil mode"  │
 *  │ Stats header     │ .stat-header                  │ [class*=statHeader]  │
 *  │ Victory overlay  │ .victory-overlay              │ [class*=victoryOverlay] │
 *  │ Victory close    │ "Back To Start"               │ "Back to Home"       │
 *  └──────────────────┴───────────────────────────────┴──────────────────────┘
 */
export class GamePage {
  constructor(
    private readonly page: Page,
    private readonly app: AppType,
  ) {}

  // ── Navigation ──────────────────────────────────────────────────────────────

  async goto(gameId: string): Promise<void> {
    await this.page.goto(`/game/${gameId}`);
    await this.gameBoard().waitFor({ state: 'visible' });
  }

  // ── Board locators ───────────────────────────────────────────────────────────

  gameBoard(): Locator {
    return this.page.locator('table');
  }

  gameCells(): Locator {
    return this.page.locator('table tbody tr td');
  }

  /** Returns the <td> for the cell at (row, col) in row-major order. */
  cellAt(row: number, col: number): Locator {
    return this.gameCells().nth(row * 9 + col);
  }

  /**
   * Returns the <input> inside the cell at (row, col).
   * Both apps render user-editable cells as <input readonly>.
   */
  cellInputAt(row: number, col: number): Locator {
    return this.cellAt(row, col).locator('input');
  }

  // ── Control locators ─────────────────────────────────────────────────────────

  numberButton(n: number): Locator {
    if (this.app === 'blazor') {
      return this.page.locator(`#btn${n}`);
    }
    return this.page.getByRole('button', { name: n.toString(), exact: true });
  }

  eraseButton(): Locator {
    if (this.app === 'blazor') {
      return this.page.locator('#btnErase');
    }
    return this.page.getByTitle('Erase');
  }

  homeButton(): Locator {
    if (this.app === 'blazor') {
      return this.page.locator('#btnHome');
    }
    return this.page.getByTitle('Home');
  }

  undoButton(): Locator {
    if (this.app === 'blazor') {
      return this.page.locator('#btnUndo');
    }
    return this.page.getByTitle('Undo');
  }

  resetButton(): Locator {
    if (this.app === 'blazor') {
      return this.page.locator('#btnReset');
    }
    return this.page.getByTitle('Reset');
  }

  pencilButton(): Locator {
    if (this.app === 'blazor') {
      return this.page.locator('#btnPencilMode');
    }
    return this.page.getByTitle('Pencil mode');
  }

  // ── Stats locators ───────────────────────────────────────────────────────────

  /**
   * The collapsible stats header.
   *
   * Blazor:  `.stat-header` CSS class.
   * React:   CSS Module class containing "statHeader".
   */
  statsHeader(): Locator {
    if (this.app === 'blazor') {
      return this.page.locator('.stat-header').first();
    }
    return this.page.locator('[class*="statHeader"]').first();
  }

  /** "Total Moves" text visible only when the stats panel is expanded. */
  statsExpanded(): Locator {
    return this.page.getByText(/total moves/i);
  }

  // ── Victory locators ─────────────────────────────────────────────────────────

  /**
   * The victory overlay container.
   *
   * Blazor:  `.victory-overlay` CSS class.
   * React:   CSS Module class containing "victoryOverlay".
   */
  victoryOverlay(): Locator {
    if (this.app === 'blazor') {
      return this.page.locator('.victory-overlay');
    }
    return this.page.locator('[class*="victoryOverlay"]');
  }

  /**
   * The close/return-home button inside the victory overlay.
   *
   * Blazor:  "Back To Start"
   * React:   "Back to Home"
   */
  victoryCloseButton(): Locator {
    if (this.app === 'blazor') {
      return this.page.getByRole('button', { name: /back to start/i });
    }
    return this.page.getByRole('button', { name: /back to home/i });
  }
}
