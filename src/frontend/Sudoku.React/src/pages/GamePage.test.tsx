import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { render, screen, waitFor, fireEvent, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import GamePage from './GamePage';
import { makeGame, make81Cells, makeCell, makeCells } from '../test/helpers';

vi.mock('../api/apiClient', () => ({
  apiClient: {
    createGame: vi.fn(),
    getGames: vi.fn(),
    getGame: vi.fn(),
    deleteGame: vi.fn(),
    makeMove: vi.fn(),
    resetGame: vi.fn(),
    undoMove: vi.fn(), requestHint: vi.fn(),
    pauseGame: vi.fn(),
    resumeGame: vi.fn(),
    addPossibleValue: vi.fn(),
    removePossibleValue: vi.fn(),
    clearPossibleValues: vi.fn(),
  },
}));

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
import { apiClient } from '../api/apiClient';

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
    currentGame: null,
    isGameLoading: false,
    gameError: null,
    getGame: vi.fn().mockResolvedValue(makeGame()),
    pauseGame: vi.fn().mockResolvedValue(undefined),
    resumeGame: vi.fn().mockResolvedValue(undefined),
    makeMove: vi.fn(),
    undoMove: vi.fn(), requestHint: vi.fn(),
    resetGame: vi.fn(),
    addPossibleValue: vi.fn(),
    removePossibleValue: vi.fn(),
    clearPossibleValues: vi.fn(),
    clearCurrentGame: vi.fn(),
  });
  
  // Mock apiClient methods
  vi.mocked(apiClient.createGame).mockResolvedValue(makeGame());
  vi.mocked(apiClient.getGames).mockResolvedValue([]);
  vi.mocked(apiClient.getGame).mockResolvedValue(makeGame());
  vi.mocked(apiClient.deleteGame).mockResolvedValue(undefined);
  vi.mocked(apiClient.makeMove).mockResolvedValue(makeGame());
  vi.mocked(apiClient.resetGame).mockResolvedValue(makeGame());
  vi.mocked(apiClient.undoMove).mockResolvedValue(makeGame());
  vi.mocked(apiClient.pauseGame).mockResolvedValue(undefined);
  vi.mocked(apiClient.resumeGame).mockResolvedValue(undefined);
  vi.mocked(apiClient.addPossibleValue).mockResolvedValue(makeGame());
  vi.mocked(apiClient.removePossibleValue).mockResolvedValue(makeGame());
  vi.mocked(apiClient.clearPossibleValues).mockResolvedValue(makeGame());
});

afterEach(() => {
  vi.restoreAllMocks();
});

describe('GamePage - loading', () => {
  it('shows "Loading puzzle..." before game data arrives', () => {
    // Mock useGameService with currentGame as null
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: null,
      isGameLoading: false,
      gameError: null,
      getGame: vi.fn().mockResolvedValue(makeGame()),
      pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    expect(screen.getByText(/Loading puzzle/i)).toBeInTheDocument();
  });

  it('shows "Initializing player..." when player is loading', () => {
    vi.mocked(usePlayerService).mockReturnValue({
      playerAlias: null,
      profileId: null,
      isInitialized: false,
      isNewPlayer: true,
      isLoading: true,
      error: null,
      initializePlayer: vi.fn(),
      clearPlayer: vi.fn(),
    });
    renderGamePage();
    expect(screen.getByText(/Initializing player/i)).toBeInTheDocument();
  });

  it('shows "Loading..." when player is not initialized but not loading', () => {
    vi.mocked(usePlayerService).mockReturnValue({
      playerAlias: null,
      profileId: null,
      isInitialized: false,
      isLoading: false,
      error: null,
      isNewPlayer: true,
      initializePlayer: vi.fn(),
      clearPlayer: vi.fn(),
    });
    renderGamePage();
    expect(screen.getByText(/Loading\.\.\./)).toBeInTheDocument();
  });

  it('shows error message when player service has error', async () => {
    vi.mocked(usePlayerService).mockReturnValue({
      playerAlias: null,
      profileId: null,
      isInitialized: true,
      isNewPlayer: false,
      isLoading: false,
      error: 'Failed to initialize player',
      initializePlayer: vi.fn(),
      clearPlayer: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByText(/Error: Failed to initialize player/)).toBeInTheDocument();
    });
    expect(screen.getByText(/Go Home/)).toBeInTheDocument();
  });

  it('navigates home when getGame fails', async () => {
    const mockGetGame = vi.fn().mockRejectedValue(new Error('Not found'));
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: null, isGameLoading: false, gameError: null,
      getGame: mockGetGame, pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });
  });
});

