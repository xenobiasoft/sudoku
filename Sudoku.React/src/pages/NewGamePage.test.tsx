import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import NewGamePage from './NewGamePage';
import { makeGame } from '../test/helpers';

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

function renderNewGamePage(difficulty: string) {
  return render(
    <MemoryRouter initialEntries={[`/new/${difficulty}`]}>
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
    expect(screen.getByText(/Creating Easy puzzle/i)).toBeInTheDocument();
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

  it('navigates home when player is not initialized', async () => {
    mockUsePlayerService.mockReturnValue({
      playerAlias: null,
      isInitialized: false,
      isLoading: false,
      error: null,
      initializePlayer: vi.fn(),
      clearPlayer: vi.fn(),
    });
    renderNewGamePage('Hard');
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/');
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

  it('shows the numeric loader with digits 1-9', () => {
    renderNewGamePage('Hard');
    for (let n = 1; n <= 9; n++) {
      expect(screen.getByText(n.toString())).toBeInTheDocument();
    }
  });
});
