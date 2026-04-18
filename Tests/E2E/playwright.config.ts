import { defineConfig, devices } from '@playwright/test';

/**
 * E2E tests for Sudoku Blazor and React applications.
 *
 * STAGING ENVIRONMENT NOTICE:
 * These tests are designed to run against deployed staging environments.
 * The following environment variables must be configured before running:
 *   - BLAZOR_BASE_URL: URL of the deployed Blazor application
 *   - REACT_BASE_URL:  URL of the deployed React application
 *
 * ⚠️  BLOCKER: Until the staging environment is provisioned, these tests cannot
 * be executed against real deployments. Set these variables to local dev server
 * URLs for local development runs. See README.md for details.
 */
export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [
    ['html', { outputFolder: 'playwright-report', open: 'never' }],
    ['list'],
  ],
  use: {
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
  },
  projects: [
    {
      name: 'blazor',
      use: {
        ...devices['Desktop Chrome'],
        baseURL: process.env.BLAZOR_BASE_URL ?? 'http://localhost:5000',
      },
    },
    {
      name: 'react',
      use: {
        ...devices['Desktop Chrome'],
        baseURL: process.env.REACT_BASE_URL ?? 'http://localhost:5173',
      },
    },
  ],
});