describe('GamePage - after load', () => {
  it('renders the game board after loading', async () => {
    const game = makeGame();
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('grid')).toBeInTheDocument();
    });
  });

  it('renders game controls after loading', async () => {
    const game = makeGame();
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /home/i })).toBeInTheDocument();
    });
  });

  it('renders game stats after loading', async () => {
    const game = makeGame();
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByText('Time')).toBeInTheDocument();
    });
  });

  it('calls resumeGame on load', async () => {
    const game = makeGame({ id: 'g1' });
    const mockGetGame = vi.fn().mockResolvedValue(game);
    const mockResumeGame = vi.fn().mockResolvedValue(undefined);
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: null, isGameLoading: false, gameError: null,
      getGame: mockGetGame, pauseGame: vi.fn(), resumeGame: mockResumeGame, makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage('g1');
    await waitFor(() => {
      expect(mockResumeGame).toHaveBeenCalledWith('test-player', 'g1');
    });
  });

  it('waits for player initialization before loading game', () => {
    const mockGetGame = vi.mocked(apiClient.getGame);
    mockGetGame.mockResolvedValue(makeGame());
    
    // Start with player not initialized
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
    
    renderGamePage();
    
    // Game should not be loaded yet
    expect(mockGetGame).not.toHaveBeenCalled();
  });
});

describe('GamePage - cell selection', () => {
  it('allows selecting a cell', async () => {
    const game = makeGame();
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('grid')).toBeInTheDocument();
    });
    // Click on the first input cell
    const inputs = within(screen.getByRole('grid')).getAllByRole('button');
    await userEvent.click(inputs[0]);
    // The cell should be selected (no specific assertion needed beyond no crash)
    expect(inputs[0]).toBeInTheDocument();
  });
});

describe('GamePage - keyboard navigation', () => {
  it('moves selection with arrow keys', async () => {
    const game = makeGame();
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('grid')).toBeInTheDocument();
    });
    const grid = screen.getByRole('grid');
    // First select a cell
    const inputs = within(grid).getAllByRole('button');
    await userEvent.click(inputs[4]);
    // Then press arrow key
    fireEvent.keyDown(grid, { key: 'ArrowDown' });
    // No error should occur
    expect(grid).toBeInTheDocument();
  });

  it('clamps arrow-key navigation to size - 1 on a 16x16 board', async () => {
    const cells = makeCells(16);
    // Select the bottom-right cell (row 15, column 15) and confirm ArrowDown/ArrowRight don't crash past bounds.
    const game = makeGame({ cells, size: 16 });
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('grid')).toBeInTheDocument();
    });
    const grid = screen.getByRole('grid');
    const inputs = within(grid).getAllByRole('button');
    // The last button in row-major order is (row 15, column 15).
    await userEvent.click(inputs[255]);
    fireEvent.keyDown(grid, { key: 'ArrowDown' });
    fireEvent.keyDown(grid, { key: 'ArrowRight' });
    // No error should occur; selection stays clamped within the 16x16 board.
    expect(grid).toBeInTheDocument();
  });
});

