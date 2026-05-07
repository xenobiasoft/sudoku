import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import GameListPage from './GameListPage';
import { makeGame } from '../test/helpers';

const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom');
  return { ...actual, useNavigate: () => mockNavigate };
});

const mockUsePlayerService = vi.fn();
const mockUseGameService = vi.fn();

vi.mock('../hooks/usePlayerService', () => ({
  usePlayerService: () => mockUsePlayerService(),
}));
vi.mock('../hooks/useGameService', () => ({
  useGameService: () => mockUseGameService(),
}));

function renderPage() {
  return render(<MemoryRouter><GameListPage /></MemoryRouter>);
}

beforeEach(() => {
  mockNavigate.mockClear();
  mockUsePlayerService.mockReturnValue({ isNewPlayer: false, isInitialized: true, playerAlias: 'test-user', profileId: 'profile-test-user' });
  mockUseGameService.mockReturnValue({ savedGames: [], isLoaded: true, isLoading: false, error: null, loadGames: vi.fn(), deleteGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn() });
});

describe('GameListPage', () => {
  it('shows empty state message when no games exist', () => {
    renderPage();
    expect(screen.getByText(/No saved games yet/i)).toBeInTheDocument();
  });

  it('shows loading state', () => {
    mockUseGameService.mockReturnValue({ savedGames: [], isLoaded: false, isLoading: true, error: null, loadGames: vi.fn(), deleteGame: vi.fn() });
    renderPage();
    expect(screen.getByText(/Loading/i)).toBeInTheDocument();
  });

  it('shows error state', () => {
    mockUseGameService.mockReturnValue({ savedGames: [], isLoaded: false, isLoading: false, error: 'Network error', loadGames: vi.fn(), deleteGame: vi.fn() });
    renderPage();
    expect(screen.getByText(/Failed to load games/i)).toBeInTheDocument();
  });

  it('displays saved games', () => {
    const games = [makeGame({ id: 'g1' }), makeGame({ id: 'g2' })];
    mockUseGameService.mockReturnValue({ savedGames: games, isLoaded: true, isLoading: false, error: null, loadGames: vi.fn(), deleteGame: vi.fn() });
    renderPage();
    expect(screen.getAllByTitle(/Easy - InProgress/i)).toHaveLength(2);
  });

  it('loads games on mount', async () => {
    const mockLoadGames = vi.fn();
    mockUseGameService.mockReturnValue({ savedGames: [], isLoaded: true, isLoading: false, error: null, loadGames: mockLoadGames, deleteGame: vi.fn() });
    renderPage();
    await waitFor(() => { expect(mockLoadGames).toHaveBeenCalledWith('profile-test-user'); });
  });

  it('navigates to /game/:id when a game is selected', async () => {
    const user = userEvent.setup();
    const game = makeGame({ id: 'abc123' });
    mockUseGameService.mockReturnValue({ savedGames: [game], isLoaded: true, isLoading: false, error: null, loadGames: vi.fn(), deleteGame: vi.fn() });
    renderPage();
    await user.click(screen.getByTitle('Easy - InProgress'));
    expect(mockNavigate).toHaveBeenCalledWith('/game/abc123');
  });

  it('calls deleteGame when delete button is clicked', async () => {
    const user = userEvent.setup();
    const mockDeleteGame = vi.fn().mockResolvedValue(undefined);
    const game = makeGame({ id: 'del123' });
    mockUseGameService.mockReturnValue({ savedGames: [game], isLoaded: true, isLoading: false, error: null, loadGames: vi.fn(), deleteGame: mockDeleteGame });
    renderPage();
    await user.click(screen.getByTitle('Delete game'));
    await waitFor(() => { expect(mockDeleteGame).toHaveBeenCalledWith('profile-test-user', 'del123'); });
  });

  it('redirects to / when new player visits', () => {
    mockUsePlayerService.mockReturnValue({ isNewPlayer: true, isInitialized: false, playerAlias: null, profileId: null });
    renderPage();
    expect(mockNavigate).toHaveBeenCalledWith('/');
  });

  it('navigates to / when Back is clicked', async () => {
    const user = userEvent.setup();
    renderPage();
    await user.click(screen.getByRole('button', { name: /Back/i }));
    expect(mockNavigate).toHaveBeenCalledWith('/');
  });
});
