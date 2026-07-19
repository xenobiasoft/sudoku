import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import NewGamePage from './NewGamePage';
import { makeGame } from '../test/helpers';
import { ApiError } from '../api/apiClient';

const mockNavigate = vi.fn();

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

const mockUsePlayerService = vi.fn();
const mockUseGameService = vi.fn();

vi.mock('../hooks/usePlayerService', () => ({
  usePlayerService: () => mockUsePlayerService(),
}));

vi.mock('../hooks/useGameService', () => ({
  useGameService: () => mockUseGameService(),
}));

function renderNewGamePage(difficulty: string, size?: number) {
  const path = size !== undefined ? `/new/${difficulty}?size=${size}` : `/new/${difficulty}`;
  return render(
    <MemoryRouter initialEntries={[path]}>
      <Routes>
        <Route path="/new/:difficulty" element={<NewGamePage />} />
      </Routes>
    </MemoryRouter>
  );
}

beforeEach(() => {
  mockNavigate.mockClear();
  mockUsePlayerService.mockClear();
  mockUseGameService.mockClear();
  
  // Default mock implementations
  mockUsePlayerService.mockReturnValue({
    playerAlias: 'test-player',
    profileId: 'profile-123',
    isInitialized: true,
    isLoading: false,
    error: null,
    initializePlayer: vi.fn(),
    clearPlayer: vi.fn(),
  });
  
  mockUseGameService.mockReturnValue({
    savedGames: [],
    isLoading: false,
    error: null,
    isLoaded: false,
    loadGames: vi.fn(),
    deleteGame: vi.fn(),
    createGame: vi.fn().mockResolvedValue(makeGame()),
    clearCache: vi.fn(),
    refreshGames: vi.fn(),
  });
});

describe('NewGamePage', () => {
  it('shows a loading message with the difficulty', () => {
    renderNewGamePage('Easy');
    expect(screen.getByText(/setting out a easy puzzle/i)).toBeInTheDocument();
  });

  it('navigates to game page after successfully creating a game', async () => {
    const game = makeGame({ id: 'new-game-id' });
    const mockCreateGame = vi.fn().mockResolvedValue(game);
    mockUseGameService.mockReturnValue({
      savedGames: [],
      isLoading: false,
      error: null,
      isLoaded: false,
      loadGames: vi.fn(),
      deleteGame: vi.fn(),
      createGame: mockCreateGame,
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    renderNewGamePage('Medium');
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/game/new-game-id', { replace: true });
    });
  });

  it('navigates home when createGame fails', async () => {
    const mockCreateGame = vi.fn().mockRejectedValue(new Error('Network error'));
    mockUseGameService.mockReturnValue({
      savedGames: [],
      isLoading: false,
      error: null,
      isLoaded: false,
      loadGames: vi.fn(),
      deleteGame: vi.fn(),
      createGame: mockCreateGame,
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    renderNewGamePage('Easy');
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });
  });

  it('shows the three breathing dots loader', () => {
    renderNewGamePage('Hard');
    expect(screen.getAllByTestId('loader-dot')).toHaveLength(3);
  });

  it('defaults to size 9 when no size query param is present', async () => {
    const mockCreateGame = vi.fn().mockResolvedValue(makeGame());
    mockUseGameService.mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: mockCreateGame, clearCache: vi.fn(), refreshGames: vi.fn(),
    });
    renderNewGamePage('Easy');
    await waitFor(() => {
      expect(mockCreateGame).toHaveBeenCalledWith('profile-123', 'Easy', 9);
    });
  });

  it('passes the size query param through to createGame', async () => {
    const mockCreateGame = vi.fn().mockResolvedValue(makeGame({ size: 16 }));
    mockUseGameService.mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: mockCreateGame, clearCache: vi.fn(), refreshGames: vi.fn(),
    });
    renderNewGamePage('Expert', 16);
    await waitFor(() => {
      expect(mockCreateGame).toHaveBeenCalledWith('profile-123', 'Expert', 16);
    });
  });

  it('shows a "Preparing puzzles" message and retry button on a 503 pool-empty response', async () => {
    const mockCreateGame = vi.fn().mockRejectedValue(new ApiError('HTTP 503', 503, 30));
    mockUseGameService.mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: mockCreateGame, clearCache: vi.fn(), refreshGames: vi.fn(),
    });
    renderNewGamePage('Expert', 16);
    await waitFor(() => {
      expect(screen.getByText(/Preparing puzzles/i)).toBeInTheDocument();
    });
    expect(screen.getByRole('button', { name: /retry/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /back to home/i })).toBeInTheDocument();
    expect(mockNavigate).not.toHaveBeenCalled();
  });

  it('navigates home when "Back to home" is clicked on a 503 pool-empty response', async () => {
    const mockCreateGame = vi.fn().mockRejectedValue(new ApiError('HTTP 503', 503, 30));
    mockUseGameService.mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: mockCreateGame, clearCache: vi.fn(), refreshGames: vi.fn(),
    });
    const user = userEvent.setup();
    renderNewGamePage('Expert', 16);
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /back to home/i })).toBeInTheDocument();
    });
    await user.click(screen.getByRole('button', { name: /back to home/i }));
    expect(mockNavigate).toHaveBeenCalledWith('/');
  });

  it('retries game creation when the retry button is clicked', async () => {
    const mockCreateGame = vi.fn()
      .mockRejectedValueOnce(new ApiError('HTTP 503', 503, 30))
      .mockResolvedValueOnce(makeGame({ id: 'retried-game', size: 16 }));
    mockUseGameService.mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: mockCreateGame, clearCache: vi.fn(), refreshGames: vi.fn(),
    });
    const user = userEvent.setup();
    renderNewGamePage('Expert', 16);
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /retry/i })).toBeInTheDocument();
    });
    await user.click(screen.getByRole('button', { name: /retry/i }));
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/game/retried-game', { replace: true });
    });
  });

  it('navigates home on a non-503 error, unaffected by the pool-empty handling', async () => {
    const mockCreateGame = vi.fn().mockRejectedValue(new ApiError('HTTP 500', 500));
    mockUseGameService.mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: mockCreateGame, clearCache: vi.fn(), refreshGames: vi.fn(),
    });
    renderNewGamePage('Easy');
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });
  });
});