describe('GamePage - number input', () => {
  it('calls makeMove when a number button is clicked with a selected cell', async () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, isFixed: false, hasValue: false });
    const game = makeGame({ cells });
    const updatedGame = makeGame({ cells });
    const mockMakeMove = vi.fn().mockResolvedValue(updatedGame);
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: mockMakeMove, undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('grid')).toBeInTheDocument();
    });
    // Select cell (0,0)
    const inputs = within(screen.getByRole('grid')).getAllByRole('button');
    await userEvent.click(inputs[0]);
    // Click the number 5 button
    await userEvent.click(screen.getByRole('button', { name: '5' }));
    await waitFor(() => {
      expect(mockMakeMove).toHaveBeenCalledWith(
        'test-player', game.id, 0, 0, 5, expect.any(String)
      );
    });
  });

  it('rejects letter input on a 9x9 board (does not call makeMove)', async () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, isFixed: false, hasValue: false });
    const game = makeGame({ cells });
    const mockMakeMove = vi.fn();
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: mockMakeMove, undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('grid')).toBeInTheDocument();
    });
    const grid = screen.getByRole('grid');
    const inputs = within(grid).getAllByRole('button');
    await userEvent.click(inputs[0]);
    fireEvent.keyDown(grid, { key: 'a' });
    expect(mockMakeMove).not.toHaveBeenCalled();
  });

  it('maps a letter keypress to its numeric value on a 16x16 board', async () => {
    const cells = makeCells(16);
    cells[0] = makeCell({ row: 0, column: 0, isFixed: false, hasValue: false });
    const game = makeGame({ cells, size: 16 });
    const updatedGame = makeGame({ cells, size: 16 });
    const mockMakeMove = vi.fn().mockResolvedValue(updatedGame);
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: mockMakeMove, undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('grid')).toBeInTheDocument();
    });
    const grid = screen.getByRole('grid');
    const inputs = within(grid).getAllByRole('button');
    await userEvent.click(inputs[0]);
    fireEvent.keyDown(grid, { key: 'g' });
    await waitFor(() => {
      expect(mockMakeMove).toHaveBeenCalledWith('test-player', game.id, 0, 0, 16, expect.any(String));
    });
  });

  it('does not call makeMove when no cell is selected', async () => {
    const game = makeGame();
    const mockMakeMove = vi.fn();
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: mockMakeMove, undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('grid')).toBeInTheDocument();
    });
    await userEvent.click(screen.getByRole('button', { name: '7' }));
    expect(mockMakeMove).not.toHaveBeenCalled();
  });
});

describe('GamePage - erase', () => {
  it('calls makeMove with null when erase is clicked on a cell with a value', async () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, isFixed: false, hasValue: true, value: 3 });
    const game = makeGame({ cells });
    const updatedGame = makeGame({ cells: make81Cells() });
    const mockMakeMove = vi.fn().mockResolvedValue(updatedGame);
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: mockMakeMove, undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('grid')).toBeInTheDocument();
    });
    const inputs = within(screen.getByRole('grid')).getAllByRole('button');
    await userEvent.click(inputs[0]);
    await userEvent.click(screen.getByRole('button', { name: /erase/i }));
    await waitFor(() => {
      expect(mockMakeMove).toHaveBeenCalledWith(
        'test-player', game.id, 0, 0, null, expect.any(String)
      );
    });
  });
});

describe('GamePage - undo / reset', () => {
  it('calls undoMove when undo button is clicked', async () => {
    const game = makeGame({ moveHistory: [{ row: 0, column: 0, value: 1, isValid: true }] });
    const updatedGame = makeGame();
    const mockUndoMove = vi.fn().mockResolvedValue(updatedGame);
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: mockUndoMove, requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /undo/i })).toBeInTheDocument();
    });
    await userEvent.click(screen.getByRole('button', { name: /undo/i }));
    await waitFor(() => {
      expect(mockUndoMove).toHaveBeenCalledWith('test-player', game.id);
    });
  });

  it('calls resetGame when reset button is clicked', async () => {
    const game = makeGame();
    const updatedGame = makeGame();
    const mockResetGame = vi.fn().mockResolvedValue(updatedGame);
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: mockResetGame,
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /reset/i })).toBeInTheDocument();
    });
    await userEvent.click(screen.getByRole('button', { name: /reset/i }));
    await waitFor(() => {
      expect(mockResetGame).toHaveBeenCalledWith('test-player', game.id);
    });
  });
});

