import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, within } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import StatsPage from './StatsPage';
import { makeDifficultyStats, makePlayerStats } from '../test/helpers';

const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom');
  return { ...actual, useNavigate: () => mockNavigate };
});

const mockUsePlayerService = vi.fn();
const mockUseStatsService = vi.fn();

vi.mock('../hooks/usePlayerService', () => ({
  usePlayerService: () => mockUsePlayerService(),
}));
vi.mock('../hooks/useStatsService', () => ({
  useStatsService: () => mockUseStatsService(),
}));

function renderPage() {
  return render(
    <MemoryRouter initialEntries={['/stats']}>
      <StatsPage />
    </MemoryRouter>
  );
}

const loadStats = vi.fn();

function statsService(overrides: Record<string, unknown> = {}) {
  return { stats: null, isLoaded: true, isLoading: false, error: null, loadStats, ...overrides };
}

function difficultyRow(difficulty: string) {
  return screen.getByRole('row', { name: new RegExp(`^${difficulty}`, 'i') });
}

beforeEach(() => {
  mockNavigate.mockClear();
  loadStats.mockClear();
  mockUsePlayerService.mockReturnValue({ isNewPlayer: false, isInitialized: true, playerAlias: 'test-user', profileId: 'profile-test-user' });
  mockUseStatsService.mockReturnValue(statsService());
});

describe('StatsPage', () => {
  it('loads stats for the current profile on mount', () => {
    renderPage();
    expect(loadStats).toHaveBeenCalledWith('profile-test-user');
  });

  it('redirects a player with no profile away from the page', () => {
    mockUsePlayerService.mockReturnValue({ isNewPlayer: true, isInitialized: false, playerAlias: null, profileId: null });
    renderPage();
    expect(mockNavigate).toHaveBeenCalledWith('/');
  });

  it('shows loading state', () => {
    mockUseStatsService.mockReturnValue(statsService({ isLoaded: false, isLoading: true }));
    renderPage();
    expect(screen.getByText(/Loading/i)).toBeInTheDocument();
  });

  it('shows error state', () => {
    mockUseStatsService.mockReturnValue(statsService({ isLoaded: false, error: 'Network error' }));
    renderPage();
    expect(screen.getByText(/Failed to load stats/i)).toBeInTheDocument();
  });

  it('shows an empty state when the player has no games', () => {
    mockUseStatsService.mockReturnValue(statsService({ stats: makePlayerStats({ gamesPlayed: 0 }) }));
    renderPage();
    expect(screen.getByText(/no games yet/i)).toBeInTheDocument();
  });

  it('shows the three headline KPI tiles', () => {
    mockUseStatsService.mockReturnValue(statsService({
      stats: makePlayerStats({ gamesPlayed: 12, gamesWon: 7, winRate: 7 / 12 }),
    }));

    renderPage();

    expect(screen.getByText('12')).toBeInTheDocument();
    expect(screen.getByText('7')).toBeInTheDocument();
    expect(screen.getByText('58%')).toBeInTheDocument();
  });

  it('shows solve times for a difficulty the player has won at', () => {
    mockUseStatsService.mockReturnValue(statsService({
      stats: makePlayerStats({
        gamesPlayed: 5,
        gamesWon: 4,
        winRate: 0.8,
        byDifficulty: [
          makeDifficultyStats({
            difficulty: 'Easy',
            gamesPlayed: 5,
            gamesWon: 4,
            averageSolveTime: '00:06:12',
            bestSolveTime: '00:04:30',
          }),
        ],
      }),
    }));

    renderPage();

    const row = difficultyRow('Easy');
    expect(within(row).getByText('06:12')).toBeInTheDocument();
    expect(within(row).getByText('04:30')).toBeInTheDocument();
  });

  it('shows a dash for a difficulty the player has no wins at', () => {
    mockUseStatsService.mockReturnValue(statsService({
      stats: makePlayerStats({
        gamesPlayed: 1,
        gamesWon: 0,
        winRate: 0,
        byDifficulty: [
          makeDifficultyStats({ difficulty: 'Expert', gamesPlayed: 1, gamesWon: 0 }),
        ],
      }),
    }));

    renderPage();

    const row = difficultyRow('Expert');
    expect(within(row).getAllByText('—')).toHaveLength(2);
  });

  it('renders a row for every difficulty, including untouched ones', () => {
    mockUseStatsService.mockReturnValue(statsService({
      stats: makePlayerStats({ gamesPlayed: 1, gamesWon: 1, winRate: 1 }),
    }));

    renderPage();

    ['Easy', 'Medium', 'Hard', 'Expert'].forEach(difficulty => {
      expect(difficultyRow(difficulty)).toBeInTheDocument();
    });
  });

  it('does not show the 16x16 section when the player has no 16x16 completions', () => {
    mockUseStatsService.mockReturnValue(statsService({
      stats: makePlayerStats({ gamesPlayed: 1, gamesWon: 1, winRate: 1 }),
    }));

    renderPage();

    expect(screen.queryByText(/Giant 16×16/i)).not.toBeInTheDocument();
  });

  it('shows the 16x16 section when the player has 16x16 completions', () => {
    mockUseStatsService.mockReturnValue(statsService({
      stats: makePlayerStats({
        gamesPlayed: 6,
        gamesWon: 5,
        winRate: 5 / 6,
        byDifficulty: [
          makeDifficultyStats({ difficulty: 'Easy', size: 9, gamesPlayed: 5, gamesWon: 4 }),
          makeDifficultyStats({
            difficulty: 'Expert',
            size: 16,
            gamesPlayed: 1,
            gamesWon: 1,
            averageSolveTime: '00:45:00',
            bestSolveTime: '00:45:00',
          }),
        ],
      }),
    }));

    renderPage();

    expect(screen.getByText(/Giant 16×16/i)).toBeInTheDocument();
    expect(screen.getByText(/Classic 9×9/i)).toBeInTheDocument();
    const row = difficultyRow('Expert');
    expect(within(row).getAllByText('45:00')).toHaveLength(2);
  });
});
