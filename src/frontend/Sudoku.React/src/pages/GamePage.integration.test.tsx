import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import GamePage from './GamePage';
import { makeGame } from '../test/helpers';

const mockNavigate = vi.fn();

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

vi.mock('../hooks/usePlayerService', () => ({
  usePlayerService: vi.fn(),
}));

vi.mock('../hooks/useGameService', () => ({
  useGameService: vi.fn(),
}));

import { usePlayerService } from '../hooks/usePlayerService';
import { useGameService } from '../hooks/useGameService';

function renderGamePage(gameId = 'test-game') {
  return render(
    <MemoryRouter initialEntries={[`/game/${gameId}`]}>
      <Routes>
        <Route path="/game/:puzzleId" element={<GamePage />} />
      </Routes>
    </MemoryRouter>
  );
}

beforeEach(() => {
  vi.clearAllMocks();
  mockNavigate.mockClear();
  
  // Mock usePlayerService to return successful initialization
  vi.mocked(usePlayerService).mockReturnValue({
    playerAlias: 'test-player',
    profileId: 'test-player',
    isInitialized: true,
    isNewPlayer: false,
    isLoading: false,
    error: null,
    initializePlayer: vi.fn(),
    clearPlayer: vi.fn(),
  });
  
  // Mock useGameService with default values
  vi.mocked(useGameService).mockReturnValue({
    // Saved games collection
    savedGames: [],
    isLoading: false,
    error: null,
    isLoaded: false,
    loadGames: vi.fn(),
    deleteGame: vi.fn().mockResolvedValue(undefined),
    createGame: vi.fn(),
    clearCache: vi.fn(),
    refreshGames: vi.fn(),
    
    // Single game management  
    currentGame: makeGame(),
    isGameLoading: false,
    gameError: null,
    getGame: vi.fn().mockResolvedValue(makeGame()),
    pauseGame: vi.fn().mockResolvedValue(undefined),
    resumeGame: vi.fn().mockResolvedValue(undefined),
    makeMove: vi.fn(),
    undoMove: vi.fn(),
    resetGame: vi.fn(),
    addPossibleValue: vi.fn(),
    removePossibleValue: vi.fn(),
    clearPossibleValues: vi.fn(),
    clearCurrentGame: vi.fn(),
  });
});

afterEach(() => {
  vi.restoreAllMocks();
});

describe('GamePage - New Architecture', () => {
  it('renders successfully with useGameService and usePlayerService', async () => {
    renderGamePage();
    
    await waitFor(() => {
      // Should render the game board from currentGame
      expect(screen.getByRole('grid')).toBeInTheDocument();
      // Should render the header back-to-home affordance
      expect(screen.getByRole('button', { name: /home/i })).toBeInTheDocument();
      // Should render stats
      expect(screen.getByText('Time')).toBeInTheDocument();
    });
  });

  it('shows loading when game is loading', () => {
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: null,
      isGameLoading: true,
      gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    
    renderGamePage();
    expect(screen.getByText(/Loading puzzle/i)).toBeInTheDocument();
  });

  it('shows error when game service has error', () => {
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: null,
      isGameLoading: false,
      gameError: 'Failed to load game',
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    
    renderGamePage();
    expect(screen.getByText(/Error loading game: Failed to load game/)).toBeInTheDocument();
    expect(screen.getByText(/Go Home/)).toBeInTheDocument();
  });

  it('calls getGame on mount when player is initialized', async () => {
    const mockGetGame = vi.fn().mockResolvedValue(makeGame());
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: null, isGameLoading: false, gameError: null,
      getGame: mockGetGame, pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    
    renderGamePage('test-game-id');
    
    await waitFor(() => {
      expect(mockGetGame).toHaveBeenCalledWith('test-player', 'test-game-id');
    });
  });

  it('does not call getGame when player is not initialized', () => {
    vi.mocked(usePlayerService).mockReturnValue({
      playerAlias: null,
      profileId: null,
      isInitialized: false,
      isNewPlayer: true,
      isLoading: false,
      error: null,
      initializePlayer: vi.fn(),
      clearPlayer: vi.fn(),
    });

    const mockGetGame = vi.fn();
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: null, isGameLoading: false, gameError: null,
      getGame: mockGetGame, pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    
    renderGamePage();
    
    // Should not call getGame when player is not initialized
    expect(mockGetGame).not.toHaveBeenCalled();
  });
});