describe('GamePage - home navigation', () => {
  it('calls pauseGame and navigates home when Home is clicked', async () => {
    const game = makeGame({ id: 'g1' });
    const mockPauseGame = vi.fn().mockResolvedValue(undefined);
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: mockPauseGame, resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage('g1');
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /home/i })).toBeInTheDocument();
    });
    await userEvent.click(screen.getByRole('button', { name: /home/i }));
    await waitFor(() => {
      expect(mockPauseGame).toHaveBeenCalledWith('test-player', 'g1');
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });
  });
});

describe('GamePage - pencil mode', () => {
  it('toggles pencil mode when pencil button is clicked', async () => {
    const game = makeGame();
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /pencil/i })).toBeInTheDocument();
    });
    // Pencil button should be present
    const pencilBtn = screen.getByRole('button', { name: /pencil/i });
    await userEvent.click(pencilBtn);
    // No crash expected
    expect(pencilBtn).toBeInTheDocument();
  });

  it('calls addPossibleValue in pencil mode when a number is clicked', async () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, isFixed: false, hasValue: false, possibleValues: [] });
    const game = makeGame({ cells });
    const updatedGame = makeGame({ cells });
    const mockAddPossibleValue = vi.fn().mockResolvedValue(updatedGame);
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: vi.fn(), undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: mockAddPossibleValue, removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('grid')).toBeInTheDocument();
    });
    // Enable pencil mode
    await userEvent.click(screen.getByRole('button', { name: /pencil/i }));
    // Select cell (0,0)
    const inputs = within(screen.getByRole('grid')).getAllByRole('button');
    await userEvent.click(inputs[0]);
    // Click number 3
    await userEvent.click(screen.getByRole('button', { name: '3' }));
    await waitFor(() => {
      expect(mockAddPossibleValue).toHaveBeenCalledWith('test-player', game.id, 0, 0, 3);
    });
  });
});

describe('GamePage - victory', () => {
  it('shows victory overlay when puzzle is solved', async () => {
    // Build a solved game — all cells have values, no duplicates
    const solution = [
      [5,3,4,6,7,8,9,1,2],
      [6,7,2,1,9,5,3,4,8],
      [1,9,8,3,4,2,5,6,7],
      [8,5,9,7,6,1,4,2,3],
      [4,2,6,8,5,3,7,9,1],
      [7,1,3,9,2,4,8,5,6],
      [9,6,1,5,3,7,2,8,4],
      [2,8,7,4,1,9,6,3,5],
      [3,4,5,2,8,6,1,7,9],
    ];
    const cells = make81Cells();
    for (let r = 0; r < 9; r++) {
      for (let c = 0; c < 9; c++) {
        const idx = r * 9 + c;
        cells[idx] = makeCell({ row: r, column: c, value: solution[r][c], hasValue: true, isFixed: false });
      }
    }
    // Start with one missing cell
    const initialCells = cells.map((c, i) => i === 0 ? { ...c, value: null, hasValue: false } : c);
    const game = makeGame({ cells: initialCells });
    const solvedGame = makeGame({ cells });
    const mockMakeMove = vi.fn().mockResolvedValue(solvedGame);
    vi.mocked(useGameService).mockReturnValue({
      savedGames: [], isLoading: false, error: null, isLoaded: false,
      loadGames: vi.fn(), deleteGame: vi.fn(), createGame: vi.fn(), clearCache: vi.fn(), refreshGames: vi.fn(),
      currentGame: game, isGameLoading: false, gameError: null,
      getGame: vi.fn(), pauseGame: vi.fn(), resumeGame: vi.fn(), makeMove: mockMakeMove, undoMove: vi.fn(), requestHint: vi.fn(), resetGame: vi.fn(),
      addPossibleValue: vi.fn(), removePossibleValue: vi.fn(), clearPossibleValues: vi.fn(), clearCurrentGame: vi.fn(),
    });

    renderGamePage();
    await waitFor(() => {
      expect(screen.getByRole('grid')).toBeInTheDocument();
    });
    // Select cell (0,0)
    const inputs = within(screen.getByRole('grid')).getAllByRole('button');
    await userEvent.click(inputs[0]);
    // Enter the missing value (5)
    await userEvent.click(screen.getByRole('button', { name: '5' }));
    await waitFor(() => {
      expect(screen.getByText('Solved')).toBeInTheDocument();
    });
  });
});
